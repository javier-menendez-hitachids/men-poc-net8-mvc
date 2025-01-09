using MenulioPocMvc.Models.Apis;
using Newtonsoft.Json;
using System.Web;
using System.Xml.Linq;

namespace MenulioPocMvc.CustomerApi
{
    public class CustomerMapper
    {
        public static Customer MapToCustomer(XDocument xml)
        {
            if (xml.Root != null)
            {
                var properties = xml.Root.Descendants("Properties").FirstOrDefault();
                var references = xml.Root.Descendants("References").FirstOrDefault();
                var name = xml.Root.Descendants("name").FirstOrDefault();
                var address = xml.Root.Descendants("address").FirstOrDefault();

                var customer = new Customer
                {
                    Id = Guid.Parse(xml.Root.Attribute("CustomerId").Value),
                    VillageCode = xml.Root.Attribute("VillagePrimary").Value,
                    Gender = GetValueFromXmlNode(xml.Root, "Gender"),
                    VersionId = GetValueFromXmlNode(xml.Root, "VersionId"),
                    Source = GetValueFromXmlNode(xml.Root, "Source"),
                    Language = GetValueFromXmlNode(xml.Root, "Language"),
                    IsPrivateClient = ParseToBool(GetValueFromXmlNode(xml.Root, "IsPrivateClient")),
                    MembershipActivationStatus = GetValueFromXmlNode(xml.Root, "MembershipActivationStatus"),
                    DateOfBirth = xml.Root.Attribute("DateOfBirth") != null
                        ? DateTime.Parse(xml.Root.Attribute("DateOfBirth").Value)
                        : DateTime.MinValue
                };

                // Set properties
                if (properties != null)
                {
                    customer.BarcodeUrl = GetValueFromChildXmlNode(properties, "type", "BARCODEURL");
                    customer.PhoneNumber = GetValueFromChildXmlNode(properties, "type", "PHONENUMBER");

                    var leadSource =
                        properties.Descendants("item").FirstOrDefault(i => i.Attribute("type").Value == "LEADSOURCE");

                    if (leadSource != null)
                        customer.CampaignId = GetValueFromChildXmlNode(properties, "type", "LEADSOURCE");

                    customer.optOutMobile = GetNullableOptOutValue(properties, "OPTOUTMOBILE");
                    customer.optOutSms = GetNullableOptOutValue(properties, "OPTOUTSMS");
                    customer.optOutPhone = GetNullableOptOutValue(properties, "OPTOUTPHONE");
                    customer.optOutEmail = GetNullableOptOutValue(properties, "OPTOUTEMAIL");
                    customer.optOutPost = GetNullableOptOutValue(properties, "OPTOUTPOSTAL");
                    customer.optOutGeo = GetNullableOptOutValue(properties, "OPTOUTGEO");
                }

                // Set references
                if (references != null)
                {
                    customer.Email = GetValueFromChildXmlNode(references, "type", "EMAIL");
                    customer.BarcodeNum = GetValueFromChildXmlNode(references, "type", "BARCODENUMBER");
                }

                // Set name
                if (name != null)
                {
                    customer.Title = GetValueFromXmlNode(name, "Title");
                    customer.FirstName = GetValueFromXmlNode(name, "Given");
                    customer.LastName = GetValueFromXmlNode(name, "Family");
                }

                // Set address
                if (address != null)
                {
                    customer.AddressLine1 = GetValueFromXmlNode(address, "AddressLine1");
                    customer.AddressLine2 = GetValueFromXmlNode(address, "AddressLine2");
                    customer.County = GetValueFromXmlNode(address, "County");
                    customer.City = GetValueFromXmlNode(address, "City");
                    customer.PostCode = GetValueFromXmlNode(address, "PostCode");
                    customer.AddressCountry = GetValueFromXmlNode(address, "Country");
                }

                return customer;
            }

            return null;
        }

