using System.Text.Json.Serialization;

namespace MenulioPocMvc.Models.Apis
{
    public class CustomerPoint
    {
        public Guid Id { get; set; }
        public long CustomerId { get; set; }
        public Guid CustomerExternalId { get; set; }
        // Currently, DI is Storing TransactionID as 0, se we're ignoring this info when on json processes
        [JsonIgnore]
        public Guid? TransactionId { get; set; }
        public Guid TypeId { get; set; }
        public int Points { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
