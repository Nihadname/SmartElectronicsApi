using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public SubCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubCategory> Create(SubCategoryCreateDto subCategoryCreateDto)
        {
           
                if (!await _unitOfWork.categoryRepository.isExists(s => s.Id == subCategoryCreateDto.CategoryId))
                {
                    throw new CustomException(400, "CategoryId", "this Subcategory doesnt exist");
                }
                if (await _unitOfWork.subCategoryRepository.isExists(s => s.Name.ToLower() == subCategoryCreateDto.Name.ToLower()))
                {
                    throw new CustomException(400, "Name", "this Subcategory name already exists");
                }
                var subCategory = _mapper.Map<SubCategory>(subCategoryCreateDto);
                var existingBrands = new List<Brand>();
                foreach (var brandId in subCategoryCreateDto.BrandIds)
                {
                    var brand = await _unitOfWork.brandRepository.GetEntity(b => b.Id == brandId);
                    if (brand == null)
                    {
                        throw new CustomException(400, "BrandId", $"Brand with ID {brandId} does not exist");
                    }
                    existingBrands.Add(brand);
                }
                subCategory.Brands = existingBrands;

                await _unitOfWork.subCategoryRepository.Create(subCategory);
                _unitOfWork.Commit();
                return subCategory;
        }
        public async Task<int> Delete(int? id)
        {
            if(id == null) throw new CustomException(400, "Id", "id cant be null");
            var SubCategory = await _unitOfWork.subCategoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if(SubCategory is null) throw new CustomException(400, "Not found");
            if (!string.IsNullOrEmpty(SubCategory.Image))
            {
                SubCategory.Image.DeleteFile();
            }
            await _unitOfWork.subCategoryRepository.Delete(SubCategory);
            _unitOfWork.Commit();
            return SubCategory.Id;
        }
        public async Task<PaginatedResponse<SubCategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = (await _unitOfWork.subCategoryRepository.GetAll()).Count();
            var subCategories = await _unitOfWork.subCategoryRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
   {
        query => query.Include(s=>s.Products).Include(s=>s.Brands).Include(s=>s.Category)
   }
);
            var subCategoriesWithMapping = _mapper.Map<List<SubCategoryListItemDto>>(subCategories);
            var paginatedResult = new PaginatedResponse<SubCategoryListItemDto>
            {
                Data = subCategoriesWithMapping,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return paginatedResult;

        }
    }
}
