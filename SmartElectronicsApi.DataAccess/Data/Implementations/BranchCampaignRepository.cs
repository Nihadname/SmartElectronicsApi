using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class BranchCampaignRepository : Repository<BranchCampaign>, IBranchCampaignRepository
    {
        public BranchCampaignRepository(SmartElectronicsDbContext context) : base(context)
        {
        }
    }
}