        public static XDocument CreateMissingElements(XDocument originalXml)
        {
            if (originalXml.Root == null)
            {
                return null;
            }

            var properties = originalXml.Root.Descendants("Properties").FirstOrDefault();
            var name = originalXml.Root.Descendants("name").FirstOrDefault();
            var address = originalXml.Root.Descendants("address").FirstOrDefault();

            if (originalXml.Root.Attribute("DateOfBirth") == null)
            {
                originalXml.Root.Add(new XAttribute("DateOfBirth", ""));
            }
            if (originalXml.Root.Attribute("Gender") == null)
            {
                originalXml.Root.Add(new XAttribute("Gender", ""));
            }
            if (originalXml.Root.Attribute("Language") == null)
            {
                originalXml.Root.Add(new XAttribute("Language", ""));
            }
            if (name != null)
            {
                if (name.Attribute("Family") == null)
                {
                    name.Add(new XAttribute("Family", ""));
                }
                if (name.Attribute("Given") == null)
                {
                    name.Add(new XAttribute("Given", ""));
                }
                if (name.Attribute("Title") == null)
                {
                    name.Add(new XAttribute("Title", ""));
                }
            }

            if (address == null) return originalXml;

            if (address.Attribute("AddressLine1") == null)
            {
                address.Add(new XAttribute("AddressLine1", ""));
            }
            if (address.Attribute("AddressLine2") == null)
            {
                address.Add(new XAttribute("AddressLine2", ""));
            }
            if (address.Attribute("County") == null)
            {
                address.Add(new XAttribute("County", ""));
            }
            if (address.Attribute("City") == null)
            {
                address.Add(new XAttribute("City", ""));
            }
            if (address.Attribute("PostCode") == null)
            {
                address.Add(new XAttribute("PostCode", ""));
            }
            if (address.Attribute("Country") == null)
            {
                address.Add(new XAttribute("Country", ""));
            }
            return originalXml;
        }

