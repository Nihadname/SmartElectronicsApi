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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Create( ProductCreateDto productCreateDto)
        {
           
                return Ok(await _productService.Create(productCreateDto));
            
            
        }
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Update(int productId, ProductUpdateDto productUpdateDto)
        {
            return Ok(await _productService.Update(productId, productUpdateDto));
        }
        [HttpPatch]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> MakeMain(int productId, int imageId)
        {
            var method =  _productService.MakeMain(productId, imageId);
            return Ok(method);
        }
        [HttpDelete("Color/{colorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteColorOfProduct(int productId, int colorId)
        {
            return Ok( _productService.DeleteColorOfProduct(productId, colorId));
        }
        [HttpGet("GetAllForSelect")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetAllForSelect()
        {
            return Ok(await _productService.GetSelectProducts());
        }

    }
}
