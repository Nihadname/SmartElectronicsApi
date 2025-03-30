using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
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
            if(await _unitOfWork.categoryRepository.isExists(s=>s.Name.ToLower() == categoryCreateDto.Name.ToLower()))
            {
                throw new CustomException(400, "Name", "this category name already exists");

            }
            var Category = _mapper.Map<Category>(categoryCreateDto);

            await _unitOfWork.categoryRepository.Create(Category);
            _unitOfWork.Commit();
            return Category;
        }
        public async Task<PaginatedResponse<CategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10)
        {

            try
            {
                var totalCount = (await _unitOfWork.categoryRepository.GetAll()).Count();
                var categories = await _unitOfWork.categoryRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize, includes: new Func<IQueryable<Category>, IQueryable<Category>>[]
                {
        query => query.Include(c => c.Products)
                      .Include(c => c.SubCategories)
                      //.ThenInclude(sc => sc.brandSubCategories) // Include BrandSubCategories
                      //.ThenInclude(bsc => bsc.Brand)
                      //.ThenInclude(s=>s.Products)
                });

                var categoriesWithMapping = _mapper.Map<List<CategoryListItemDto>>(categories);
                var paginatedResult = new PaginatedResponse<CategoryListItemDto>
                {
                    Data = categoriesWithMapping,
                    TotalRecords = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return paginatedResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex);
            }
        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var category = await _unitOfWork.categoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (category is null) throw new CustomException(404, "Not found");
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                category.ImageUrl.DeleteFile();
            }
            await _unitOfWork.categoryRepository.Delete(category);
            _unitOfWork.Commit();
            return category.Id;
        }


        public async Task<int> Update(int? id, CategoryUpdateDto categoryUpdateDto)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var category = await _unitOfWork.categoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (category is null) throw new CustomException(404, "Not found");
            //if (string.IsNullOrWhiteSpace(categoryUpdateDto.Name))
            //{
            //    throw new CustomException(400, "Name", "Category name can't be empty");
            //}
            if (!string.IsNullOrEmpty(categoryUpdateDto.Name))
            {
                if (!category.Name.ToLower().Equals(categoryUpdateDto.Name.ToLower()))
                {
                    if (await _unitOfWork.categoryRepository.isExists(s => s.Name.ToLower() == categoryUpdateDto.Name.ToLower() && s.Id != id))
                    {
                        throw new CustomException(400, "Name", "This category name already exists in another category");
                    }
                }

            }


            _mapper.Map(categoryUpdateDto, category);

            if (categoryUpdateDto.formFile != null)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    category.ImageUrl.DeleteFile();
                }

                category.ImageUrl = categoryUpdateDto.formFile.Save(Directory.GetCurrentDirectory(), "img");
            }
            await _unitOfWork.categoryRepository.Update(category);
            _unitOfWork.Commit();

            return category.Id;
        }
        public async Task<CategoryReturnDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id can't be null");

            var category = await _unitOfWork.categoryRepository.GetEntity(
                s => s.Id == id && s.IsDeleted == false,
                includes: new Func<IQueryable<Category>, IQueryable<Category>>[]
                {
            query => query.Include(c => c.SubCategories)
                          .ThenInclude(sc => sc.brandSubCategories) // Include BrandSubCategories
                          .ThenInclude(bsc => bsc.Brand), // Include Brands through join entity
            query => query.Include(c => c.Products)
                });

            if (category is null) throw new CustomException(404, "Not found");

            var categoryWithMapping = _mapper.Map<CategoryReturnDto>(category);
            return categoryWithMapping;
        }
        public async Task<List<CategoryListItemDto>> GetAllForUserInterface(int skip, int take)
        {
          
                var categories = await _unitOfWork.categoryRepository.GetAll(
               s => s.IsDeleted == false,
               skip,
               take,
               includes: new Func<IQueryable<Category>, IQueryable<Category>>[]
               {
            query => query.Include(c => c.SubCategories)
                          .ThenInclude(sc => sc.brandSubCategories) 
                          .ThenInclude(bsc => bsc.Brand), 
            query => query.Include(c => c.Products)
               });

                var categoryItemDto = _mapper.Map<List<CategoryListItemDto>>(categories);
                return categoryItemDto;

           


        }
        public async Task<List<CategoryListItemDto>> GetForLandingPage()
        {
            var categories =( await _unitOfWork.categoryRepository.GetAll()).Take(4);
            var MappedCategories=_mapper.Map<List<CategoryListItemDto>>(categories);
            return MappedCategories;
           
        }
    }
}
