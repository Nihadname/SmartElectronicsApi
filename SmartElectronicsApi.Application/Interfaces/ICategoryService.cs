using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> Create(CategoryCreateDto categoryCreateDto);
        Task<PaginatedResponse<CategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10);
        Task<int> Delete(int? id);
        Task<int> Update(int? id, CategoryUpdateDto categoryUpdateDto);
        Task<CategoryReturnDto> GetById(int? id);
        Task<List<CategoryListItemDto>> GetAllForUserInterface(int skip, int take);
    }
}
