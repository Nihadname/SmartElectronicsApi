using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Extensions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.DataAccess.Data;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
       // private readonly SmartElectronicsDbContext _context;


        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ProductReturnDto> Create(ProductCreateDto productCreateDto)
        {
            // Check if product name already exists
            
                if (await _unitOfWork.productRepository.isExists(s => s.Name.ToLower() == productCreateDto.Name.ToLower()))
                {
                    throw new CustomException(400, "Name", "This product name already exists.");
                }

                // Validate category
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
        query => query.Include(sc => sc.brandSubCategories).ThenInclude(s=>s.Brand)
                });
                if (subcategory == null)
                {
                    throw new CustomException(400, "SubCategory", "Invalid subcategory selected.");
                }


                if (!category.SubCategories.Any(sc => sc.Id == productCreateDto.SubcategoryId))
                {
                    throw new CustomException(400, "SubCategory", "The selected subcategory does not belong to the chosen category.");
                }


                if (!subcategory.brandSubCategories.Select(s=>s.Brand).Any(s=>s.Id==productCreateDto.BrandId))
                {
                var availableBrands = subcategory.brandSubCategories.Select(b => b.Id).ToList();
                throw new CustomException(400, "Brand", $"The selected brand ID {productCreateDto.BrandId} is not associated with the chosen subcategory. Available brands: {string.Join(", ", availableBrands)}");
            }

            if (productCreateDto.DiscountPercentage.HasValue&&productCreateDto.DiscountPercentage!=0)
                {
                    productCreateDto.DiscountedPrice = productCreateDto.Price - (productCreateDto.Price * productCreateDto.DiscountPercentage.Value) / 100;
                }
            if (productCreateDto.DiscountedPrice == null)
            {
                productCreateDto.DiscountedPrice = 0;
            }

                productCreateDto.ProductCode = productCreateDto.Name.Length >= 5
                    ? productCreateDto.Name.Substring(0, 5) + Guid.NewGuid().ToString().Substring(0, 15)
                    : productCreateDto.Name + Guid.NewGuid().ToString().Substring(0, 15);


                foreach (var colorId in productCreateDto.ColorIds)
                {
                    if (!await _unitOfWork.colorRepository.isExists(s => s.Id == colorId))
                    {
                        throw new CustomException(400, "Color", "Invalid color selected.");
                    }
                }
                productCreateDto.CreatedTime=DateTime.Now;
                var mappedProduct = _mapper.Map<Product>(productCreateDto);
               
                await _unitOfWork.productRepository.Create(mappedProduct);

                _unitOfWork.Commit();
                foreach (var item in productCreateDto.ColorIds)
                {
                    var productColor = new ProductColor
                    {
                        ProductId = mappedProduct.Id,
                        ColorId = item
                    };
                    await _unitOfWork.ProductColorRepository.Create(productColor);
                    _unitOfWork.Commit();
                }
                bool isFirstImage = true;
                foreach (var item in productCreateDto.Images)
                {
                    var imagePath = item.Save(Directory.GetCurrentDirectory(), "img");
                    var productImage = new ProductImage
                    {
                        Name = Path.GetFileName(imagePath),
                        IsMain = isFirstImage, 
                        ProductId = mappedProduct.Id,
                    };
                    await _unitOfWork.ProductImageRepository.Create(productImage);

                    isFirstImage = false;
                }


                _unitOfWork.Commit();
            var product=await GetById(mappedProduct.Id);
                return product;
            
            
        }
        public async Task<PaginatedResponse<ProdutListItemDto>> GetAll(
           int pageNumber = 1,
           int pageSize = 10,
           string searchQuery = null,
           int? categoryId = null)
        {
            var ProductQuery = await _unitOfWork.productRepository.GetQuery(s => s.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                ProductQuery = ProductQuery.Where(s => s.Name.Contains(searchQuery) || s.Description.Contains(searchQuery));
            }

            if (categoryId.HasValue)
            {
                ProductQuery = ProductQuery.Where(p => p.CategoryId == categoryId);
            }

            var TotalCount = await ProductQuery.CountAsync();

            var products = await ProductQuery
                .Include(p => p.Category)
                .Include(s => s.productImages)
                .Include(s => s.productColors).ThenInclude(s => s.Color)
                .Include(s => s.parametricGroups).ThenInclude(s => s.parametrValues)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);

            
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
            var product = await _unitOfWork.productRepository.GetEntity(
    s => s.Id == id && s.IsDeleted == false,
    includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
    {
        query => query
            .Include(p => p.Category)
            .Include(p => p.productImages)
            .Include(p => p.productColors)
                .ThenInclude(pc => pc.Color)
            .Include(p => p.parametricGroups)
                .ThenInclude(pg => pg.parametrValues)
            .Include(p => p.Variations)
                .ThenInclude(v => v.productImages)
            .Include(p => p.Variations)
                .ThenInclude(v => v.productVariationColors)
                    .ThenInclude(pvc => pvc.Color)
                    .Include(s=>s.comments)
                    .ThenInclude(s=>s.AppUser)
    });

            if (product is null) throw new CustomException(404, "Not found");
            product.ViewCount ++;
            await _unitOfWork.productRepository.Update(product);
            _unitOfWork.Commit();   
            var MappedProduct=_mapper.Map<ProductReturnDto>(product);
            return MappedProduct;
        }
        public async Task<int> Delete(int? id)
        {
           
                if (id is null) throw new CustomException(400, "Id", "id cant be null");
                var product = await _unitOfWork.productRepository.GetEntity(s => s.Id == id && s.IsDeleted == false);
                if (product is null) throw new CustomException(404, "Not found");
                var parameterGroups = await _unitOfWork.parametricGroupRepository.GetAll(s => s.ProductId == id);
                if (parameterGroups.Any()||parameterGroups!=null)
                {
                    foreach (var parameterGroup in parameterGroups)
                    {
                        await _unitOfWork.parametricGroupRepository.Delete(parameterGroup);
                        _unitOfWork.Commit();
                    }
                }
                var productImages=await _unitOfWork.ProductImageRepository.GetAll(s => s.ProductId == id);
            if (productImages.Any() || productImages != null)
            {
                foreach(var productImage in productImages)
                {
                    if (!string.IsNullOrEmpty(productImage.Name))
                    {
                        productImage.Name.DeleteFile();
                    }
                    await _unitOfWork.ProductImageRepository.Delete(productImage);
                    _unitOfWork.Commit();
                }
            }
                await _unitOfWork.productRepository.Delete(product);
                    _unitOfWork.Commit();
                    return product.Id;
                
            
            
        }
        public async Task<List<ProdutListItemDto>> GetAllNewOnes()
        {
            var products = await _unitOfWork.productRepository.GetAll(s => s.IsDeleted == false && s.isNew == true, 0,8, includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
 {
        query => query.Include(p => p.Category).Include(s=>s.productImages).Include(s=>s.productColors).ThenInclude(s=>s.Color).Include(s=>s.parametricGroups).ThenInclude(s=>s.parametrValues).Include(s=>s.comments).ThenInclude(s=>s.AppUser)
    .Include(s => s.comments).ThenInclude(s => s.commentImages)
 });

            var MappedProducts=_mapper.Map<List<ProdutListItemDto>>(products);
            return MappedProducts;
        }
        public async Task<List<ProdutListItemDto>> GetAllWithTheMostViews(int top = 8)
        {
            var ProductQuery = await _unitOfWork.productRepository.GetQuery(s => s.IsDeleted == false);

            var products = await ProductQuery
      .Include(p => p.Category)
      .Include(s => s.productImages)
      .Include(s => s.productColors).ThenInclude(s => s.Color)
      .Include(s => s.parametricGroups).ThenInclude(s => s.parametrValues)
      .Include(s=>s.comments).ThenInclude(s=>s.AppUser)
    .Include(s => s.comments).ThenInclude(s => s.commentImages)

      .OrderByDescending(s => s.ViewCount) 
      .Take(top)  
      .ToListAsync();


            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);

            return MappedProducts;
        }
        public async Task<List<ProdutListItemDto>> GetAllWithDiscounted()
        {
            var products = await _unitOfWork.productRepository.GetAll(
                s => (s.DiscountPercentage != null || s.DiscountedPrice != null) && s.IsDeleted == false, // Group the discount conditions
                0,
                8,
                includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
                {
            query => query.Include(p => p.Category)
                          .Include(s => s.productImages)
                          .Include(s => s.productColors).ThenInclude(s => s.Color)
                          .Include(s => s.parametricGroups)
                           .Include(s=>s.comments).ThenInclude(s=>s.AppUser)
    .Include(s => s.comments).ThenInclude(s => s.commentImages)
                }
            );

            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);
            return MappedProducts;
        }
        public async Task<List<ProdutListItemDto>> GetDealOfThisWeek()
        {
            var products=await _unitOfWork.productRepository.GetAll(s=>s.IsDeleted==false&&s.IsDealOfTheWeek==true,
            0,
            8,
            includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
                {
            query => query.Include(p => p.Category)
                          .Include(s => s.productImages)
                          .Include(s => s.productColors).ThenInclude(s => s.Color)
                          .Include(s => s.parametricGroups)
                }
            );
            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);
            return MappedProducts;
        }

        public async Task<PaginatedResponse<ProdutListItemDto>> Search(
    int pageNumber = 1,
    int pageSize = 10,
    string searchQuery = null,
   
    string sortBy = "Name", // Default sort property
    string sortOrder = "asc"
)
        {
            var productQuery = await _unitOfWork.productRepository.GetQuery(s => s.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                productQuery = productQuery.Where(s => s.Name.Contains(searchQuery) || s.Description.Contains(searchQuery));
            }
            
            // Apply sorting
            switch (sortBy.ToLower())
            {
                case "name":
                    productQuery = sortOrder.ToLower() == "desc" ? productQuery.OrderByDescending(p => p.Name) : productQuery.OrderBy(p => p.Name);
                    break;
                case "price":
                    productQuery = sortOrder.ToLower() == "desc" ? productQuery.OrderByDescending(p => p.Price) : productQuery.OrderBy(p => p.Price);
                    break;
                case "createddate":
                    productQuery = sortOrder.ToLower() == "desc" ? productQuery.OrderByDescending(p => p.CreatedTime) : productQuery.OrderBy(p => p.CreatedTime);
                    break;
                case "viewcount":
                    productQuery = sortOrder.ToLower() == "desc" ? productQuery.OrderByDescending(p => p.ViewCount) : productQuery.OrderBy(p => p.ViewCount);
                    break;
                default:
                    productQuery = productQuery.OrderBy(p => p.Name); 
                    break;
            }

            // Pagination
            var totalCount = await productQuery.CountAsync();
            var products = await productQuery
                .Include(p => p.Category)
                .Include(s => s.productImages)
                .Include(s => s.productColors).ThenInclude(s => s.Color)
                .Include(s => s.parametricGroups).ThenInclude(s => s.parametrValues)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map the result to DTOs
            var mappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);

            return new PaginatedResponse<ProdutListItemDto>
            {
                Data = mappedProducts,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<List<ProdutListItemDto>> Get()
        {
            var products = await _unitOfWork.productRepository.GetAll();
            var MappedProducts =_mapper.Map<List<ProdutListItemDto>>(products);
            return MappedProducts;
        }
        public async Task<List<ProdutListItemDto>> GetAllProductsWithBrandId(int? id)
        {
            var products = await _unitOfWork.productRepository.GetAll(s => s.IsDeleted == false  && s.BrandId == id,
         0,
         8,
         includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
             {
            query => query.Include(p => p.Category)
                          .Include(s => s.productImages)
                          .Include(s => s.productColors).ThenInclude(s => s.Color)
                          .Include(s => s.parametricGroups)
             }
         );
            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);
            return MappedProducts;
        }
        public async Task<List<ProdutListItemDto>> GetDealOfTheWeekInBrand(int? brandId)
        {
            var products = await _unitOfWork.productRepository.GetAll(s => s.IsDeleted == false && s.IsDealOfTheWeek == true&&s.BrandId==brandId,
          0,
          8,
          includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
              {
            query => query.Include(p => p.Category)
                          .Include(s => s.productImages)
                          .Include(s => s.productColors).ThenInclude(s => s.Color)
                          .Include(s => s.parametricGroups)
              }
          );
            var MappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);
