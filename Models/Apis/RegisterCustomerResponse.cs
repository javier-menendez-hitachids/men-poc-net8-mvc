namespace MenulioPocMvc.Models.Apis
{
    public class RegisterCustomerResponse : BaseResponse
    {
        public Customer CreatedCustomer { get; set; }
        public Guid CustomerId { get; set; }
    }
}
