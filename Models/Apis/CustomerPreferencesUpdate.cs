namespace MenulioPocMvc.Models.Apis
{
    public class CustomerPreferencesUpdate
    {
        public IEnumerable<CustomerPreference> OldPreferences { get; set; }
        public IEnumerable<CustomerPreference> NewPreferences { get; set; }
    }
}
