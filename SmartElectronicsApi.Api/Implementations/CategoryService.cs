using SmartElectronicsApi.Api.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;

namespace SmartElectronicsApi.Api.Implementations
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
