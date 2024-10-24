using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Product;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
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

        [HttpPost]
        public async Task<IActionResult> Create( ProductCreateDto productCreateDto)
        {
           
                return Ok(await _productService.Create(productCreateDto));
            
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            
                return Ok(await _productService.Delete(id));
          
           
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10, string searchQuery = null,
           int? categoryId = null)
        {
            return Ok(await _productService.GetAll(pageNumber, pageSize,searchQuery,categoryId));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _productService.GetById(id));
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productService.Get());
        }
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, ProductUpdateDto productUpdateDto)
        {
            return Ok(await _productService.Update(productId, productUpdateDto));
        }
    }
}
