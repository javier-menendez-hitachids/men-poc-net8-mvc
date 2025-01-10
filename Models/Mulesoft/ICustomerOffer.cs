namespace MenulioPocMvc.Models.Mulesoft
{
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
