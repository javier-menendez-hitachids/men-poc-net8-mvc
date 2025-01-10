namespace MenulioPocMvc.Models.Mulesoft
{
    public class CustomerTreatCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsToHide { get; set; }
    }
}