        public static XDocument MapToXml(Customer customer)
        {
            var dateCreated = DateTime.Now.ToString("s");

            XDocument xml;

            if (string.IsNullOrWhiteSpace(customer.FacebookId))
            {
                xml = new XDocument(
                new XElement("Customer",
                    new XAttribute("DateCreated", dateCreated),
                    new XAttribute("DateOfBirth", string.Empty),
                    new XAttribute("Language", customer.Language),
                    new XAttribute("Gender", string.IsNullOrWhiteSpace(customer.Gender) ? "0" : customer.Gender),
                    new XAttribute("Provider", string.IsNullOrWhiteSpace(customer.Provider) ? string.Empty : customer.Provider),
                    new XAttribute("VillagePrimary", customer.VillageCode.ToUpper()),
                    new XAttribute("Source", "Website"),
                    new XElement("Properties",
                        new XElement("item", new XAttribute("type", "CUSTOMERIDHASH")),
                        new XElement("item", new XAttribute("type", "CUSTOMERCARDID")),
                        new XElement("item", new XAttribute("type", "PHONENUMBER"), string.IsNullOrWhiteSpace(customer.PhoneNumber) ? string.Empty : customer.PhoneNumber),
                        new XElement("item", new XAttribute("type", "NATIONALITY")),
                        new XElement("item", new XAttribute("type", "BARCODENUMBER")),
                        new XElement("item", new XAttribute("type", "OLDNATIONALITY")),
                        new XElement("item", new XAttribute("type", "LEADSOURCE"), customer.CampaignId)),
                    new XElement("References",
                        new XElement("item", new XAttribute("type", "CUSTOMERID")),
                        new XElement("item", new XAttribute("type", "EMAIL"), customer.Email)),
                    new XElement("name",
                        new XAttribute("Given", string.IsNullOrWhiteSpace(customer.FirstName) ? string.Empty : customer.FirstName),
                        new XAttribute("Family", string.IsNullOrWhiteSpace(customer.LastName) ? string.Empty : customer.LastName)),
                    new XElement("address",
                        new XAttribute("AddressLine1", string.IsNullOrWhiteSpace(customer.AddressLine1) ? string.Empty : customer.AddressLine1),
                        new XAttribute("AddressLine2", string.IsNullOrWhiteSpace(customer.AddressLine2) ? string.Empty : customer.AddressLine2),
                        new XAttribute("County", ""),
                        new XAttribute("City", string.IsNullOrWhiteSpace(customer.City) ? string.Empty : customer.City),
                        new XAttribute("PostCode", string.IsNullOrWhiteSpace(customer.PostCode) ? string.Empty : customer.PostCode),
                        new XAttribute("Country", string.IsNullOrWhiteSpace(customer.AddressCountry) ? string.Empty : customer.AddressCountry))));

                UpdateOrRemoveDateOfBirthAttribute(customer, xml);
            }
            else
            {
                customer.Gender = "0";
                xml = new XDocument(
                new XElement("Customer",
                    new XAttribute("DateCreated", dateCreated),
                    new XAttribute("Language", customer.Language),
                    new XAttribute("Gender", customer.Gender),
                    new XAttribute("IsNativeRegistration", false),
                    new XAttribute("Provider", string.IsNullOrWhiteSpace(customer.Provider) ? string.Empty : customer.Provider),
                    new XAttribute("VillagePrimary", customer.VillageCode.ToUpper()),
                    new XAttribute("Source", "Website"),
                    new XElement("Properties",
                        new XElement("item", new XAttribute("type", "CUSTOMERIDHASH")),
                        new XElement("item", new XAttribute("type", "CUSTOMERCARDID")),
                        new XElement("item", new XAttribute("type", "PHONENUMBER")),
                        new XElement("item", new XAttribute("type", "NATIONALITY")),
                        new XElement("item", new XAttribute("type", "BARCODENUMBER")),
                        new XElement("item", new XAttribute("type", "OLDNATIONALITY")),
                        new XElement("item", new XAttribute("type", "LEADSOURCE"), customer.CampaignId)),
                    new XElement("References",
                        new XElement("item", new XAttribute("type", "FACEBOOKID"), string.IsNullOrWhiteSpace(customer.FacebookId) ? null : customer.FacebookId),
                        new XElement("item", new XAttribute("type", "CUSTOMERID")),
                        new XElement("item", new XAttribute("type", "EMAIL"), customer.Email)),
                    new XElement("name",
                        new XAttribute("Given", string.IsNullOrWhiteSpace(customer.FirstName) ? string.Empty : customer.FirstName),
                        new XAttribute("Family", string.IsNullOrWhiteSpace(customer.LastName) ? string.Empty : customer.LastName)),
                    new XElement("address",
                        new XAttribute("AddressLine1", ""),
                        new XAttribute("AddressLine2", ""),
                        new XAttribute("County", ""),
                        new XAttribute("City", ""),
                        new XAttribute("PostCode", ""),
                        new XAttribute("Country", ""))));
            }

            return xml;
        }

        public static string MapToJsonString(Customer customer)
        {
            return JsonConvert.SerializeObject(MapToJson(customer));
        }

        public static Models.Common.Customer MapRegistrationToJson(Customer customer)
        {
            Models.Common.Customer result = MapCustomerBaseToJson(customer);
            // Map eMail consent (custom registration form)
            result.OptOutEmail = customer.optOutEmail;

            return result;
        }

        public static Models.Common.Customer MapPrivateClientRegistrationToJson(Customer customer)
        {
            Models.Common.Customer result = MapCustomerBaseToJson(customer);

            // Check if the customer is a private client to send the proper consent fields
            if (!customer.IsPrivateClient)
            {
                result.OptOutEmail = customer.optOutEmail;
                result.OptOutMobile = customer.optOutMobile;
                result.OptOutPhone = customer.optOutPhone;
                result.OptOutPostal = customer.optOutPost;
            }

            // Set IsPrivateClient to true to send guestType as 'Private Client + Member' to Mulesoft
            result.IsPrivateClient = true;

            return result;
        }

