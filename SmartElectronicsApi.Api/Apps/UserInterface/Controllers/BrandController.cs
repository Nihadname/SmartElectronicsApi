using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet("GetForUi")]
        public async Task<IActionResult> Get(int skip, int take)
        {
            return Ok(await _brandService.getAllForUi(skip, take));
        }
        [HttpGet("Ui/{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _brandService.GetById(id));
        }
    }
}