return MappedProducts;  
        }

        public async Task<List<ProdutListItemDto>> GetProductsByCategoryIdAndBrandId(int? categoryId, int? BrandId, int excludeProductId) {
            var products=await _unitOfWork.productRepository.GetAll(s=>s.CategoryId==categoryId&&s.BrandId==BrandId&&s.IsDeleted==false &&
             s.Id != excludeProductId, includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
              {
            query => query.Include(p => p.Category)
                          .Include(s => s.productImages)
                          .Include(s => s.productColors).ThenInclude(s => s.Color)
                          .Include(s => s.parametricGroups)
              });
            var MappedProducts=_mapper.Map<List<ProdutListItemDto>>(products);
               return MappedProducts;
           
        }
        public async Task<PaginatedResponse<ProdutListItemDto>> GetFilteredProducts(
    int? categoryId,
    int? subCategoryId,
    int? brandId,
    List<int> colorIds,
    int? minPrice,  // Add minPrice parameter
    int? maxPrice,  // A
    int pageNumber,
    int pageSize,
    string sortOrder = "name_asc") // Default to ascending
        {
            // Base query to fetch products
            var query = await _unitOfWork.productRepository.GetQuery(s => s.IsDeleted == false);

            // Apply Category Filter
            if (categoryId.HasValue)
            {
                query = query.Where(s => s.CategoryId == categoryId.Value);
            }

            if (subCategoryId.HasValue)
            {
                query = query.Where(s => s.SubCategoryId == subCategoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(s => s.BrandId == brandId.Value);
            }

            if (colorIds != null && colorIds.Any())
            {
                query = query.Where(s => s.productColors.Any(pc => colorIds.Contains(pc.ColorId)));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(s => (s.DiscountedPrice.HasValue ? s.DiscountedPrice.Value : s.Price) >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => (s.DiscountedPrice.HasValue ? s.DiscountedPrice.Value : s.Price) <= maxPrice.Value);
            }
            switch (sortOrder.ToLower())
            {
                case "asc":
                    query = query.OrderBy(s => s.DiscountedPrice.HasValue ? s.DiscountedPrice : s.Price);
                    break;
                case "desc":
                    query = query.OrderByDescending(s => s.DiscountedPrice.HasValue ? s.DiscountedPrice : s.Price);
                    break;
                case "name_asc":
                    query = query.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(s => s.Name);
                    break;
                case "views":
                    query = query.OrderByDescending(s => s.ViewCount); 
                    break;
                default:
                    query = query.OrderBy(s => s.Name); 
                    break;
            }


            var totalProducts = await query.CountAsync();

            // Apply Pagination and fetch products
            var products = await query
                .Include(p => p.productColors).ThenInclude(pc => pc.Color)
                .Include(p => p.productImages)
                .Include(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Use AutoMapper to map the products to ProductListItemDto
            var mappedProducts = _mapper.Map<List<ProdutListItemDto>>(products);

            // Return paginated response
            return new PaginatedResponse<ProdutListItemDto>
            {
                Data = mappedProducts,
                TotalRecords = totalProducts,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<ProductUpdateDto> Update(int productId, ProductUpdateDto productUpdateDto)
        {
           
            var existingProduct = await _unitOfWork.productRepository.GetEntity(
                p => p.Id == productId,
                includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
                {
            query => query.Include(p => p.productImages)
                          .Include(p => p.productColors)
                });

            if (existingProduct == null)
            {
                throw new CustomException(404, "Product", "Product not found.");
            }

            
          

            if (productUpdateDto.DiscountPercentage != 0)
            {
                existingProduct.DiscountedPrice = productUpdateDto.Price - (productUpdateDto.Price * productUpdateDto.DiscountPercentage) / 100;
            }
            if (productUpdateDto.DiscountPercentage.HasValue && productUpdateDto.DiscountPercentage != 0)
            {
                productUpdateDto.DiscountedPrice = productUpdateDto.Price - (productUpdateDto.Price * productUpdateDto.DiscountPercentage.Value) / 100;
            }
            if (productUpdateDto.DiscountedPrice == null)
            {
                productUpdateDto.DiscountedPrice = 0;
            }

            productUpdateDto.ProductCode = productUpdateDto.Name.Length >= 5
                   ? productUpdateDto.Name.Substring(0, 5) + Guid.NewGuid().ToString().Substring(0, 15)
                   : productUpdateDto.Name + Guid.NewGuid().ToString().Substring(0, 15);
  
            existingProduct.productColors.Clear();
            foreach (var colorId in productUpdateDto.ColorIds)
            {
                if (!await _unitOfWork.colorRepository.isExists(s => s.Id == colorId))
                {
                    throw new CustomException(400, "Color", "Invalid color selected.");
                }

                var productColor = new ProductColor
                {
                    ProductId = existingProduct.Id,
                    ColorId = colorId
                };
                existingProduct.productColors.Add(productColor);
            }

            
         if(productUpdateDto.Images != null && productUpdateDto.Images.Any())
            {
                existingProduct.productImages.Clear();
                bool isFirstImage = true;
                foreach (var image in productUpdateDto.Images)
                {
                    var imagePath = image.Save(Directory.GetCurrentDirectory(), "img");
                    var productImage = new ProductImage
                    {
                        Name = Path.GetFileName(imagePath),
                        IsMain = isFirstImage,
                        ProductId = existingProduct.Id,
                    };
                    existingProduct.productImages.Add(productImage);
                    isFirstImage = false;
                }
            }
            else
            {

            }

      
            _mapper.Map(productUpdateDto, existingProduct);

           
            await _unitOfWork.productRepository.Update(existingProduct);
            _unitOfWork.Commit();

            return productUpdateDto;
        }
        public async Task MakeMain(int productId, int imageId)
        {
           
            var product = await _unitOfWork.productRepository.GetEntity(
                p => p.Id == productId && p.IsDeleted == false,
                includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
                {
            query => query.Include(p => p.productImages)
                });

          
            if (product == null)
            {
                throw new CustomException(404, "Product", "Product not found.");
            }

            var image = product.productImages.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
            {
                throw new CustomException(404, "Image", "Image not found.");
            }

       
            foreach (var productImage in product.productImages)
            {
                productImage.IsMain = false;
            }

       
            image.IsMain = true;

          
            await _unitOfWork.productRepository.Update(product);
            _unitOfWork.Commit();
        }
        public async Task DeleteColorOfProduct(int productId, int colorId)
        {
         
            var product = await _unitOfWork.productRepository.GetEntity(
                p => p.Id == productId && p.IsDeleted == false,
                includes: new Func<IQueryable<Product>, IQueryable<Product>>[]
                {
            query => query.Include(p => p.productColors)
                });

          
            if (product == null)
            {
                throw new CustomException(404, "Product", "Product not found.");
            }

           
            var productColor = product.productColors.FirstOrDefault(pc => pc.ColorId == colorId);
            if (productColor == null)
            {
                throw new CustomException(404, "Color", "This product does not have the specified color.");
            }

      
            product.productColors.Remove(productColor);

            await _unitOfWork.productRepository.Update(product);
            _unitOfWork.Commit();
        }
  
    }
}
