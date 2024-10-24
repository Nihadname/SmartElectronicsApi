using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos;
using SmartElectronicsApi.Application.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface IProductService
    {
         Task<ProductCreateDto> Create(ProductCreateDto productCreateDto);
        Task<PaginatedResponse<ProdutListItemDto>> GetAll(int pageNumber = 1, int pageSize = 10, string searchQuery = null,
           int? categoryId = null);
        Task<ProductReturnDto> GetById(int? id);
        Task<int> Delete(int? id);
        Task<List<ProdutListItemDto>> GetAllNewOnes();
        Task<List<ProdutListItemDto>> GetAllWithTheMostViews(int top = 10);
        Task<List<ProdutListItemDto>> GetAllWithDiscounted();
        Task<List<ProdutListItemDto>> GetDealOfThisWeek();
         Task<PaginatedResponse<ProdutListItemDto>> Search(
    int pageNumber = 1,
    int pageSize = 10,
    string searchQuery = null,
    string sortBy = "Name", 
    string sortOrder = "asc"
);
        Task<List<ProdutListItemDto>> Get();
        Task<List<ProdutListItemDto>> GetDealOfTheWeekInBrand(int? brandId);
        Task<List<ProdutListItemDto>> GetAllProductsWithBrandId(int? id);
        Task<List<ProdutListItemDto>> GetProductsByCategoryIdAndBrandId(int? categoryId, int? BrandId, int excludeProductId);
        Task<PaginatedResponse<ProdutListItemDto>> GetFilteredProducts(
       int? categoryId,
       int? subCategoryId,
       int? brandId,
       List<int> colorIds,
       int? minPrice,  // Add minPrice parameter
    int? maxPrice,
       int pageNumber,
       int pageSize,
       string sortOrder = "asc");
        Task<ProductUpdateDto> Update(int productId, ProductUpdateDto productUpdateDto);
        Task MakeMain(int productId, int imageId);
         Task DeleteColorOfProduct(int productId, int colorId);
    }
}
