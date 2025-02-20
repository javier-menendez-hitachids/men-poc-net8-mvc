﻿namespace MenulioPocMvc.Models.Mulesoft
{
    public class CustomerOffer
    {
        public string OfferId { get; set; }
        public string OfferIdCode { get; set; }
        public string Title { get; set; }
        public string Title2 { get; set; }
        public string PassbookTitle { get; set; }
        public string Description { get; set; }
        public string Instructions { get; set; }
        public string ImageUrl { get; set; }
        public string Terms { get; set; }
        public string BarcodeUrl { get; set; }
        public string BarcodeNumber { get; set; }
        public string Conditions { get; set; }
        public bool IsActive { get; set; }
        public bool Redeemed { get; set; }
        public string VillageID { get; set; }
        public string ConditionsUrl { get; set; }
        public string CategoryId { get; set; }
        public string LanguageId { get; set; }
        public string OutOfStockMessage { get; set; }
        public string DateRedeemed { get; set; }
        public string DateExpiry { get; set; }
        public string Channel { get; set; }
        public string RedemptionLimit { get; set; }
        public string Status { get; set; }
        public bool IsTeaser => false;
    }

    
}
