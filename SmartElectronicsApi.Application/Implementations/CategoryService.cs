using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.Slider;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;
using SmartElectronicsApi.DataAccess.Data.Implementations;

namespace SmartElectronicsApi.Application.Implementations
{
    public class CategoryService : ICategoryService
    {
      private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Category> Create(CategoryCreateDto categoryCreateDto)
        {
            var Category = _mapper.Map<Category>(categoryCreateDto);

            await _unitOfWork.categoryRepository.Create(Category);
            _unitOfWork.Commit();
            return Category;    
        }
        public async Task<PaginatedResponse<CategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount=(await _unitOfWork.categoryRepository.GetAll()).Count();
            var categories = await _unitOfWork.categoryRepository.GetAll(s=>s.IsDeleted==false,(pageNumber-1)*pageSize, pageSize)
           var categoriesWithMapping = _mapper.Map<List<CategoryListItemDto>>(categories);
            var paginatedResult = new PaginatedResponse<CategoryListItemDto>
            {
                Data = categoriesWithMapping,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return paginatedResult;

        }
    }
}