        public static Models.Common.Customer MapCustomerBaseToJson(Customer customer)
        {
            // Set common fields
            Models.Common.Customer result = new Models.Common.Customer
            {
                SingleOptIn = customer.IsSingleOptIn,
                DateOfBirth = customer.DateOfBirth,
                Language = customer.Language,
                Provider = string.IsNullOrWhiteSpace(customer.Provider) ? string.Empty : customer.Provider,
                VillagePrimary = customer.VillageCode.ToUpper(),
                SourceSystem = "Website",
                NameGiven = string.IsNullOrWhiteSpace(customer.FirstName) ? string.Empty : customer.FirstName,
                NameFamily = string.IsNullOrWhiteSpace(customer.LastName) ? string.Empty : customer.LastName,
                Email = customer.Email,
                EncodedPassword = HttpUtility.UrlEncode(customer.Password),
                PhoneNumber = customer.PhoneNumber
            };

            // Set common properties
            result.PropertySet("CUSTOMERIDHASH", string.Empty);
            result.PropertySet("CUSTOMERCARDID", string.Empty);
            result.PropertySet("NATIONALITY", string.Empty);
            result.PropertySet("BARCODENUMBER", string.Empty);
            result.PropertySet("OLDNATIONALITY", string.Empty);
            result.PropertySet("LEADSOURCE", customer.CampaignId);

            // If registeed from facebook, ignore address, phone and gender fields
            if (string.IsNullOrWhiteSpace(customer.FacebookId))
            {
                result.IsNativeRegistration = true;
                result.Gender = string.IsNullOrWhiteSpace(customer.Gender) ? "0" : customer.Gender;
                result.AddressLine1 = string.IsNullOrWhiteSpace(customer.AddressLine1) ? string.Empty : customer.AddressLine1;
                result.AddressLine2 = string.IsNullOrWhiteSpace(customer.AddressLine2) ? string.Empty : customer.AddressLine2;
                result.County = string.IsNullOrWhiteSpace(customer.County) ? string.Empty : customer.County;
                result.City = string.IsNullOrWhiteSpace(customer.City) ? string.Empty : customer.City;
                result.PostCode = string.IsNullOrWhiteSpace(customer.PostCode) ? string.Empty : customer.PostCode;
                result.Country = string.IsNullOrWhiteSpace(customer.AddressCountry) ? string.Empty : customer.AddressCountry;

                result.PropertySet("PHONENUMBER", string.IsNullOrWhiteSpace(customer.PhoneNumber) ? string.Empty : customer.PhoneNumber);
            }
            else
            {
                result.IsNativeRegistration = false;
                result.FacebookId = string.IsNullOrWhiteSpace(customer.FacebookId) ? null : customer.FacebookId;
                result.Gender = "0";
                result.AddressLine1 = string.Empty;
                result.AddressLine2 = string.Empty;
                result.County = string.Empty;
                result.City = string.Empty;
                result.PostCode = string.Empty;
                result.Country = string.Empty;

                result.PropertySet("PHONENUMBER", string.Empty);
            }

            return result;
        }

        public static Models.Common.Customer MapToJson(Customer customer)
        {
            // Set common fields
            Models.Common.Customer result = new Models.Common.Customer
            {
                SingleOptIn = customer.IsSingleOptIn,
                DateOfBirth = customer.DateOfBirth,
                Language = customer.Language,
                Provider = string.IsNullOrWhiteSpace(customer.Provider) ? string.Empty : customer.Provider,
                VillagePrimary = customer.VillageCode.ToUpper(),
                SourceSystem = "Website",
                NameGiven = string.IsNullOrWhiteSpace(customer.FirstName) ? string.Empty : customer.FirstName,
                NameFamily = string.IsNullOrWhiteSpace(customer.LastName) ? string.Empty : customer.LastName,
                Email = customer.Email,
                EncodedPassword = HttpUtility.UrlEncode(customer.Password),
                IsPrivateClient = customer.IsPrivateClient,
            };



            // Set common properties
            result.PropertySet("CUSTOMERIDHASH", string.Empty);
            result.PropertySet("CUSTOMERCARDID", string.Empty);
            result.PropertySet("NATIONALITY", string.Empty);
            result.PropertySet("BARCODENUMBER", string.Empty);
            result.PropertySet("OLDNATIONALITY", string.Empty);
            result.PropertySet("LEADSOURCE", customer.CampaignId);

            // If registeed from facebook, ignore address, phone and gender fields
            if (string.IsNullOrWhiteSpace(customer.FacebookId))
            {
                result.IsNativeRegistration = true;
                result.Gender = string.IsNullOrWhiteSpace(customer.Gender) ? "0" : customer.Gender;
                result.AddressLine1 = string.IsNullOrWhiteSpace(customer.AddressLine1) ? string.Empty : customer.AddressLine1;
                result.AddressLine2 = string.IsNullOrWhiteSpace(customer.AddressLine2) ? string.Empty : customer.AddressLine2;
                result.County = string.IsNullOrWhiteSpace(customer.County) ? string.Empty : customer.County;
                result.City = string.IsNullOrWhiteSpace(customer.City) ? string.Empty : customer.City;
                result.PostCode = string.IsNullOrWhiteSpace(customer.PostCode) ? string.Empty : customer.PostCode;
                result.Country = string.IsNullOrWhiteSpace(customer.AddressCountry) ? string.Empty : customer.AddressCountry;

                result.PropertySet("PHONENUMBER", string.IsNullOrWhiteSpace(customer.PhoneNumber) ? string.Empty : customer.PhoneNumber);
            }
            else
            {
                result.IsNativeRegistration = false;
                result.FacebookId = string.IsNullOrWhiteSpace(customer.FacebookId) ? null : customer.FacebookId;
                result.Gender = "0";
                result.AddressLine1 = string.Empty;
                result.AddressLine2 = string.Empty;
                result.County = string.Empty;
                result.City = string.Empty;
                result.PostCode = string.Empty;
                result.Country = string.Empty;

                result.PropertySet("PHONENUMBER", string.Empty);
            }

            return result;
        }

