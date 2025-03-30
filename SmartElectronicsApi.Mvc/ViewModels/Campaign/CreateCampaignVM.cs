namespace SmartElectronicsApi.Mvc.ViewModels.Campaign
{
    public sealed record CreateCampaignVM
    {
        public required IFormFile formFile { get; init; }
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required DateTimeOffset StartDate { get; init; }
        public required DateTimeOffset EndDate { get; init; }
        public decimal? DiscountPercentage { get; init; }
        public List<int>? ProductIds { get; init; }
        public List<int?> BranchIds { get; init; }
    }
}
