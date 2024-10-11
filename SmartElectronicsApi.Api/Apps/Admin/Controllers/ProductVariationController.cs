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
            try
            {
                return Ok(await productVariationService.Create(productVariationCreateDto));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1,
          int pageSize = 10)
        {
            return Ok(await productVariationService.GetAll(pageNumber, pageSize));
        }
    }
}
