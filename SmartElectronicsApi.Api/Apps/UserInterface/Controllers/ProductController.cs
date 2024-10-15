using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("GetTheNewOnes")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productService.GetAllNewOnes());
        }
        [HttpGet("TheMostViewed")]
        public async Task<IActionResult> GetTheMostViewed()
        {
            return Ok(await _productService.GetAllWithTheMostViews());
        }
        [HttpGet("WithDiscount")]
        public async Task<IActionResult> GetAllWithDiscount()
        {
            return Ok(await _productService.GetAllWithDiscounted());
        }
        [HttpGet("DealOfTheWeeks")]
        public async Task<IActionResult> GetAllDealOfTheWeeks()
        {
            return Ok(await _productService.GetDealOfThisWeek());
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Search(int pageNumber = 1,
    int pageSize = 10,
    string searchQuery = null,
    string sortBy = "Name", // Default sort property
    string sortOrder = "asc")
        {
            return Ok(await _productService.Search(pageNumber, pageSize, searchQuery, sortBy, sortOrder));
        }
        [HttpGet("GetDealOfTheWeekInBrand/{brandId}")]
        public async Task<IActionResult> GetDealOfTheWeekInBrand(int? brandId)
        {
            return Ok(await _productService.GetDealOfTheWeekInBrand(brandId));
        }
        [HttpGet("GetAllProductsWithBrandId/{brandId}")]
        public async Task<IActionResult> GetAllProductsWithBrandId(int? brandId)
        {
            return Ok(await _productService.GetAllProductsWithBrandId(brandId));
        }
        [HttpGet("GetProductsByCategoryIdAndBrandId")]
        public async Task<IActionResult> GetProductsByCategoryIdAndBrandId(int? categoryId, int? BrandId, int excludeProductId)
        {
            return Ok(await _productService.GetProductsByCategoryIdAndBrandId(categoryId, BrandId, excludeProductId));

        }
        [HttpGet("Filter")]

        public async Task<IActionResult> GetFilteredProducts(
    [FromQuery] int? categoryId = null,
    [FromQuery] int? subCategoryId = null,
    [FromQuery] int? brandId = null,
    [FromQuery] List<int>? colorIds = null,  
    [FromQuery] int? minPrice = null, 
    [FromQuery] int? maxPrice = null,  
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string sortOrder = "asc")
        {
            var result = await _productService.GetFilteredProducts(categoryId, subCategoryId, brandId, colorIds, minPrice, maxPrice, pageNumber, pageSize, sortOrder);
            return Ok(result);
        }



    }
}
