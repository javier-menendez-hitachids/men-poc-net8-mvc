namespace MenulioPocMvc.Models.Apis
{
    public class CustomerExtended
    {
        public IList<CustomerPoint> CustomerPoints { get; set; }
        public long TotalCustomerPoints { get; set; }
        public Guid CurrentSpendTierId { get; set; }
        public Guid CurrentBehavioralTierId { get; set; }
    }
}
