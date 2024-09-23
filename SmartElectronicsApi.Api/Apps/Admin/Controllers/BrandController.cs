using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
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
        [HttpPost]
        public async Task<IActionResult> Create(BrandCreateDto brandCreateDto)
        {
            return Ok(await _brandService.Create(brandCreateDto));
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _brandService.GetForAdmin(pageNumber, pageSize));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await _brandService.Delete(id));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _brandService.GetById(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int? id, BrandUpdateDto updateDto)
        {
            return Ok(await _brandService.Update(id, updateDto));
        }
    }
}
