using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Exceptions;
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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductCreateDto> Create(ProductCreateDto productCreateDto)
        {
            if (await _unitOfWork.productRepository.isExists(s => s.Name.ToLower() == productCreateDto.Name.ToLower()))
            {
                throw new CustomException(400, "Name", "This product name already exists.");
            }

            
            var category = await _unitOfWork.categoryRepository.GetEntity(s => s.Id == productCreateDto.CategoryId, includes: new Func<IQueryable<Category>, IQueryable<Category>>[]
            {
        query => query.Include(p => p.SubCategories)
            });
            if (category == null)
            {
                throw new CustomException(400, "Category", "Invalid category selected.");
            }

            
            var subcategory = await _unitOfWork.subCategoryRepository.GetEntity(s => s.Id == productCreateDto.SubcategoryId, includes: new Func<IQueryable<SubCategory>, IQueryable<SubCategory>>[]
            {
        query => query.Include(sc => sc.brandSubCategories) 
            });
            if (subcategory == null)
            {
                throw new CustomException(400, "SubCategory", "Invalid subcategory selected.");
            }

            
            if (!category.SubCategories.Any(sc => sc.Id == productCreateDto.SubcategoryId))
            {
                throw new CustomException(400, "SubCategory", "The selected subcategory does not belong to the chosen category.");
            }

            if (!subcategory.brandSubCategories.Any(b => b.Id == productCreateDto.BrandId))
            {
                throw new CustomException(400, "Brand", "The selected brand is not associated with the chosen subcategory.");
            }

            if (productCreateDto.DiscountPercentage.HasValue)
            {
                productCreateDto.DiscountedPrice = productCreateDto.Price - (productCreateDto.Price * productCreateDto.DiscountPercentage.Value) / 100;
            }

           
            productCreateDto.ProductCode = productCreateDto.Name.Length >= 5
                ? productCreateDto.Name.Substring(0, 5) + Guid.NewGuid().ToString().Substring(0, 15)
                : productCreateDto.Name + Guid.NewGuid().ToString().Substring(0, 15);

           
            var mappedProduct = _mapper.Map<Product>(productCreateDto);
            await _unitOfWork.productRepository.Create(mappedProduct);
             _unitOfWork.Commit();

            return productCreateDto;
        }
        public async Task<PaginatedResponse<ProdutListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var TotalCount = (await _unitOfWork.productRepository.GetAll(s => s.IsDeleted == false)).Count();
             var products=await _unitOfWork.productRepository.GetAll(s => s.IsDeleted == false, (pageNumber - 1) * pageSize, pageSize, includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
    {
        query => query.Include(p => p.Category)
    });
            var MappedProducts=_mapper.Map<List<ProdutListItemDto>>(products);
            return new PaginatedResponse<ProdutListItemDto>
            {
                Data = MappedProducts,
                TotalRecords = TotalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

        }
        public async Task<ProductReturnDto> GetById(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (product is null) throw new CustomException(404, "Not found");
            var MappedProduct=_mapper.Map<ProductReturnDto>(product);
            return MappedProduct;
        }
        public async Task<int> Delete(int? id)
        {
            if (id is null) throw new CustomException(400, "Id", "id cant be null");
            var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
            if (product is null) throw new CustomException(404, "Not found");
            await _unitOfWork.productRepository.Delete(product);
            _unitOfWork.Commit();
            return product.Id;
        }
    }
}
