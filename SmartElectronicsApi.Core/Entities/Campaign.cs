using SmartElectronicsApi.Core.Entities.Common;

namespace SmartElectronicsApi.Core.Entities
{
    public sealed class Campaign:BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get;  set; }
        public decimal? DiscountPercentageValue { get; set; }
        public ICollection<CampaignProduct> CampaignProducts { get; set; }
        public ICollection<BranchCampaign> branchCampaigns { get; set; }

    }
    public enum Status
    {
        Active,
        Inactive,
    }
}
