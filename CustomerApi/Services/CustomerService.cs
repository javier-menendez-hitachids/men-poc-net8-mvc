using Contentful.Core.Models.Management;
using MenulioPocMvc.CustomerApi.Interfaces;
using MenulioPocMvc.CustomerApi.Services.Interface;
using MenulioPocMvc.Models.Apis;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.IdentityModel.Tokens.Jwt;
using MenulioPocMvc.Models.Enums;
using MenulioPocMvc.Models.Mulesoft;
using System.Security.Claims;

namespace MenulioPocMvc.CustomerApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IApiCalls _api;
        private readonly string _baseUri;

        public CustomerService(ILogger<CustomerService> logger, IConfiguration configuration, IApiCalls apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _configuration = configuration;
            _api = apiClient;
            _baseUri = _configuration["CustomerApi:BaseUri"] ?? string.Empty;

            if (IsUrlValid(_baseUri))
            {
                _baseUri = _configuration["CustomerApi:BaseUri"] ?? string.Empty;
            }
            else
            {
                _logger.LogError("Base URI for IMG API is not a valid URI");
            }

            _httpContextAccessor = httpContextAccessor;
        }

        private bool IsUrlValid(string url)
        {
            Uri uriResult;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult!)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public async Task<SignInResponse> SignIn(string emailAddress, string password)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return null;

            var processedEmail = emailAddress.ToLower().TrimEnd('.').Trim();
            var uri = _baseUri + "signin";
            var signInBody = string.Format(@"client_id=4e8ce86a9c914dd8a41c09e0709ef297&grant_type=password&Username={0}&Password={1}&ReferenceType=EMAIL", HttpUtility.UrlEncode(processedEmail), HttpUtility.UrlEncode(password));

            var apiResponse = await _api.Post(uri, ContentType.Xml, signInBody);

            var newResponse = new SignInResponse();
            newResponse.Response = apiResponse.Response;

            if (!newResponse.Response.IsSuccessStatusCode)
            {
                var failResponse = JsonConvert.DeserializeObject<SignInResponse.FailLogin>(apiResponse.ResponseBody);

                newResponse.Error = failResponse.error;
                newResponse.ErrorDescription = failResponse.error_description;

                return newResponse;
            }

            var body = JsonConvert.DeserializeObject<SignInResponse.SuccessfulLogin>(apiResponse.ResponseBody);

            newResponse.AuthToken = body.access_token;
            newResponse.NeedsRepermission = IsRepermission(newResponse.AuthToken);
            newResponse.NeedsPrivilege = IsPrivilege(newResponse.AuthToken);

            return newResponse;
        }

        public async Task<bool> SocialSignIn(ReferenceType provider, string socialId)
        {
            if (string.IsNullOrEmpty(socialId))
                return false;

            var signInBody = string.Format(@"client_id=4e8ce86a9c914dd8a41c09e0709ef297&grant_type=password&socialId={0}&ReferenceType={1}", HttpUtility.UrlEncode(socialId), Enum.GetName(typeof(ReferenceType), provider));

            var apiResponse = await _api.Post(_baseUri + "socialSignin", ContentType.Xml, signInBody);

            return apiResponse.Response.IsSuccessStatusCode;
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var xml = await GetCustomerXml(customerId);

            return CustomerMapper.MapToCustomer(xml);
        }

        public async Task<Customer> GetCustomerByRef(ReferenceType refType, string refValue)
        {
            var valueToUse = HttpUtility.UrlEncode(refValue);
            var apiResponse = await _api.Get(_baseUri + string.Format("customer?reftype={0}&refvalue={1}", Enum.GetName(typeof(ReferenceType), refType), valueToUse), ContentType.Xml);
            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogError("Customer not found by API");
                return null;
            }

            var xml = XDocument.Parse(apiResponse.ResponseBody);
            return CustomerMapper.MapToCustomer(xml);
        }

        public async Task<RegisterCustomerResponse> RegisterCustomer(Customer customer)
        {
            var body = JsonConvert.SerializeObject(CustomerMapper.MapRegistrationToJson(customer));
            string uri = _baseUri + "customer";
            var apiResponse = await _api.Post(uri, ContentType.Json, body);
            
            return FormatResponse(apiResponse, customer);
        }

        public async Task<UpdateProfileResponse> UpdateProfile(Customer customer)
        {
            // call the API to get the original Customer XML
            var originalXml = await GetCustomerXml(customer.Id);

            // pass in the original XML to the mapper
            // need to remove XML properties if they are not set
            var updatedXml = CustomerMapper.MapToXml(customer, originalXml);

            BaseResponse apiResponse = await _api.Put(_baseUri + "customer", ContentType.Xml, updatedXml.ToString());

            var newResponse = new UpdateProfileResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<IEnumerable<CustomerPreference>> GetCustomerPreferences(Guid customerId)
        {
            var apiResponse = await _api.Get(_baseUri + "customerpreferences/" + customerId, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                return null;
            }

            var xml = XDocument.Parse(apiResponse.ResponseBody);
            if (xml.Root == null)
            {
                return null;
            }


            var prefs = xml.Root.Descendants().Select(i => new CustomerPreference()
            {
                CustomerId = customerId,
                Group = i.Attribute("Group").Value,
                Key = i.Attribute("Key").Value,
                Value = bool.Parse(i.Attribute("Value").Value),
                Label = GetPreferenceLabel(i.Attribute("Key").Value)
            });

            return prefs;
        }

        /// <summary>
        /// Deletes all old preferences passed in as a parameter. Then creates the new preferences.
        /// Best use is to pass in all old and new preferences belonging to a specific group.
        /// </summary>
        /// <returns></returns>
        public async Task CreateCustomerPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        {
            if (oldPreferences != null && newPreferences != null)
            {
                // remove null keys
                newPreferences = newPreferences.Where(p => p.Key != null);

                // only delete old pref if they are not in new ones
                var resultsToDelete = oldPreferences.Where(p => !newPreferences.Any(p2 => p2.Key == p.Key));

                foreach (var pref in resultsToDelete)
                {
                    var idString = string.Format("{0}|{1}|{2}", customerId, pref.Group, pref.Key);
                    var apiResponse = await _api.Delete(_baseUri + "customerpreference/" + idString,
                        ContentType.Xml, "");
                }

                // only add new preferences if thet are not in the old ones
                var resultsToAdd = newPreferences.Where(p => !oldPreferences.Any(p2 => p2.Key == p.Key));

                foreach (var pref in resultsToAdd)
                {
                    var body = string.Format(@"<CustomerPreference CustomerId=""{0}"" Group=""{1}"" Key=""{2}"" Value=""{3}"" />",
                            customerId, pref.Group, pref.Key, pref.Value);

                    var apiResponse = await _api.Post(_baseUri + "customerpreference", ContentType.Xml, body);
                }
            }
        }

        /// <summary>
        /// Deletes all old preferences passed in as a parameter. Then creates the new preferences.
        /// Best use is to pass in all old and new preferences belonging to a specific group.
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePreferencesMulesoft(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        {
            var payload = new CustomerPreferencesUpdate
            {
                OldPreferences = oldPreferences,
                NewPreferences = newPreferences
            };

            var apiResponse = await _api.Post(_baseUri + "customerPreferences/" + customerId, ContentType.Json, JsonConvert.SerializeObject(payload));
        }

        public async Task CreateCustomerContactPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        {
            // Delete old preferences
            if (oldPreferences != null)
            {
                foreach (var pref in oldPreferences)
                {
                    var idString = string.Format("{0}|{1}|{2}", customerId, pref.Group, pref.Key);
                    var apiResponse = await _api.Delete(_baseUri + "customerpreference/" + idString,
                        ContentType.Xml, "");
                }
            }

            // Now add new preferences
            if (newPreferences != null)
            {
                foreach (var pref in newPreferences)
                {
                    var body = string.Format(@"<item type=""OPTOUTEMAIL"">{0}</item>", pref.Value);

                    var apiResponse = await _api.Post(_baseUri + "customerpreference", ContentType.Xml, body);
                }
            }
        }

        public async Task<CustomerExtended> GetCustomerMembershipInfo(Guid customerId)
        {
            var apiResponse = await _api.Get(_baseUri + "customerpoints/" + customerId, ContentType.Json);
            CustomerExtended customerPointsPrivilege = JsonConvert.DeserializeObject<CustomerExtended>(apiResponse.ResponseBody);

            return customerPointsPrivilege;
        }

        public async Task<GuestInfo> GetCustomerMembershipInfoV2(Guid customerId)
        {
            GuestInfo customerPointsPrivilege = new GuestInfo();
            var apiResponse = await _api.Get(_baseUri + "customerpoints/" + customerId, ContentType.Json);

            if (apiResponse.Response.IsSuccessStatusCode)
                customerPointsPrivilege = JsonConvert.DeserializeObject<GuestInfo>(apiResponse.ResponseBody);
            else
                _logger.LogError($"Error retrieving customer points. {apiResponse.Response.StatusCode}");

            return customerPointsPrivilege;
        }

        public async Task<SpendTier> GetCustomerSpendTierInfo(Guid tierId)
        {
            var apiResponse = await _api.Get(_baseUri + "segment/" + tierId, ContentType.Json);
            SpendTier spendTier = JsonConvert.DeserializeObject<SpendTier>(apiResponse.ResponseBody);

            return spendTier;
        }

        public async Task<SegmentInfo> GetCustomerSpendTierInfoV2(string tierId)
        {
            // TODO: Adapt to Contentful
            //var language = ContentHelper.GetSiteLanguageRoot();
            string language = "en";
            SegmentInfo spendTier = new SegmentInfo();
            
            var url = $"{_baseUri}/segment/{tierId}?language={language}";
            var apiResponse = await _api.Get(url, ContentType.Json);

            if (apiResponse.Response.IsSuccessStatusCode)
                spendTier = JsonConvert.DeserializeObject<SegmentInfo>(apiResponse.ResponseBody);
            else
                _logger.LogError($"Error retrieving customer segment. {apiResponse.Response.StatusCode}");

            return spendTier;
        }

        public async Task<RewardsInfo> GetCustomerOffersV2(Guid customerId, string language = null)
        {
            // TDDO: Adapt to Contentful
            //var rootNode = ContentHelper.GetSiteRoot();
            //var siteCode = rootNode.GetPropertyValue<string>("siteCode");
            string siteCode = "KV";

            var endpoint = $"{_baseUri}customeroffers/{customerId}/{siteCode}";

            if (!string.IsNullOrWhiteSpace(language))
            {
                var languageClean = language.Trim().Replace(" ", "").ToLower();
                endpoint += $"?language={languageClean}";
            }

            var apiResponse = await _api.Get(endpoint, ContentType.Json);
            RewardsInfo offers = new RewardsInfo
            {
                Offers = new List<Models.Mulesoft.CustomerOffer>()
            };

            if (apiResponse.Response.IsSuccessStatusCode)
                offers = JsonConvert.DeserializeObject<RewardsInfo>(apiResponse.ResponseBody);
            else
                _logger.LogError($"Error retrieving customer offers: {apiResponse.Response.StatusCode}");

            return offers;
        }

        public async IAsyncEnumerable<Models.Apis.ICustomerOffer> GetCustomerOffers(Guid customerId, string language = null)
        {
            var xml = language == null ?
                await this.GetCustomerOffersXml(customerId) : await this.GetCustomerOffersXml(customerId, language);

            if (xml.Root == null)
            {
                yield break;
            }

            var offers = xml.Root.Descendants();

            foreach (var xmlOffer in offers)
            {
                if (!Guid.TryParse(GetXmlAttribute(xmlOffer, "CategoryId"), out Guid categoryId))
                {
                    categoryId = Guid.Empty;
                }

                var offer = new Models.Apis.CustomerOffer()
                {
                    BarcodeUrl = GetXmlAttribute(xmlOffer, "BarcodeUrl"),
                    BarcodeNumber = GetXmlAttribute(xmlOffer, "BarcodeNumber"),
                    Conditions = GetXmlAttribute(xmlOffer, "Conditions"),
                    Description = GetXmlAttribute(xmlOffer, "Description"),
                    IsActive = bool.Parse(GetXmlAttribute(xmlOffer, "IsActive")),
                    OfferId = GetXmlAttribute(xmlOffer, "OfferId"),
                    Redeemed = bool.Parse(GetXmlAttribute(xmlOffer, "Redeemed")),
                    Title = GetXmlAttribute(xmlOffer, "Title"),
                    Title2 = GetXmlAttribute(xmlOffer, "Title2"),
                    PassbookTitle = GetXmlAttribute(xmlOffer, "PassbookTitle"),
                    ImageUrl = GetXmlAttribute(xmlOffer, "ImageURL"),
                    VillageID = GetXmlAttribute(xmlOffer, "VillageId"),
                    ConditionsUrl = GetXmlAttribute(xmlOffer, "ConditionsUrl"),
                    CategoryId = categoryId
                };

                yield return offer;

                // Add a small delay to avoid blocking
                await Task.Delay(1);
            }
        }


        private async Task<XDocument> GetCustomerOffersXml(Guid customerId, string language = null)
        {
            // TDDO: Adapt to Contentful
            //var rootNode = ContentHelper.GetSiteRoot();
            //var siteCode = rootNode.GetPropertyValue<string>("siteCode");
            string siteCode = "KV";
            
            var endpoint = _baseUri + "customeroffers/" + customerId + "/" + siteCode;

            if (!string.IsNullOrWhiteSpace(language))
            {
                var languageClean = language.Trim().Replace(" ", "").ToLower();
                endpoint += $"?language={languageClean}";
            }

            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            var xml = new XDocument();

            if (apiResponse.ResponseBody != null)
            {
                xml = XDocument.Parse(apiResponse.ResponseBody);
            }

            return xml;
        }

        public async Task<IList<Models.Apis.CustomerTreatCategory>> GetTreatCategories(Guid customerId, string villageCode, string language = null)
        {
            var endpoint = $"{_baseUri}customer/{customerId}/treatcategories?villageCode={villageCode}";

            if (!string.IsNullOrWhiteSpace(language))
            {
                var languageClean = language.Trim().Replace(" ", "").ToLower();
                endpoint += $"&language={languageClean}";
            }

            var response = await _api.Get(endpoint, ContentType.Json);
            var result = new List<Models.Apis.CustomerTreatCategory>();

            if (!string.IsNullOrWhiteSpace(response.ResponseBody))
            {
                result = JsonConvert.DeserializeObject<List<Models.Apis.CustomerTreatCategory>>(response.ResponseBody);
            }

            return result;
        }

        public async Task<IList<Models.Mulesoft.CustomerTreatCategory>> GetTreatCategoriesV2(Guid customerId, string villageCode, string language = null)
        {
            var endpoint = $"{_baseUri}customer/{customerId}/treatcategories?village={villageCode}";

            if (!string.IsNullOrWhiteSpace(language))
            {
                var languageClean = language.Trim().Replace(" ", "").ToLower();
                endpoint += $"&language={languageClean}";
            }

            var apiResponse = await _api.Get(endpoint, ContentType.Json);
            var result = new List<Models.Mulesoft.CustomerTreatCategory>();

            if (!apiResponse.Response.IsSuccessStatusCode)
                _logger.LogError($"Error retrieving customer treat categories. {apiResponse.Response.StatusCode}");
            else if (!string.IsNullOrWhiteSpace(apiResponse.ResponseBody))
                result = JsonConvert.DeserializeObject<List<Models.Mulesoft.CustomerTreatCategory>>(apiResponse.ResponseBody);

            return result;
        }

        public async IAsyncEnumerable<Models.Apis.ICustomerOffer> GetVillageTeasers(string villageCode, string languageId)
        {
            var endpoint = $"{_baseUri}Teasers/{villageCode}/{languageId}";
            var response = await _api.Get(endpoint, ContentType.Json);

            if (response.Response.IsSuccessStatusCode)
            {
                var responseContent = await response.Response.Content.ReadAsStringAsync();
                var villageTeaser = JsonConvert.DeserializeObject<VillageTeaser>(response.ResponseBody);

                if (villageTeaser != null)
                {
                    // Adapt to Contentful
                    //var customerOffer = TreatsMapper.Map(villageTeaser);
                    yield return new Models.Apis.CustomerOffer();
                }
            }
            else
            {
                _logger.LogWarning("Failed to get village teasers. Status code: {StatusCode}", response.Response.StatusCode);
            }
        }

        /// <summary>
        /// TO BE REMOVED
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<CustomerBrandPreference> GetCustomerBrandPreferences(Guid customerId)
        {
            var apiResponse = await _api.Get(_baseUri + "CustomerBrandPreferences/" + customerId, ContentType.Xml);
            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                yield return null;
            }

            var xml = XDocument.Parse(apiResponse.ResponseBody);

            if (xml.Root == null)
            {
                yield break;
            }

            var brandPreferences = xml.Root.Descendants();

            foreach (var xmlBrandPreference in brandPreferences)
            {
                var brandPreference = new CustomerBrandPreference
                {
                    BrandId = GetXmlAttribute(xmlBrandPreference, "BrandId")
                };

                yield return brandPreference;
            }
        }

        public async IAsyncEnumerable<ApiEnum> GetVillageBrandsAsync(string villageId)
        {
            var endpoint = $"{_baseUri}villagebrands/{villageId.ToLower()}";
            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get village brands. Status code: {StatusCode}", apiResponse.Response.StatusCode);
                yield break;
            }

            var xml = XDocument.Parse(apiResponse.ResponseBody);

            if (xml.Root == null)
            {
                _logger.LogWarning("Village brands XML response has no root element");
                yield break;
            }

            foreach (var element in xml.Root.Descendants())
            {
                var key = element.Attribute("Key")?.Value;
                if (string.IsNullOrEmpty(key))
                {
                    _logger.LogWarning("Skipping element with missing or empty Key attribute");
                    continue;
                }

                yield return new ApiEnum
                {
                    Key = key,
                    Value = element.Value
                };

                await Task.Yield(); // Allows other tasks to run
            }
        }

        public async Task<bool> CreateCustomerBrandPreferences(Guid customerId, string brandId)
        {
            if (string.IsNullOrEmpty(brandId))
            {
                return false;
            }

            var apiResponse = await _api.Post(_baseUri + "customerbrandpreference", ContentType.Xml, 
                string.Format(@"<CustomerBrandPreference CustomerId=""{0}"" BrandId=""{1}"" />", customerId, brandId));
            return apiResponse.Response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerBrandPreferences(Guid customerId, string brandId)
        {
            if (string.IsNullOrEmpty(brandId))
            {
                return false;
            }

            var apiResponse = await _api.Delete(string.Format(_baseUri + "customerbrandpreference/{0}|{1}", customerId, brandId), ContentType.Xml, "");
            return apiResponse.Response.IsSuccessStatusCode;
        }

        public async Task<ForgottenPasswordResponse> ResetPasswordRequest(string emailAddress)

        {
            BaseResponse apiResponse = await _api.Post(_baseUri + "customer/passwordresetrequest/" + HttpUtility.UrlEncode(emailAddress) + "/", ContentType.Xml, "");
            var newResponse = new ForgottenPasswordResponse
            {
                Response = apiResponse.Response
            };
            return newResponse;
        }

        public async Task<BaseResponse> InitialisePassword(Customer customer)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/passwordinitialise/" + customer.Id, ContentType.Form, "password=" + HttpUtility.UrlEncode(customer.Password));
            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<BaseResponse> ResetPassword(string token, string newPassword)
        {
            BaseResponse apiResponse = await _api.Post(_baseUri + "customer/passwordreset/" + token, ContentType.Form, "password=" + HttpUtility.UrlEncode(newPassword));

            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<bool> ChangePassword(Guid customerId, string newPassword)
        {
            BaseResponse apiResponse = await _api.Post(_baseUri + "customer/passwordchange/" + customerId, ContentType.Form, "password=" + HttpUtility.UrlEncode(newPassword));

            if (apiResponse.Response.StatusCode == HttpStatusCode.NoContent)
            {
                return false;
            }

            if (apiResponse.Response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<RegisterCustomerResponse> ConfirmEmail(string token)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/ConfirmEmail/" + token, ContentType.Xml, "");
            return FormatConfirmEmailResponse(apiResponse);
        }

        // Post
        public async Task<BaseResponse> ResetEmail(Guid customerId, string oldEmail, string newEmail)
        {
            var requestBody = string.Format("oldEmail={0}&newEmail={1}", HttpUtility.UrlEncode(oldEmail), HttpUtility.UrlEncode(newEmail));
            BaseResponse apiResponse = await _api.Post(_baseUri + "customer/emailchange/" + customerId, ContentType.Xml, requestBody);

            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<BaseResponse> Repermission(string email)
        {
            var emailToUse = HttpUtility.UrlEncode(email).Replace("+", "%2B");
            var apiResponse = await _api.Post(_baseUri + "customer/repermission/" + emailToUse + "/", ContentType.Xml, "");
            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<BaseResponse> AcceptPrivilege(Guid customerID)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/acceptprivilege/" + customerID, ContentType.Xml, "");
            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            return newResponse;
        }

        public async Task<BaseResponse> CreateCustomerExtendedProperties(CustomerPreference preference)
        {
            var apiResponse = await _api.Post(_baseUri + "customerpreference", ContentType.Xml, string.Format(@"<CustomerPreference CustomerId=""{0}"" Group=""{1}"" contractmode""{1}"" Key=""{2}"" Value=""{3}"" Value=""true"" />", preference.CustomerId, "contractmodes", preference.Key, preference.Value));
            var newResponse = new RegisterCustomerResponse();
            newResponse.Response = apiResponse.Response;
            newResponse.ResponseBody = apiResponse.ResponseBody;
            return newResponse;
        }

        public async IAsyncEnumerable<ApiEnum> GetCustomerExtendedProperties(Guid customerId)
        {
            var endpoint = $"{_baseUri}customerextendedproperties/{customerId}";
            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get customer extended properties. Status code: {StatusCode}", apiResponse.Response.StatusCode);
                yield break;
            }

            var enums = DecodeEnums(apiResponse);

            foreach (var item in enums)
            {
                yield return item;
                await Task.Yield(); // Allows other tasks to run
            }
        }

        public async Task<BaseResponse> DeleteCustomerExtendedProperties(Guid customerId, string property)
        {
            var apiResponse = await _api.Post(_baseUri + "customerpreference/" + property, ContentType.Xml, "");
            var newResponse = new RegisterCustomerResponse();
            newResponse.Response = apiResponse.Response;
            newResponse.ResponseBody = apiResponse.ResponseBody;
            return newResponse;
        }

        public async Task<IList<ApiEnum>> GetBrandPreferences(Guid customerId)
        {
            var apiResponse = await _api.Get(_baseUri + "CustomerBrandPreferences/" + customerId, ContentType.Xml);
            var enums = DecodeEnums(apiResponse);
            return enums;
        }

        public async Task<BaseResponse> CreateBrandPreferences(Guid customerId, string brandId)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/repermission/" + customerId, ContentType.Xml, "");
            var newResponse = new RegisterCustomerResponse();
            newResponse.Response = apiResponse.Response;
            newResponse.ResponseBody = apiResponse.ResponseBody;
            return newResponse;
        }

        public async Task<BaseResponse> DeleteBrandPreferences(Guid customerId, string brandId)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/repermission/" + customerId, ContentType.Xml, "");
            var newResponse = new RegisterCustomerResponse();
            newResponse.Response = apiResponse.Response;
            newResponse.ResponseBody = apiResponse.ResponseBody;
            return newResponse;
        }

        public async IAsyncEnumerable<ApiEnum> GetVillages()
        {
            var endpoint = $"{_baseUri}villages/profile";
            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get villages. Status code: {StatusCode}", apiResponse.Response.StatusCode);
                yield break;
            }

            XDocument xml;
            try
            {
                xml = XDocument.Parse(apiResponse.ResponseBody);
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "Failed to parse XML response for villages");
                yield break;
            }

            if (xml.Root == null)
            {
                _logger.LogWarning("Villages XML response has no root element");
                yield break;
            }

            var orderedElements = xml.Root.Descendants()
                .Select(i => new ApiEnum
                {
                    Key = i.Attribute("Key")?.Value ?? string.Empty,
                    Value = i.Value
                })
                .OrderBy(i => i.Value);

            foreach (var item in orderedElements)
            {
                yield return item;
                await Task.Yield(); // Allows other tasks to run
            }
        }


        public async IAsyncEnumerable<ApiEnum> GetLanguages(string locale)
        {
            var endpoint = $"{_baseUri}languages/{locale}";
            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get languages for locale {Locale}. Status code: {StatusCode}", locale, apiResponse.Response.StatusCode);
                yield break;
            }

            XDocument xml;
            try
            {
                xml = XDocument.Parse(apiResponse.ResponseBody);
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "Failed to parse XML response for languages. Locale: {Locale}", locale);
                yield break;
            }

            if (xml.Root == null)
            {
                _logger.LogWarning("Languages XML response has no root element. Locale: {Locale}", locale);
                yield break;
            }

            var orderedElements = xml.Root.Descendants()
                .Select(i => new ApiEnum
                {
                    Key = i.Attribute("Key")?.Value ?? string.Empty,
                    Value = i.Value
                })
                .OrderBy(i => i.Value);

            foreach (var item in orderedElements)
            {
                yield return item;
                await Task.Yield(); // Allows other tasks to run
            }
        }

        public async IAsyncEnumerable<ApiEnum> GetCountries(string locale)
        {
            var endpoint = $"{_baseUri}countries/{locale}";
            var apiResponse = await _api.Get(endpoint, ContentType.Xml);

            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get countries for locale {Locale}. Status code: {StatusCode}. Returning default list.", locale, apiResponse.Response.StatusCode);
                yield return new ApiEnum
                {
                    Key = "GB",
                    Value = "United Kingdom"
                };
                yield break;
            }

            await foreach (var country in GetEnumList(apiResponse.ResponseBody))
            {
                yield return country;
            }
        }

        public async Task<IList<ApiEnum>> GetBrands()
        {
            var apiResponse = await _api.Get(_baseUri + "Brands/en-gb", ContentType.Xml);
            var enums = DecodeEnums(apiResponse);
            return enums;
        }

        public async Task<TokenStatus> GetTokenStatus(string token)
        {
            var apiResponse = await _api.Get(_baseUri + "customer/ValidateCustomerResetToken/" + token, ContentType.Form);

            if (apiResponse.Response.StatusCode == HttpStatusCode.OK)
            {
                return TokenStatus.Valid;
            }
            else if (apiResponse.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return TokenStatus.NotFound;
            }
            else if (apiResponse.Response.StatusCode == HttpStatusCode.BadRequest)
            {
                return TokenStatus.InvalidFormat;
            }
            else if (apiResponse.Response.StatusCode == HttpStatusCode.Conflict)
            {
                return TokenStatus.Used;
            }
            else if (apiResponse.Response.StatusCode == HttpStatusCode.Gone)
            {
                return TokenStatus.Expired;
            }
            else
            {
                return TokenStatus.Undetermined;
            }
        }

        public async Task<IList<UserStates>> GetStates(Guid customerId)
        {
            var states = new List<UserStates>();
            var apiResponse = await _api.Get(_baseUri + "customer/GetStates/" + customerId, ContentType.Xml);

            if (apiResponse.Response.IsSuccessStatusCode)
            {
                var doc = new XmlDocument();
                doc.LoadXml(apiResponse.ResponseBody);
                var results = JsonConvert.DeserializeObject<List<States>>(doc.FirstChild.InnerText);
                foreach (var result in results)
                {
                    states.Add(result.State);
                }
            }

            return states;
        }

        public async Task<bool> UpdateMarketingPreferences(Guid customerId, bool emailMarketing, bool smsMarketing, bool phoneMarketing, bool postMarketing, bool locationMarketing)
        {
            // Reverse boolean values for EmailMarketing/SmsMarketing/PhoneMarketing/PostMarketing/locationMarketing so that it is in-line with the API
            var body =
            $@"<CustomerContactPreferences CustomerId=""{customerId}"" EmailOptOut=""{!emailMarketing}"" GeoOptOut=""{!locationMarketing}"" SmsOptOut=""{!smsMarketing}"" MobileOptOut=""{!phoneMarketing}"" PostOptOut=""{!postMarketing}"" />";

            var apiResponse = await _api.Post(_baseUri + "CustomerContactPreferences/", ContentType.Xml, body);
            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateTerms(Guid customerId)
        {
            var apiResponse = await _api.Post(_baseUri + "customer/SetCustomerViewTandC/" + customerId, ContentType.Xml, HttpUtility.UrlEncode(DateTime.Now.ToString()));
            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task<bool?> HasPassword(Guid customerId)
        {
            bool result = false;

            var apiResponse = await _api.Get(string.Format("{0}customer/{1}/HasPassword/", _baseUri, customerId), ContentType.Json);
            if (!apiResponse.Response.IsSuccessStatusCode || !bool.TryParse(apiResponse.ResponseBody, out result))
            {
                return null;
            }
            return result;
        }

        public IEnumerable<SelectListItem> GetDaysList()
        {
            var dayLabel = // TODO: Adapt to Contentful ->
                // ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("dayDateLabel") ?? 
                "DD";

            yield return new SelectListItem { Text = dayLabel, Value = "" };

            for (int i = 1; i <= 31; i++)
            {
                yield return new SelectListItem { Text = i.ToString(), Value = i.ToString() };
            }
        }


        public IEnumerable<SelectListItem> GetMonthsList()
        {
            var monthLabel = // TODO: Adapt to Contentful ->
                // ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("monthDateLabel") ?? 
                "MM";

            yield return new SelectListItem { Text = monthLabel, Value = "" };

            for (int i = 1; i <= 12; i++)
            {
                yield return new SelectListItem { Text = i.ToString(), Value = i.ToString() };
            }
        }

        public IEnumerable<SelectListItem> GetYearsList()
        {
            var yearLabel = // TODO: Adapt to Contentful ->
                // ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("yearDateLabel") ?? 
                "YY";
            yield return new SelectListItem { Text = yearLabel, Value = "" };

            int currentYear = DateTime.Now.Year;
            int startYear = currentYear - 103; // 120 - 17

            for (int year = currentYear - 17; year >= startYear; year--)
            {
                yield return new SelectListItem { Text = year.ToString(), Value = year.ToString() };
            }
        }


        public async Task<BaseResponse> AllocateTreatToUser(Guid customerId, string leadSource)
        {
            var info = JsonConvert.SerializeObject(new
            {
                Parameters = new
                {
                    CustomerId = customerId,
                    LeadSource = leadSource
                }
            });
            _logger.LogInformation($"[START] {nameof(CustomerService)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");

            var leadSourceEncoded = HttpUtility.UrlEncode(leadSource);

            var uri = $"{_baseUri}customer/{customerId}/allocate/{leadSourceEncoded}";
            info = JsonConvert.SerializeObject(new
            {
                Parameters = new
                {
                    CustomerId = customerId,
                    LeadSource = leadSource
                },
                Uri = uri
            });
            _logger.LogInformation($"[INFO] {nameof(CustomerService)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");

            var apiResponse = await _api.Put(uri, ContentType.Xml, string.Empty);
            var response = new BaseResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody,
                ResponseHeaders = apiResponse.ResponseHeaders
            };
            info = JsonConvert.SerializeObject(new
            {
                Parameters = new
                {
                    CustomerId = customerId,
                    LeadSource = leadSource
                },
                Result = response
            });
            _logger.LogInformation($"[END] {nameof(CustomerService)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");
            return response;
        }

        public async Task<IEnumerable<ApiEnum>> GetRegistrationVillages()
        {
            var apiResponse = await _api.Get(_baseUri + "villages/registration", ContentType.Xml);
            if (!apiResponse.Response.IsSuccessStatusCode)
            {
                return null;
            }

            var xml = XDocument.Parse(apiResponse.ResponseBody);
            if (xml.Root == null)
            {
                return null;
            }

            var list = xml.Root.Descendants().Select(i => new ApiEnum()
            {
                Key = i.Attribute("Key").Value,
                Value = i.Value + " - " + i.Attribute("Language").Value
            });

            return list.OrderBy(i => i.Value);
        }

        public async Task<Customer> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid id))
            {
                return null;
            }

            return await GetCustomer(id);
        }

        public bool IsCustomerSuspended(Customer customer)
        {
            var suspendedStatusList = _configuration["CustomerApi:SuspendedStatusList"]!.Split('|').ToList() ?? new List<string>();
            return suspendedStatusList.Contains(customer.MembershipActivationStatus!);
        }

        public async Task<bool> IsCurrentUserSuspended()
        {
            var customer = await GetCurrentUser();

            return IsCustomerSuspended(customer);
        }

        // TODO: Adapt to Contentful

        //public IList<IWidgetModel> GetTreats(IList<Models.Mulesoft.YourTreatsCategory> categories, int limit = 0,
        //    WidgetColumn column = WidgetColumn.Half, string language = null)
        //{
        //    var rootNode = ContentHelper.GetSiteRoot();
        //    var siteCode = rootNode.GetPropertyValue<string>("siteCode");

        //    var customer = GetCurrentUser();

        //    // Get Treats
        //    var rewardsInfo = language == null ? GetCustomerOffersV2(customer.Id) : GetCustomerOffersV2(customer.Id, language);

        //    var treatResultList = categories
        //        .Join(rewardsInfo.Offers, category => category.Id, offer => offer.CategoryId,
        //        (category, offer) => Mappers.Mulesoft.TreatsMapper.Map(offer, category, column)).OrderBy(x => x.CategoryName).ToList();

        //    List<IWidgetModel> orderedList = treatResultList.OrderBy(x => x.CategoryName).Cast<IWidgetModel>().ToList();

        //    if (limit > 0)
        //        orderedList = orderedList.Take(limit).ToList();

        //    return orderedList;
        //}

        //public IList<Models.Mulesoft.YourTreatsCategory> GetCategories(string language = null)
        //{
        //    var rootNode = ContentHelper.GetSiteRoot();
        //    var siteCode = rootNode.GetPropertyValue<string>("siteCode");

        //    var customer = GetCurrentUser();

        //    var customerTreatCategories = language == null ? GetTreatCategoriesV2(customer.Id, siteCode) : GetTreatCategoriesV2(customer.Id, siteCode, language);

        //    return customerTreatCategories?.OrderBy(x => x.Order)
        //        .Select(x => new Models.Mulesoft.YourTreatsCategory(x.Id, x.Name, x.IsToHide))
        //        .ToList();
        //}
        private async Task<XDocument> GetCustomerXml(Guid customerId)
        {
            var apiResponse = await _api.Get(_baseUri + "customer/" + customerId, ContentType.Xml);
            var xml = XDocument.Parse(apiResponse.ResponseBody);

            return xml;
        }

        private async IAsyncEnumerable<ApiEnum> GetEnumList(string xmlContent)
        {
            XDocument xml;
            try
            {
                xml = XDocument.Parse(xmlContent);
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "Failed to parse XML response");
                yield break;
            }

            if (xml.Root == null)
            {
                _logger.LogWarning("XML response has no root element");
                yield break;
            }

            var orderedElements = xml.Root.Descendants()
                .Select(i => new ApiEnum
                {
                    Key = i.Attribute("Key")?.Value ?? string.Empty,
                    Value = i.Value
                })
                .OrderBy(i => i.Value);

            foreach (var item in orderedElements)
            {
                yield return item;
                await Task.Yield(); // Allows other tasks to run
            }
        }

        private RegisterCustomerResponse FormatResponse(BaseResponse apiResponse, Customer customer)
        {
            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                ResponseBody = apiResponse.ResponseBody
            };
            if (newResponse.ResponseBody == null) return newResponse;

            var createdCustomer = JsonConvert.DeserializeObject<Models.Common.Customer>(newResponse.ResponseBody);
            customer.Id = createdCustomer.Id;
            newResponse.CreatedCustomer = customer;

            return newResponse;
        }

        private RegisterCustomerResponse FormatConfirmEmailResponse(BaseResponse apiResponse)
        {
            var newResponse = new RegisterCustomerResponse
            {
                Response = apiResponse.Response,
                CustomerId = Guid.Empty
            };

            // map the customerID in the Response body
            if (apiResponse.Response.IsSuccessStatusCode && apiResponse.ResponseBody != null)
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(apiResponse.ResponseBody);
                var xElement = XElement.Parse(xml.OuterXml);
                if (xElement != null)
                {
                    if (Guid.TryParse(xElement.Value, out Guid customerID))
                    {
                        newResponse.CustomerId = customerID;
                    }
                };
            };

            return newResponse;
        }

        private string GetXmlAttribute(XElement xmlNode, string attribute)
        {
            if (xmlNode.Attribute(attribute) == null)
            {
                return null;
            }
            return xmlNode.Attribute(attribute).Value;
        }

        private bool IsRepermission(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return securityToken.Claims.Any(i => i.Type == "role" && i.Value == "Repermission");
        }

        private bool IsPrivilege(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return securityToken.Claims.Any(i => i.Type == "role" && i.Value == "PendingPrivilege");
        }

        private string GetPreferenceLabel(string prefKey)
        {
            // TODO: Adapt to Contentful
            //var root = ContentHelper.GetSiteLanguageRoot();
            switch (prefKey)
            {
                case "email":
                //return root.GetPropertyValue<string>("emailPreferenceLabel");

                case "post":
                //return root.GetPropertyValue<string>("postOptInLabel");

                case "sms":
                //return root.GetPropertyValue<string>("smsPreferenceLabel");

                default:
                    return "";
            }
        }

        private IList<ApiEnum> DecodeEnums(BaseResponse apiResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(apiResponse.ResponseBody); // supposse that myXmlString contains "<Names>...</Names>"
            var xEmelent = XElement.Parse(xml.OuterXml);

            // Convert the XML document in to a dynamic C# object.
            List<string> listNodes = new List<string>() { "Item" };
            dynamic xmlContent = new ExpandoObject();

            ExpandoObjectHelper.Parse(xmlContent, xEmelent, listNodes);

            var byName = ((ExpandoObject)xmlContent.Enumeration).FirstOrDefault(x => x.Key == "Item").Value;
            var list = (List<object>)byName;

            IList<ApiEnum> newList = new List<ApiEnum>();

            foreach (var item in list)
            {
                var x = new ApiEnum() { Key = (string)item, Value = (string)item };
                newList.Add(x);
            }

            return newList;
        }
    }
}
