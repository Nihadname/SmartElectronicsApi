using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Core.Entities;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> Create(CategoryCreateDto categoryCreateDto); 
    }
}