        public static XDocument MapToXml(Customer customer, XDocument originalXml)
        {
            if (originalXml.Root == null) return null;
            // create all the elements and attributes - add them to the original XML
            originalXml = CreateMissingElements(originalXml);

            // get the three main sections
            var properties = originalXml.Root.Descendants("Properties").FirstOrDefault();
            var name = originalXml.Root.Descendants("name").FirstOrDefault();
            var address = originalXml.Root.Descendants("address").FirstOrDefault();

            UpdateOrRemoveDateOfBirthAttribute(customer, originalXml);

            //originalXml.Root.Attribute("Gender").Value = customer.Gender;
            UpdateOrRemoveAttribute("Gender", customer.Gender, originalXml.Root);

            //originalXml.Root.Attribute("Language").Value = customer.Language;
            UpdateOrRemoveAttribute("Language", customer.Language, originalXml.Root);

            if (properties != null && properties.Descendants().Any())
            {
                // get the phone number element
                var phoneNumberElement = properties.Descendants().FirstOrDefault(i => i.Attribute("type").Value == "PHONENUMBER");

                // if it exists
                if (phoneNumberElement != null)
                {
                    phoneNumberElement.Value = customer.PhoneNumber;
                }
                else
                {
                    properties.Add(new XElement("item", new XAttribute("type", "PHONENUMBER"), "0"));
                    phoneNumberElement = properties.Descendants().FirstOrDefault(i => i.Attribute("type").Value == "PHONENUMBER");
                    if (phoneNumberElement != null)
                    {
                        phoneNumberElement.Value = customer.PhoneNumber;
                    }
                }

                SetOptValue(properties, "OPTOUTEMAIL", customer.optOutEmail);
                SetOptValue(properties, "OPTOUTMOBILE", customer.optOutMobile);
                SetOptValue(properties, "OPTOUTSMS", customer.optOutSms);
                SetOptValue(properties, "OPTOUTPHONE", customer.optOutPhone);
                SetOptValue(properties, "OPTOUTPOSTAL", customer.optOutPost);
                SetOptValue(properties, "OPTOUTGEO", customer.optOutGeo);
            }

            if (name != null)
            {

                UpdateOrRemoveAttribute("Family", customer.LastName, name);
                UpdateOrRemoveAttribute("Given", customer.FirstName, name);
                UpdateOrRemoveAttribute("Title", customer.Title, name);

            }

            if (address != null)
            {
                UpdateOrRemoveAttribute("AddressLine1", customer.AddressLine1, address);
                UpdateOrRemoveAttribute("AddressLine2", customer.AddressLine2, address);
                UpdateOrRemoveAttribute("City", customer.City, address);
                UpdateOrRemoveAttribute("PostCode", customer.PostCode, address);
                UpdateOrRemoveAttribute("County", customer.County, address);
                UpdateOrRemoveAttribute("Country", customer.AddressCountry, address);
            }

            return originalXml;
        }

