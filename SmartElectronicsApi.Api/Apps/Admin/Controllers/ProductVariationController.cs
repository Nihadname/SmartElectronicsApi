using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.ProductVariation;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariationController : ControllerBase
    {
        private readonly IProductVariationService productVariationService;

        public ProductVariationController(IProductVariationService productVariationService)
        {
            this.productVariationService = productVariationService;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await productVariationService.Delete(id));
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductVariationCreateDto productVariationCreateDto)
        {
            return Ok(await productVariationService.Create(productVariationCreateDto));
        }
    }
}
