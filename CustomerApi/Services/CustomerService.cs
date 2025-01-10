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

namespace MenulioPocMvc.CustomerApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;

        private readonly IConfiguration _configuration;
        private readonly IApiCalls _api;
        private readonly string _baseUri;

        public CustomerService(ILogger<CustomerService> logger, IConfiguration configuration, IApiCalls apiClient)
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
        }

        private bool IsUrlValid(string url)
        {
            Uri uriResult;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult!)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public async Task<RegisterCustomerResponse> RegisterCustomer(Customer customer)
        {
            var body = JsonConvert.SerializeObject(CustomerMapper.MapRegistrationToJson(customer));
            string uri = _baseUri + "customer";
            var apiResponse = await _api.Post(uri, ContentType.Json, body);
            
            return FormatResponse(apiResponse, customer);
        }

        //public UpdateProfileResponse UpdateProfile(Customer customer)
        //{
        //    // call the API to get the original Customer XML
        //    var originalXml = GetCustomerXml(customer.Id);

        //    // pass in the original XML to the mapper
        //    // need to remove XML properties if they are not set
        //    var updatedXml = CustomerMapper.MapToXml(customer, originalXml);

        //    BaseResponse apiResponse = _api.Put(_baseUri + "customer", ContentType.XML, updatedXml.ToString());

        //    var newResponse = new UpdateProfileResponse
        //    {
        //        Response = apiResponse.Response,
        //        ResponseBody = apiResponse.ResponseBody
        //    };
        //    return newResponse;
        //}

        //public IEnumerable<CustomerPreference> GetCustomerPreferences(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "customerpreferences/" + customerId, ContentType.XML);

        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);
        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var prefs = xml.Root.Descendants().Select(i => new CustomerPreference()
        //    {
        //        CustomerId = customerId,
        //        Group = i.Attribute("Group").Value,
        //        Key = i.Attribute("Key").Value,
        //        Value = bool.Parse(i.Attribute("Value").Value),
        //        Label = GetPreferenceLabel(i.Attribute("Key").Value)
        //    });

        //    return prefs;
        //}

        //private string GetPreferenceLabel(string prefKey)
        //{
        //    var root = ContentHelper.GetSiteLanguageRoot();
        //    switch (prefKey)
        //    {
        //        case "email":
        //            return root.GetPropertyValue<string>("emailPreferenceLabel");

        //        case "post":
        //            return root.GetPropertyValue<string>("postOptInLabel");

        //        case "sms":
        //            return root.GetPropertyValue<string>("smsPreferenceLabel");

        //        default:
        //            return "";
        //    }
        //}

        ///// <summary>
        ///// Deletes all old preferences passed in as a parameter. Then creates the new preferences.
        ///// Best use is to pass in all old and new preferences belonging to a specific group.
        ///// </summary>
        ///// <returns></returns>
        //public void CreateCustomerPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        //{
        //    if (oldPreferences != null && newPreferences != null)
        //    {
        //        // remove null keys
        //        newPreferences = newPreferences.Where(p => p.Key != null);

        //        // only delete old pref if they are not in new ones
        //        var resultsToDelete = oldPreferences.Where(p => !newPreferences.Any(p2 => p2.Key == p.Key));

        //        foreach (var pref in resultsToDelete)
        //        {
        //            var idString = string.Format("{0}|{1}|{2}", customerId, pref.Group, pref.Key);
        //            var apiResponse = _api.Delete(_baseUri + "customerpreference/" + idString,
        //                ContentType.XML, "");
        //        }

        //        // only add new preferences if thet are not in the old ones
        //        var resultsToAdd = newPreferences.Where(p => !oldPreferences.Any(p2 => p2.Key == p.Key));

        //        foreach (var pref in resultsToAdd)
        //        {
        //            var body = string.Format(@"<CustomerPreference CustomerId=""{0}"" Group=""{1}"" Key=""{2}"" Value=""{3}"" />",
        //                    customerId, pref.Group, pref.Key, pref.Value);

        //            var apiResponse = _api.Post(_baseUri + "customerpreference", ContentType.XML, body);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Deletes all old preferences passed in as a parameter. Then creates the new preferences.
        ///// Best use is to pass in all old and new preferences belonging to a specific group.
        ///// </summary>
        ///// <returns></returns>
        //public void UpdatePreferencesMulesoft(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        //{
        //    var payload = new CustomerPreferencesUpdate
        //    {
        //        OldPreferences = oldPreferences,
        //        NewPreferences = newPreferences
        //    };

        //    var apiResponse = _api.Post(_baseUri + "customerPreferences/" + customerId, ContentType.JSON, JsonConvert.SerializeObject(payload));
        //}

        //public void CreateCustomerContactPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences)
        //{
        //    // Delete old preferences
        //    if (oldPreferences != null)
        //    {
        //        foreach (var pref in oldPreferences)
        //        {
        //            var idString = string.Format("{0}|{1}|{2}", customerId, pref.Group, pref.Key);
        //            var apiResponse = _api.Delete(_baseUri + "customerpreference/" + idString,
        //                ContentType.XML, "");
        //        }
        //    }

        //    // Now add new preferences
        //    if (newPreferences != null)
        //    {
        //        foreach (var pref in newPreferences)
        //        {
        //            var body = string.Format(@"<item type=""OPTOUTEMAIL"">{0}</item>", pref.Value);

        //            var apiResponse = _api.Post(_baseUri + "customerpreference", ContentType.XML, body);
        //        }
        //    }
        //}

        // Post
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

        // Post
        //public bool SocialSignIn(ReferenceType provider, string socialId)
        //{
        //    if (string.IsNullOrEmpty(socialId))
        //        return false;

        //    var signInBody = string.Format(@"client_id=4e8ce86a9c914dd8a41c09e0709ef297&grant_type=password&socialId={0}&ReferenceType={1}", HttpUtility.UrlEncode(socialId), Enum.GetName(typeof(ReferenceType), provider));

        //    var apiResponse = _api.Post(_baseUri + "socialSignin", ContentType.XML, signInBody);

        //    return apiResponse.Response.IsSuccessStatusCode;
        //}

        // Get
        public async Task<Customer> GetCustomer(Guid customerId)
        {
            var uri = _baseUri + "customer/" + customerId;
            BaseResponse apiResponse = await _api.Get(uri, ContentType.Xml);
            
            var xml = XDocument.Parse(apiResponse.ResponseBody);

            return CustomerMapper.MapToCustomer(xml);
        }

        //public CustomerExtended GetCustomerMembershipInfo(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "customerpoints/" + customerId, ContentType.JSON);
        //    CustomerExtended customerPointsPrivilege = JsonConvert.DeserializeObject<CustomerExtended>(apiResponse.ResponseBody);

        //    return customerPointsPrivilege;
        //}

        //public GuestInfo GetCustomerMembershipInfoV2(Guid customerId)
        //{
        //    GuestInfo customerPointsPrivilege = new GuestInfo();
        //    var apiResponse = _api.Get(_baseUri + "customerpoints/" + customerId, ContentType.JSON);

        //    if (apiResponse.Response.IsSuccessStatusCode)
        //        customerPointsPrivilege = JsonConvert.DeserializeObject<GuestInfo>(apiResponse.ResponseBody);
        //    else
        //        Log.Error($"Error retrieving customer points. {apiResponse.Response.StatusCode}");

        //    return customerPointsPrivilege;
        //}

        //public SpendTier GetCustomerSpendTierInfo(Guid tierId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "segment/" + tierId, ContentType.JSON);
        //    SpendTier spendTier = JsonConvert.DeserializeObject<SpendTier>(apiResponse.ResponseBody);

        //    return spendTier;
        //}

        //public SegmentInfo GetCustomerSpendTierInfoV2(string tierId)
        //{
        //    SegmentInfo spendTier = new SegmentInfo();
        //    var language = ContentHelper.GetSiteLanguageRoot();
        //    var url = $"{_baseUri}/segment/{tierId}?language={language.UrlName}";
        //    var apiResponse = _api.Get(url, ContentType.JSON);

        //    if (apiResponse.Response.IsSuccessStatusCode)
        //        spendTier = JsonConvert.DeserializeObject<SegmentInfo>(apiResponse.ResponseBody);
        //    else
        //        Log.Error($"Error retrieving customer segment. {apiResponse.Response.StatusCode}");

        //    return spendTier;
        //}

        //// Get
        //public Customer GetCustomer(ReferenceType refType, string refValue)
        //{
        //    var valueToUse = HttpUtility.UrlEncode(refValue);
        //    var apiResponse = _api.Get(_baseUri + string.Format("customer?reftype={0}&refvalue={1}", Enum.GetName(typeof(ReferenceType), refType), valueToUse), ContentType.XML);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        Log.Error("Customer not found by API");
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);
        //    return CustomerMapper.MapToCustomer(xml);
        //}

        //public RewardsInfo GetCustomerOffersV2(Guid customerId, string language = null)
        //{
        //    var rootNode = ContentHelper.GetSiteRoot();
        //    var siteCode = rootNode.GetPropertyValue<string>("siteCode");

        //    var endpoint = $"{_baseUri}customeroffers/{customerId}/{siteCode}";

        //    if (!string.IsNullOrWhiteSpace(language))
        //    {
        //        var languageClean = language.Trim().Replace(" ", "").ToLower();
        //        endpoint += $"?language={languageClean}";
        //    }

        //    var apiResponse = _api.Get(endpoint, ContentType.JSON);
        //    RewardsInfo offers = new RewardsInfo
        //    {
        //        Offers = new List<Models.Mulesoft.CustomerOffer>()
        //    };

        //    if (apiResponse.Response.IsSuccessStatusCode)
        //        offers = JsonConvert.DeserializeObject<RewardsInfo>(apiResponse.ResponseBody);
        //    else
        //        Log.Error($"Error retrieving customer offers: {apiResponse.Response.StatusCode}");

        //    return offers;
        //}

        //// Get
        //public IEnumerable<ICustomerOffer> GetCustomerOffers(Guid customerId, string language = null)
        //{
        //    var xml = language == null ?
        //        this.GetCustomerOffersXml(customerId) : this.GetCustomerOffersXml(customerId, language);

        //    if (xml.Root == null)
        //    {
        //        yield break;
        //    }

        //    var offers = xml.Root.Descendants();

        //    foreach (var xmlOffer in offers)
        //    {
        //        Guid categoryId;
        //        if (!Guid.TryParse(GetXmlAttribute(xmlOffer, "CategoryId"), out categoryId))
        //        {
        //            categoryId = Guid.Empty;
        //        }

        //        var offer = new Models.Apis.CustomerOffer()
        //        {
        //            BarcodeUrl = GetXmlAttribute(xmlOffer, "BarcodeUrl"),
        //            BarcodeNumber = GetXmlAttribute(xmlOffer, "BarcodeNumber"),
        //            Conditions = GetXmlAttribute(xmlOffer, "Conditions"),
        //            Description = GetXmlAttribute(xmlOffer, "Description"),
        //            IsActive = bool.Parse(GetXmlAttribute(xmlOffer, "IsActive")),
        //            OfferId = GetXmlAttribute(xmlOffer, "OfferId"),
        //            Redeemed = bool.Parse(GetXmlAttribute(xmlOffer, "Redeemed")),
        //            Title = GetXmlAttribute(xmlOffer, "Title"),
        //            Title2 = GetXmlAttribute(xmlOffer, "Title2"),
        //            PassbookTitle = GetXmlAttribute(xmlOffer, "PassbookTitle"),
        //            ImageUrl = GetXmlAttribute(xmlOffer, "ImageURL"),
        //            VillageID = GetXmlAttribute(xmlOffer, "VillageId"),
        //            ConditionsUrl = GetXmlAttribute(xmlOffer, "ConditionsUrl"),
        //            CategoryId = categoryId
        //        };

        //        yield return offer;
        //    }
        //}

        //// Get
        //public XDocument GetCustomerOffersXml(Guid customerId, string language = null)
        //{
        //    var rootNode = ContentHelper.GetSiteRoot();
        //    var siteCode = rootNode.GetPropertyValue<string>("siteCode");

        //    var endpoint = _baseUri + "customeroffers/" + customerId + "/" + siteCode;

        //    if (!string.IsNullOrWhiteSpace(language))
        //    {
        //        var languageClean = language.Trim().Replace(" ", "").ToLower();
        //        endpoint += $"?language={languageClean}";
        //    }

        //    var apiResponse = _api.Get(endpoint, ContentType.XML);

        //    var xml = new XDocument();

        //    if (apiResponse.ResponseBody != null)
        //    {
        //        xml = XDocument.Parse(apiResponse.ResponseBody);
        //    }

        //    return xml;
        //}

        //public IList<Models.Apis.CustomerTreatCategory> GetTreatCategories(Guid customerId, string villageCode, string language = null)
        //{


        //    var endpoint = $"{_baseUri}customer/{customerId}/treatcategories?villageCode={villageCode}";

        //    if (!string.IsNullOrWhiteSpace(language))
        //    {
        //        var languageClean = language.Trim().Replace(" ", "").ToLower();
        //        endpoint += $"&language={languageClean}";
        //    }

        //    var response = _api.Get(endpoint, ContentType.JSON);
        //    var result = new List<Models.Apis.CustomerTreatCategory>();

        //    if (!string.IsNullOrWhiteSpace(response.ResponseBody))
        //    {
        //        result = JsonConvert.DeserializeObject<List<Models.Apis.CustomerTreatCategory>>(response.ResponseBody);
        //    }

        //    return result;
        //}

        //public IList<Models.Mulesoft.CustomerTreatCategory> GetTreatCategoriesV2(Guid customerId, string villageCode, string language = null)
        //{
        //    var endpoint = $"{_baseUri}customer/{customerId}/treatcategories?village={villageCode}";

        //    if (!string.IsNullOrWhiteSpace(language))
        //    {
        //        var languageClean = language.Trim().Replace(" ", "").ToLower();
        //        endpoint += $"&language={languageClean}";
        //    }

        //    var apiResponse = _api.Get(endpoint, ContentType.JSON);
        //    var result = new List<Models.Mulesoft.CustomerTreatCategory>();

        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //        Log.Error($"Error retrieving customer treat categories. {apiResponse.Response.StatusCode}");
        //    else if (!string.IsNullOrWhiteSpace(apiResponse.ResponseBody))
        //        result = JsonConvert.DeserializeObject<List<Models.Mulesoft.CustomerTreatCategory>>(apiResponse.ResponseBody);

        //    return result;
        //}

        //public IEnumerable<ICustomerOffer> GetVillageTeasers(string villageCode, string languageId)
        //{
        //    var villageTeaserList = new List<ICustomerOffer>();

        //    var response = _api.Get($"{_baseUri}Teasers/{villageCode}/{languageId}", ContentType.JSON);

        //    if (response.Response.IsSuccessStatusCode)
        //    {
        //        var villageTeaser = JsonConvert.DeserializeObject<VillageTeaser>(response.ResponseBody);
        //        var customerOffer = TreatsMapper.Map(villageTeaser);
        //        villageTeaserList.Add(customerOffer);
        //    }

        //    return villageTeaserList;
        //}

        //// Get - Customer's Selected Brands
        //public IEnumerable<CustomerBrandPreference> GetCustomerBrandPreferences(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "CustomerBrandPreferences/" + customerId, ContentType.XML);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        yield return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);

        //    if (xml.Root == null)
        //    {
        //        yield break;
        //    }

        //    var brandPreferences = xml.Root.Descendants();

        //    foreach (var xmlBrandPreference in brandPreferences)
        //    {
        //        var brandPreference = new CustomerBrandPreference
        //        {
        //            BrandId = GetXmlAttribute(xmlBrandPreference, "BrandId")
        //        };

        //        yield return brandPreference;
        //    }
        //}

        //// Get - Village Brands
        //public IEnumerable<ApiEnum> GetVillageBrands(string villageId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "villagebrands/" + villageId.ToLower(), ContentType.XML);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);

        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var list = xml.Root.Descendants().Select(i => new ApiEnum
        //    {
        //        Key = i.Attribute("Key").Value,
        //        Value = i.Value
        //    });

        //    return list;
        //}

        //// Post
        //public bool CreateCustomerBrandPreferences(Guid customerId, string brandId)
        //{
        //    if (string.IsNullOrEmpty(brandId))
        //    {
        //        return false;
        //    }

        //    var apiResponse = _api.Post(_baseUri + "customerbrandpreference", ContentType.XML, BrandPreferenceCreateXml(customerId, brandId));
        //    return apiResponse.Response.IsSuccessStatusCode;
        //}

        //// Delete
        //public bool DeleteCustomerBrandPreferences(Guid customerId, string brandId)
        //{
        //    if (string.IsNullOrEmpty(brandId))
        //    {
        //        return false;
        //    }

        //    var apiResponse = _api.Delete(string.Format(_baseUri + "customerbrandpreference/{0}|{1}", customerId, brandId), ContentType.XML, "");
        //    return apiResponse.Response.IsSuccessStatusCode;
        //}

        //// Post

        //public ForgottenPasswordResponse ResetPasswordRequest(string emailAddress)

        //{
        //    BaseResponse apiResponse = _api.Post(_baseUri + "customer/passwordresetrequest/" + HttpUtility.UrlEncode(emailAddress) + "/", ContentType.XML, "");
        //    var newResponse = new ForgottenPasswordResponse
        //    {
        //        Response = apiResponse.Response
        //    };
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse InitialisePassword(Customer customer)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/passwordinitialise/" + customer.Id, ContentType.FORM, "password=" + HttpUtility.UrlEncode(customer.Password));
        //    var newResponse = new RegisterCustomerResponse
        //    {
        //        Response = apiResponse.Response,
        //        ResponseBody = apiResponse.ResponseBody
        //    };
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse ResetPassword(string token, string newPassword)
        //{
        //    BaseResponse apiResponse = _api.Post(_baseUri + "customer/passwordreset/" + token, ContentType.FORM, "password=" + HttpUtility.UrlEncode(newPassword));

        //    var newResponse = new RegisterCustomerResponse
        //    {
        //        Response = apiResponse.Response,
        //        ResponseBody = apiResponse.ResponseBody
        //    };
        //    return newResponse;
        //}

        //// Post
        //public bool ChangePassword(Guid customerId, string newPassword)
        //{
        //    BaseResponse apiResponse = _api.Post(_baseUri + "customer/passwordchange/" + customerId, ContentType.FORM, "password=" + HttpUtility.UrlEncode(newPassword));

        //    if (apiResponse.Response.StatusCode == HttpStatusCode.NoContent)
        //    {
        //        return false;
        //    }

        //    if (apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //public RegisterCustomerResponse ConfirmEmail(string token)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/ConfirmEmail/" + token, ContentType.XML, "");
        //    return FormatConfirmEmailResponse(apiResponse);
        //}

        //private RegisterCustomerResponse FormatConfirmEmailResponse(BaseResponse apiResponse)
        //{
        //    var newResponse = new RegisterCustomerResponse
        //    {
        //        Response = apiResponse.Response,
        //        CustomerId = Guid.Empty
        //    };

        //    // map the customerID in the Response body
        //    if (apiResponse.Response.IsSuccessStatusCode && apiResponse.ResponseBody != null)
        //    {
        //        XmlDocument xml = new XmlDocument();
        //        xml.LoadXml(apiResponse.ResponseBody);
        //        var xElement = XElement.Parse(xml.OuterXml);
        //        if (xElement != null)
        //        {
        //            if (Guid.TryParse(xElement.Value, out Guid customerID))
        //            {
        //                newResponse.CustomerId = customerID;
        //            }
        //        };
        //    };

        //    return newResponse;
        //}

        //// Post
        //public BaseResponse ResetEmail(Guid customerId, string oldEmail, string newEmail)
        //{
        //    var requestBody = string.Format("oldEmail={0}&newEmail={1}", HttpUtility.UrlEncode(oldEmail), HttpUtility.UrlEncode(newEmail));
        //    BaseResponse apiResponse = _api.Post(_baseUri + "customer/emailchange/" + customerId, ContentType.FORM, requestBody);

        //    var newResponse = new RegisterCustomerResponse
        //    {
        //        Response = apiResponse.Response,
        //        ResponseBody = apiResponse.ResponseBody
        //    };
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse Repermission(string email)
        //{
        //    var emailToUse = HttpUtility.UrlEncode(email).Replace("+", "%2B");
        //    var apiResponse = _api.Post(_baseUri + "customer/repermission/" + emailToUse + "/", ContentType.XML, "");
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse AcceptPrivilege(Guid customerID)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/acceptprivilege/" + customerID, ContentType.XML, "");
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse CreateCustomerExtendedProperties(CustomerPreference preference)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customerpreference", ContentType.XML, string.Format(@"<CustomerPreference CustomerId=""{0}"" Group=""{1}"" contractmode""{1}"" Key=""{2}"" Value=""{3}"" Value=""true"" />", preference.CustomerId, "contractmodes", preference.Key, preference.Value));
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Get
        //public IEnumerable<string> GetCustomerExtendedProperties(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "customerextendedproperties/" + customerId, ContentType.XML);
        //    var enums = decodeEnums(apiResponse);
        //    return (IEnumerable<string>)enums;
        //}

        //// Delete
        //public BaseResponse DeleteCustomerExtendedProperties(Guid customerId, string property)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customerpreference/" + property, ContentType.XML, "");
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Get
        //public IList<ApiEnum> GetBrandPreferences(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "CustomerBrandPreferences/" + customerId, ContentType.XML);
        //    var enums = decodeEnums(apiResponse);
        //    return enums;
        //}

        //// Post
        //public BaseResponse CreateBrandPreferences(Guid customerId, string brandId)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/repermission/" + customerId, ContentType.XML, "");
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Post
        //public BaseResponse DeleteBrandPreferences(Guid customerId, string brandId)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/repermission/" + customerId, ContentType.XML, "");
        //    var newResponse = new RegisterCustomerResponse();
        //    newResponse.Response = apiResponse.Response;
        //    newResponse.ResponseBody = apiResponse.ResponseBody;
        //    return newResponse;
        //}

        //// Get
        //public IEnumerable<ApiEnum> GetVillages()
        //{
        //    var apiResponse = _api.Get(_baseUri + "villages/profile", ContentType.XML);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);
        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var list = xml.Root.Descendants().Select(i => new ApiEnum()
        //    {
        //        Key = i.Attribute("Key").Value,
        //        Value = i.Value
        //    });

        //    return list.OrderBy(i => i.Value);
        //}

        //// Get
        //public IEnumerable<ApiEnum> GetLanguages(string locale)
        //{
        //    var apiResponse = _api.Get(_baseUri + "languages/" + locale, ContentType.XML);

        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);
        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var list = xml.Root.Descendants().Select(i => new ApiEnum()
        //    {
        //        Key = i.Attribute("Key").Value,
        //        Value = i.Value
        //    });

        //    return list.OrderBy(i => i.Value);
        //}

        //// Get
        //public IEnumerable<ApiEnum> GetGenders(string locale)
        //{
        //    var apiResponse = _api.Get(_baseUri + "genders/" + locale, ContentType.XML);

        //    return GetEnumList(apiResponse.ResponseBody);
        //}

        //// Get
        //public IEnumerable<ApiEnum> GetCountries(string locale)
        //{
        //    var apiResponse = _api.Get(_baseUri + "countries/" + locale, ContentType.XML);

        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        // Return default list to avoid Website crash
        //        return new List<ApiEnum>
        //        {
        //            new ApiEnum
        //            {
        //                Key = "UK",
        //                Value = "United Kingdom"
        //            }
        //        };
        //    }

        //    return GetEnumList(apiResponse.ResponseBody);
        //}

        //private IEnumerable<ApiEnum> GetEnumList(string body)
        //{
        //    var xml = XDocument.Parse(body);
        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var list = xml.Root.Descendants().Select(i => new ApiEnum()
        //    {
        //        Key = i.Attribute("Key").Value,
        //        Value = i.Value
        //    });

        //    return list.OrderBy(i => i.Value);
        //}

        //// Get
        //public IList<ApiEnum> GetBrands()
        //{
        //    var apiResponse = _api.Get(_baseUri + "Brands/en-gb", ContentType.XML);
        //    var enums = decodeEnums(apiResponse);
        //    return enums;
        //}

        //public Customer GetLoggedInCustomer()
        //{
        //    return GetCustomer(Guid.NewGuid());
        //}

        //private string BrandPreferenceCreateXml(Guid customerId, string brandId)
        //{
        //    return string.Format(@"<CustomerBrandPreference CustomerId=""{0}"" BrandId=""{1}"" />", customerId, brandId);
        //}

        //private IList<ApiEnum> decodeEnums(BaseResponse apiResponse)
        //{
        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(apiResponse.ResponseBody); // supposse that myXmlString contains "<Names>...</Names>"
        //    var xEmelent = XElement.Parse(xml.OuterXml);

        //    // Convert the XML document in to a dynamic C# object.
        //    List<string> listNodes = new List<string>() { "Item" };
        //    dynamic xmlContent = new ExpandoObject();

        //    ExpandoObjectHelper.Parse(xmlContent, xEmelent, listNodes);

        //    var byName = ((ExpandoObject)xmlContent.Enumeration).FirstOrDefault(x => x.Key == "Item").Value;
        //    var list = (List<object>)byName;

        //    IList<ApiEnum> newList = new List<ApiEnum>();

        //    foreach (var item in list)
        //    {
        //        var x = new ApiEnum() { Key = (string)item, Value = (string)item };
        //        newList.Add(x);
        //    }

        //    return newList;
        //}

        

        //private string GetXmlAttribute(XElement xmlNode, string attribute)
        //{
        //    if (xmlNode.Attribute(attribute) == null)
        //    {
        //        return null;
        //    }
        //    return xmlNode.Attribute(attribute).Value;
        //}

        //public XDocument GetCustomerXml(Guid customerId)
        //{
        //    var apiResponse = _api.Get(_baseUri + "customer/" + customerId, ContentType.XML);
        //    var xml = XDocument.Parse(apiResponse.ResponseBody);

        //    return xml;
        //}

        //public TokenStatus GetTokenStatus(string token)
        //{
        //    var apiResponse = _api.Get(_baseUri + "customer/ValidateCustomerResetToken/" + token, ContentType.FORM);

        //    if (apiResponse.Response.StatusCode == HttpStatusCode.OK)
        //    {
        //        return TokenStatus.Valid;
        //    }
        //    else if (apiResponse.Response.StatusCode == HttpStatusCode.NotFound)
        //    {
        //        return TokenStatus.NotFound;
        //    }
        //    else if (apiResponse.Response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        return TokenStatus.InvalidFormat;
        //    }
        //    else if (apiResponse.Response.StatusCode == HttpStatusCode.Conflict)
        //    {
        //        return TokenStatus.Used;
        //    }
        //    else if (apiResponse.Response.StatusCode == HttpStatusCode.Gone)
        //    {
        //        return TokenStatus.Expired;
        //    }
        //    else
        //    {
        //        return TokenStatus.Undetermined;
        //    }
        //}

        //public IList<UserStates> GetStates(Guid customerId)
        //{
        //    var states = new List<UserStates>();
        //    var apiResponse = _api.Get(_baseUri + "customer/GetStates/" + customerId, ContentType.XML);

        //    if (apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        var doc = new XmlDocument();
        //        doc.LoadXml(apiResponse.ResponseBody);
        //        var results = JsonConvert.DeserializeObject<List<States>>(doc.FirstChild.InnerText);
        //        foreach (var result in results)
        //        {
        //            states.Add(result.State);
        //        }
        //    }

        //    return states;
        //}

        //// Post
        //public bool UpdateMarketingPreferences(Guid customerId, bool emailMarketing, bool smsMarketing, bool phoneMarketing, bool postMarketing, bool locationMarketing)
        //{
        //    // Reverse boolean values for EmailMarketing/SmsMarketing/PhoneMarketing/PostMarketing/locationMarketing so that it is in-line with the API
        //    var body =
        //    $@"<CustomerContactPreferences CustomerId=""{customerId}"" EmailOptOut=""{!emailMarketing}"" GeoOptOut=""{!locationMarketing}"" SmsOptOut=""{!smsMarketing}"" MobileOptOut=""{!phoneMarketing}"" PostOptOut=""{!postMarketing}"" />";

        //    var apiResponse = _api.Post(_baseUri + "CustomerContactPreferences/", ContentType.XML, body);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //// Post
        //public bool UpdateTerms(Guid customerId)
        //{
        //    var apiResponse = _api.Post(_baseUri + "customer/SetCustomerViewTandC/" + customerId, ContentType.XML, HttpUtility.UrlEncode(DateTime.Now.ToString()));
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public bool? HasPassword(Guid customerId)
        //{
        //    bool result = false;

        //    var apiResponse = _api.Get(string.Format("{0}customer/{1}/HasPassword/", _baseUri, customerId), ContentType.JSON);
        //    if (!apiResponse.Response.IsSuccessStatusCode || !bool.TryParse(apiResponse.ResponseBody, out result))
        //    {
        //        return null;
        //    }
        //    return result;
        //}

        //public IEnumerable<SelectListItem> GetDaysList()
        //{
        //    var dayLabel = ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("dayDateLabel");
        //    var days = Enumerable.Range(1, 31).Select(i => new SelectListItem() { Text = i.ToString(), Value = i.ToString() }).ToList();
        //    days.Insert(0, new SelectListItem() { Text = dayLabel ?? "DD", Value = "" });
        //    return days;
        //}

        //public IEnumerable<SelectListItem> GetMonthsList()
        //{
        //    var monthLabel = ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("monthDateLabel");
        //    var months = Enumerable.Range(1, 12).Select(i => new SelectListItem() { Text = i.ToString(), Value = i.ToString() }).ToList();
        //    months.Insert(0, new SelectListItem() { Text = monthLabel ?? "MM", Value = "" });
        //    return months;
        //}

        //public IEnumerable<SelectListItem> GetYearsList()
        //{
        //    int count = 120;
        //    int initialYear = DateTime.Now.Year - count;
        //    int quantity = count - 17;
        //    var years = Enumerable.Range(initialYear, quantity).Select(i => new SelectListItem() { Text = i.ToString(), Value = i.ToString() }).ToList();
        //    years.Reverse();
        //    var yearLabel = ContentHelper.GetSiteLanguageRoot().GetPropertyValue<string>("yearDateLabel");
        //    years.Insert(0, new SelectListItem() { Text = yearLabel ?? "YY", Value = "" });
        //    return years;
        //}

        //public BaseResponse AllocateTreatToUser(Guid customerId, string leadSource)
        //{
        //    var info = JsonConvert.SerializeObject(new
        //    {
        //        Parameters = new
        //        {
        //            CustomerId = customerId,
        //            LeadSource = leadSource
        //        }
        //    });
        //    Log.Info($"[START] {nameof(CustomerServices)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");

        //    var leadSourceEncoded = HttpUtility.UrlEncode(leadSource);

        //    var uri = $"{_baseUri}customer/{customerId}/allocate/{leadSourceEncoded}";
        //    info = JsonConvert.SerializeObject(new
        //    {
        //        Parameters = new
        //        {
        //            CustomerId = customerId,
        //            LeadSource = leadSource
        //        },
        //        Uri = uri
        //    });
        //    Log.Info($"[INFO] {nameof(CustomerServices)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");

        //    var apiResponse = _api.Put(uri, ContentType.XML, string.Empty);
        //    var response = new BaseResponse
        //    {
        //        Response = apiResponse.Response,
        //        ResponseBody = apiResponse.ResponseBody,
        //        ResponseHeaders = apiResponse.ResponseHeaders
        //    };
        //    info = JsonConvert.SerializeObject(new
        //    {
        //        Parameters = new
        //        {
        //            CustomerId = customerId,
        //            LeadSource = leadSource
        //        },
        //        Result = response
        //    });
        //    Log.Info($"[END] {nameof(CustomerServices)}.{nameof(AllocateTreatToUser)}{Environment.NewLine}{info}");
        //    return response;
        //}

        //public IEnumerable<ApiEnum> GetRegistrationVillages()
        //{
        //    var apiResponse = _api.Get(_baseUri + "villages/registration", ContentType.XML);
        //    if (!apiResponse.Response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var xml = XDocument.Parse(apiResponse.ResponseBody);
        //    if (xml.Root == null)
        //    {
        //        return null;
        //    }

        //    var list = xml.Root.Descendants().Select(i => new ApiEnum()
        //    {
        //        Key = i.Attribute("Key").Value,
        //        Value = i.Value + " - " + i.Attribute("Language").Value
        //    });

        //    return list.OrderBy(i => i.Value);
        //}

        //public Customer GetCurrentUser()
        //{
        //    var id = MembershipHelper.GetCurrentUserId();

        //    if (id == Guid.Empty) return null;

        //    return GetCustomer(id);
        //}

        //public bool IsCustomerSuspended(Customer customer)
        //{
        //    return Configuration.SuspendedStatusList.Contains(customer.MembershipActivationStatus);
        //}

        //public bool IsCurrentUserSuspended()
        //{
        //    var customer = GetCurrentUser();

        //    return IsCustomerSuspended(customer);
        //}

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
    }
}
