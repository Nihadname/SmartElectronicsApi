using SmartElectronicsApi.Application.Dtos.Branch;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class BranchService:IBranchService
    {
        private  readonly IUnitOfWork _unitOfWork;

        public BranchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> CreateBranch(BranchCreateDto branchCreateDto)
        {
            var isExistBranchNameExist=await _unitOfWork.BranchRepository.isExists(s=>s.Name.ToLower()==branchCreateDto.Name.ToLower());
            if (isExistBranchNameExist)
                throw new CustomException(400, "Name already exists");
            var newBranch = new Branch()
            {
                Name = branchCreateDto.Name,
            };
            await _unitOfWork.BranchRepository.Create(newBranch);
            _unitOfWork.Commit();
            return newBranch.Name;
        }
        public async Task<IEnumerable<BranchSelectDto>> GetAllSelect()
        {
            var branchs= (await _unitOfWork.BranchRepository.GetAll()).Select(s=>new BranchSelectDto() {
                Name = s.Name.Trim(),
                Id = s.Id,
            });
            return branchs;
        }
        
    }
}
