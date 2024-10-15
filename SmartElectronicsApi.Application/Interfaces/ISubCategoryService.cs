using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ISubCategoryService
    {
        Task<SubCategory> Create(SubCategoryCreateDto subCategoryCreateDto);
        Task<int> Delete(int? id);
        Task<PaginatedResponse<SubCategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10);
        Task<SubCategoryReturnDto> GetById(int? id);
        Task<int> Update(int? id,SubCategoryUpdateDto subCategoryUpdateDto);
        Task<List<SubCategoryListItemDto>> GetAll();    }
}
