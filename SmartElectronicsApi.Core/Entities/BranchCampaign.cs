using SmartElectronicsApi.Core.Entities.Common;

namespace SmartElectronicsApi.Core.Entities
{
    public class BranchCampaign:BaseEntity
    {
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}
