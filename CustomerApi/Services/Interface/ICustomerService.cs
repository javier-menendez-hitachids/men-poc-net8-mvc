using MenulioPocMvc.Models.Apis;
using MenulioPocMvc.Models.Enums;
using MenulioPocMvc.Models.Mulesoft;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;

namespace MenulioPocMvc.CustomerApi.Services.Interface
{
    public interface ICustomerService
    {
        Task<SignInResponse> SignIn(string emailAddress, string password);
        Task<bool> SocialSignIn(ReferenceType provider, string socialId);
        Task<Customer> GetCustomer(Guid customerId);
        Task<Customer> GetCustomerByRef(ReferenceType refType, string refValue);
        Task<RegisterCustomerResponse> RegisterCustomer(Customer customer);
        Task<UpdateProfileResponse> UpdateProfile(Customer customer);
        Task<IEnumerable<CustomerPreference>> GetCustomerPreferences(Guid customerId);
        Task CreateCustomerPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences);
        Task UpdatePreferencesMulesoft(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences);
        Task CreateCustomerContactPreferences(Guid customerId, IEnumerable<CustomerPreference> oldPreferences, IEnumerable<CustomerPreference> newPreferences);
        Task<CustomerExtended> GetCustomerMembershipInfo(Guid customerId);
        Task<GuestInfo> GetCustomerMembershipInfoV2(Guid customerId);
        Task<SpendTier> GetCustomerSpendTierInfo(Guid tierId);
        Task<SegmentInfo> GetCustomerSpendTierInfoV2(string tierId);
        Task<RewardsInfo> GetCustomerOffersV2(Guid customerId, string language = null);
        IAsyncEnumerable<Models.Apis.ICustomerOffer> GetCustomerOffers(Guid customerId, string language = null);
        Task<IList<Models.Apis.CustomerTreatCategory>> GetTreatCategories(Guid customerId, string villageCode, string language = null);
        Task<IList<Models.Mulesoft.CustomerTreatCategory>> GetTreatCategoriesV2(Guid customerId, string villageCode, string language = null);
        
        Task<ForgottenPasswordResponse> ResetPasswordRequest(string emailAddress);
        Task<BaseResponse> InitialisePassword(Customer customer);
        Task<BaseResponse> ResetPassword(string token, string newPassword);
        Task<bool> ChangePassword(Guid customerId, string newPassword);
        Task<RegisterCustomerResponse> ConfirmEmail(string token);
        Task<BaseResponse> ResetEmail(Guid customerId, string oldEmail, string newEmail);
        Task<BaseResponse> Repermission(string email);
        Task<BaseResponse> AcceptPrivilege(Guid customerID);
        Task<BaseResponse> CreateCustomerExtendedProperties(CustomerPreference preference);
        IAsyncEnumerable<ApiEnum> GetCustomerExtendedProperties(Guid customerId);
        Task<BaseResponse> DeleteCustomerExtendedProperties(Guid customerId, string property);
        IAsyncEnumerable<ApiEnum> GetVillages();
        IAsyncEnumerable<ApiEnum> GetLanguages(string locale);
        IAsyncEnumerable<ApiEnum> GetCountries(string locale);
        Task<TokenStatus> GetTokenStatus(string token);
        Task<IList<UserStates>> GetStates(Guid customerId);
        Task<bool> UpdateMarketingPreferences(Guid customerId, bool emailMarketing, bool smsMarketing, bool phoneMarketing, bool postMarketing, bool locationMarketing);
        Task<bool> UpdateTerms(Guid customerId);
        Task<bool?> HasPassword(Guid customerId);
        Task<BaseResponse> AllocateTreatToUser(Guid customerId, string leadSource);
        Task<IEnumerable<ApiEnum>> GetRegistrationVillages();

        // Non-async methods
        IEnumerable<SelectListItem> GetDaysList();
        IEnumerable<SelectListItem> GetMonthsList();
        IEnumerable<SelectListItem> GetYearsList();

        // REmove these below methods
        Task<IList<ApiEnum>> GetBrandPreferences(Guid customerId);
        Task<BaseResponse> CreateBrandPreferences(Guid customerId, string brandId);
        Task<BaseResponse> DeleteBrandPreferences(Guid customerId, string brandId);
        IAsyncEnumerable<Models.Apis.ICustomerOffer> GetVillageTeasers(string villageCode, string languageId);
        IAsyncEnumerable<CustomerBrandPreference> GetCustomerBrandPreferences(Guid customerId);
        IAsyncEnumerable<ApiEnum> GetVillageBrandsAsync(string villageId);
        Task<bool> CreateCustomerBrandPreferences(Guid customerId, string brandId);
        Task<bool> DeleteCustomerBrandPreferences(Guid customerId, string brandId);
        Task<IList<ApiEnum>> GetBrands();
    }
}
