namespace MenulioPocMvc.Models.Apis
{
    public class VillageTeaser
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ConditionDescription { get; set; }
        public Guid CategoryId { get; set; }
        public string LanguageId { get; set; }
    }
}