        private static bool? GetNullableOptOutValue(XElement properties, string type)
        {
            var optOut = properties.Descendants("item").FirstOrDefault(i => i.Attribute("type").Value == type);
            if (optOut != null)
            {
                var xmlAttr = GetValueFromChildXmlNode(properties, "type", type).ToString();
                if (xmlAttr.Equals("0") || xmlAttr.Equals("1"))
                    return Convert.ToBoolean(Convert.ToInt32(xmlAttr));
                else
                    return null;
            }

            return null;
        }

        private static bool GetOptOutValue(XElement properties, string type)
        {
            var optOut = properties.Descendants("item").FirstOrDefault(i => i.Attribute("type").Value == type);
            if (optOut != null)
            {
                var xmlAttr = GetValueFromChildXmlNode(properties, "type", type).ToString();
                if (xmlAttr.Equals("0") || xmlAttr.Equals("1"))
                    return Convert.ToBoolean(Convert.ToInt32(xmlAttr));
                else
                    return true;
            }

            return true;
        }



        private static void SetOptValue(XElement properties, string type, bool? value)
        {
            if (value != null)
            {
                var element = properties.Descendants().FirstOrDefault(i => i.Attribute("type").Value == type);
                if (element != null)
                {
                    element.Value = Convert.ToInt32(value).ToString();
                }
                else
                {
                    properties.Add(new XElement("item", new XAttribute("type", type), "1"));
                    element = properties.Descendants().FirstOrDefault(i => i.Attribute("type").Value == type);
                    if (element != null)
                    {
                        element.Value = Convert.ToInt32(value).ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Method that sets an attribute or removes it if it's not set
        /// </summary>
        /// <param name="attributeName">Name of attribute to be set</param>
        /// <param name="attributeValue">Value of attribute to be set</param>
        /// <param name="element">The element</param>
        private static void UpdateOrRemoveAttribute(string attributeName, string attributeValue, XElement element)
        {
            if (!string.IsNullOrEmpty(attributeValue))
            {
                element.Attribute(attributeName).Value = attributeValue;
            }
            else
            {
                if (element.Attribute(attributeName) != null)
                {
                    element.Attribute(attributeName).Remove();
                }
            }
        }

        /// <summary>
        /// Update Date Of Birth attribute if there is a value 
        /// or remove it if DOB is equal to DateTime.MinValue
        /// </summary>
        /// <param name="customer">Customer class</param>
        /// <param name="originalXml">Xml provided by web service</param>
        private static void UpdateOrRemoveDateOfBirthAttribute(Customer customer, XDocument originalXml)
        {
            if (customer.DateOfBirth.HasValue && customer.DateOfBirth != DateTime.MinValue)
            {
                originalXml.Root.Attribute("DateOfBirth").Value = customer.DateOfBirth.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                if (originalXml.Root.Attribute("DateOfBirth") != null)
                {
                    originalXml.Root.Attribute("DateOfBirth").Remove();
                }
            }
        }


        private static string setXMLAttribute(XElement parentNode, string attribute, string modelValue)
        {
            string output = "";
            if (parentNode.Attribute(attribute) == null)
            {

                parentNode.Add(new XAttribute(attribute, ""));
                if (parentNode.Attribute(attribute) != null)
                {
                    output = modelValue;
                }
            }
            return output;

        }

        private static string GetValueFromChildXmlNode(XElement parentNode, string attrToMatch, string attrValueToMatch)
        {
            var firstOrDefault = parentNode.Descendants()
                .FirstOrDefault(i => i.Attribute(attrToMatch).Value == attrValueToMatch);

            return firstOrDefault != null ? firstOrDefault.Value : "";
        }

        private static string GetValueFromXmlNode(XElement node, string attribute)
        {
            return node.Attribute(attribute) != null ? node.Attribute(attribute).Value : "";
        }

        public static bool ParseToBool(string value)
        {
            bool result;
            bool.TryParse(value, out result);
            return result;
        }
    }
}
