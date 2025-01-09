using MenulioPocMvc.Models.Apis;

namespace MenulioPocMvc.CustomerApi.Services.Interface
{
    public interface ICustomerService
    {
        Task<SignInResponse> SignIn(string emailAddress, string password);
        Task<Customer> GetCustomer(Guid customerId);
        Task<RegisterCustomerResponse> RegisterCustomer(Customer customer);
    }
}
