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
                throw new CustomException(400, "CategoryId", "This category doesn't exist");
            }

            if (await _unitOfWork.subCategoryRepository.isExists(s => s.Name.ToLower() == subCategoryCreateDto.Name.ToLower()))
            {
                throw new CustomException(400, "Name", "This subcategory name already exists");
            }

            var subCategory = _mapper.Map<SubCategory>(subCategoryCreateDto);

            var brandSubCategories = new List<BrandSubCategory>();

            foreach (var brandId in subCategoryCreateDto.BrandIds)
            {
                var brand = await _unitOfWork.brandRepository.GetEntity(b => b.Id == brandId);
                if (brand == null)
                {
                    throw new CustomException(400, "BrandId", $"Brand with ID {brandId} does not exist");
                }
                brandSubCategories.Add(new BrandSubCategory
                {
                    BrandId = brandId,
                    SubCategory = subCategory
                });
            }

            subCategory.brandSubCategories = brandSubCategories;

            await _unitOfWork.subCategoryRepository.Create(subCategory);
            _unitOfWork.Commit();
            return subCategory;
        }
        public async Task<int> Delete(int? id)
        {
            if (id == null) throw new CustomException(400, "Id", "Id can't be null");

            var subCategory = await _unitOfWork.subCategoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
            {
        query => query.Include(s => s.brandSubCategories)
            });

            if (subCategory is null) throw new CustomException(400, "Not found");

            subCategory.brandSubCategories.Clear();

            if (!string.IsNullOrEmpty(subCategory.Image))
            {
                subCategory.Image.DeleteFile();
            }

            await _unitOfWork.subCategoryRepository.Delete(subCategory);
            _unitOfWork.Commit();
            return subCategory.Id;
        }
        public async Task<SubCategoryReturnDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "Id can't be null");

            var subCategory = await _unitOfWork.subCategoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
            {
        query => query.Include(s => s.brandSubCategories).ThenInclude(bs => bs.Brand).Include(s => s.Products).Include(s => s.Category)
            });

            if (subCategory is null)
            {
                throw new CustomException(404, "Not found");
            }

            var subCategoryWithMapping = _mapper.Map<SubCategoryReturnDto>(subCategory);
            return subCategoryWithMapping;
        }

        public async Task<PaginatedResponse<SubCategoryListItemDto>> GetAllForAdmin(int pageNumber = 1, int pageSize = 10)
        {
            var totalCount = (await _unitOfWork.subCategoryRepository.GetAll()).Count();
            var subCategories = await _unitOfWork.subCategoryRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
            {
        query => query.Include(s => s.brandSubCategories).ThenInclude(bs => bs.Brand).Include(s => s.Products).Include(s => s.Category)
            });

            var subCategoriesWithMapping = _mapper.Map<List<SubCategoryListItemDto>>(subCategories);
            var paginatedResult = new PaginatedResponse<SubCategoryListItemDto>
            {
                Data = subCategoriesWithMapping,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return paginatedResult;
        }

        public async Task<int> Update(int? id, SubCategoryUpdateDto subCategoryUpdateDto)
        {
            if (id is null) throw new CustomException(400, "Id", "Id can't be null");

            var subCategory = await _unitOfWork.subCategoryRepository.GetEntity(s => s.Id == id && s.IsDeleted == false, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
            {
        query => query.Include(s => s.brandSubCategories)
            });

            if (subCategory is null) throw new CustomException(404, "Not found");

            if (!string.IsNullOrWhiteSpace(subCategoryUpdateDto.Name))
            {
                if (await _unitOfWork.subCategoryRepository.isExists(s => s.Name.ToLower() == subCategoryUpdateDto.Name.ToLower() && s.Id != id))
                {
                    throw new CustomException(400, "Name", "This subcategory name already exists");
                }

            }


            if (subCategoryUpdateDto.CategoryId.HasValue && subCategoryUpdateDto.CategoryId.Value > 0)
            {
                if (!await _unitOfWork.categoryRepository.isExists(s => s.Id == subCategoryUpdateDto.CategoryId.Value))
                {
                    throw new CustomException(400, "CategoryId", "This category doesn't exist");
                }
                subCategory.CategoryId = subCategoryUpdateDto.CategoryId.Value;
            }

            _mapper.Map(subCategoryUpdateDto, subCategory);

            if (subCategoryUpdateDto.formFile != null)
            {
                if (!string.IsNullOrEmpty(subCategory.Image))
                {
                    subCategory.Image.DeleteFile();
                }
                subCategory.Image = subCategoryUpdateDto.formFile.Save(Directory.GetCurrentDirectory(), "img");
            }

            if (subCategoryUpdateDto.BrandIds != null)
            {
                subCategory.brandSubCategories.Clear();

                foreach (var brandId in subCategoryUpdateDto.BrandIds)
                {
                    var brand = await _unitOfWork.brandRepository.GetEntity(b => b.Id == brandId);
                    if (brand == null)
                    {
                        throw new CustomException(400, "BrandId", $"Brand with ID {brandId} does not exist");
                    }

                    subCategory.brandSubCategories.Add(new BrandSubCategory
                    {
                        BrandId = brandId,
                        SubCategory = subCategory
                    });
                }
            }

            await _unitOfWork.subCategoryRepository.Update(subCategory);
            _unitOfWork.Commit();
            return subCategory.Id;
        }
        public async Task<List<SubCategoryListItemDto>> GetAll()
        {
           var SubCategories =await _unitOfWork.subCategoryRepository.GetAll(s=>s.IsDeleted==false);
            var MappedSubCategories = _mapper.Map<List<SubCategoryListItemDto>>(SubCategories);
            return MappedSubCategories;
        }
    }
}