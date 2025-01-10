namespace MenulioPocMvc.Models.Apis
{
    public class SpendTier : BaseTier
    {
        public decimal? AmountTo { get; set; }
        public decimal? AmountFrom { get; set; }
        public int? DayFrom { get; set; }
    }
}
