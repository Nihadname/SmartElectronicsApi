namespace SmartElectronicsApi.Mvc.ViewModels.Campaign
{
    public class CampaignListItemVM
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal? DiscountPercentageValue { get; set; }
    }
}
