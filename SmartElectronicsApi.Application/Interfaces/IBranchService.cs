using SmartElectronicsApi.Application.Dtos.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IBranchService
    {
        Task<string> CreateBranch(BranchCreateDto branchCreateDto);
        Task<IEnumerable<BranchSelectDto>> GetAllSelect();
    }
}
