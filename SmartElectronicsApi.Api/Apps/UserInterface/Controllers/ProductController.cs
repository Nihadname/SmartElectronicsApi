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
    }
}
