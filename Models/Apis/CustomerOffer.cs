namespace MenulioPocMvc.Models.Apis
{
    public class CustomerOffer : ICustomerOffer
    {
        public string OfferId { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string PassbookTitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Terms { get; set; }
        public string BarcodeUrl { get; set; }
        public string BarcodeNumber { get; set; }
        public string Conditions { get; set; }
        public bool IsActive { get; set; }
        public bool Redeemed { get; set; }
        public string VillageID { get; set; }
        public string ConditionsUrl { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsTeaser => false;
    }

    public interface ICustomerOffer
    {
        string OfferId { get; set; }
        string Title { get; set; }
        string Title2 { get; set; }
        string PassbookTitle { get; set; }
        string Description { get; set; }
        string ImageUrl { get; set; }
        string Terms { get; set; }
        string BarcodeUrl { get; set; }
        string BarcodeNumber { get; set; }
        string Conditions { get; set; }
        bool IsActive { get; set; }
        bool Redeemed { get; set; }
        string VillageID { get; set; }
        string ConditionsUrl { get; set; }
        Guid CategoryId { get; set; }
        bool IsTeaser { get; }
    }
}
