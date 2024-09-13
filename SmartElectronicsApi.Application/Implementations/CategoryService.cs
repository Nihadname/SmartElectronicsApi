using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;

namespace SmartElectronicsApi.Application.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository; 
        }

        public Category Create(Category category)
        {
           
            throw new NotImplementedException();
        }
    }
}
