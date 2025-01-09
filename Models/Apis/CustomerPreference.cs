namespace MenulioPocMvc.Models.Apis
{
    public class CustomerPreference
    {
        public Guid CustomerId { get; set; }
        public string? Group { get; set; }
        public string? Key { get; set; }
        public bool Value { get; set; }
        public string? Label { get; set; }
    }
}
