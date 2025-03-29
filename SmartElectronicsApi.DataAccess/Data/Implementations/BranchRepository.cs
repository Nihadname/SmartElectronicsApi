using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class BranchRepository : Repository<Branch>, IBranchRepository
    {
        public BranchRepository(SmartElectronicsDbContext context) : base(context)
        {
        }
    }
}
