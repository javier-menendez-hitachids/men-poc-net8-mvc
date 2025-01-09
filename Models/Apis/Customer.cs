namespace MenulioPocMvc.Models.Apis
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Language { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? County { get; set; }
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public string? AddressCountry { get; set; }
        public string? Password { get; set; }
        public string? BarcodeUrl { get; set; }
        public string? BarcodeNum { get; set; }
        public string? FacebookId { get; set; }
        public string? Provider { get; set; }
        public string? VillageCode { get; set; }
        public string? CampaignId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? VersionId { get; set; }
        public string? Source { get; set; }

        // This field is linked to eMail consent. 
        public bool? optOutEmail { get; set; }
        // This field is in fact linked to Mobile consent on Mulesoft API.
        public bool? optOutPhone { get; set; }
        // This field is linked to Post consent on Mulesoft API.
        public bool? optOutPost { get; set; }
        // This field is in fact linked to Mobile Channels consent on Mulesoft API 
        public bool? optOutMobile { get; set; }
        public bool? optOutGeo { get; set; }
        public bool? optOutPush { get; set; }
        public bool? optOutSms { get; set; }

        public bool NeedsPrivilege { get; set; }
        public bool NeedsRepermission { get; set; }
        public string? RepermissionToken { get; set; }

        public IEnumerable<CustomerPreference> Preferences { get; set; }
        public bool IsSingleOptIn { get; set; }

        public bool IsSensitiveInformation()
        {
            // check if the fields are set 
            // if so then there is sensitive information 
            // used to not sign in the user

            if (
                !string.IsNullOrEmpty(FirstName) ||
                !string.IsNullOrEmpty(LastName) ||
                !string.IsNullOrEmpty(Title) ||
                !string.IsNullOrEmpty(PhoneNumber) ||
                Gender != "0" ||
                !string.IsNullOrEmpty(AddressLine1) ||
                !string.IsNullOrEmpty(AddressLine2) ||
                !string.IsNullOrEmpty(County) ||
                !string.IsNullOrEmpty(City) ||
                !string.IsNullOrEmpty(PostCode) ||
                !string.IsNullOrEmpty(AddressCountry) ||
                DateOfBirth != DateTime.MinValue
                )
            {
                return true;
            }


            return false;
        }

        public bool OptInEmail { get; set; }
        public bool OptInMobile { get; set; }
        public bool OptInPhone { get; set; }
        public bool OptInPost { get; set; }

        public bool IsPrivateClient { get; set; }

        public string? MembershipActivationStatus { get; set; }
    }
}
