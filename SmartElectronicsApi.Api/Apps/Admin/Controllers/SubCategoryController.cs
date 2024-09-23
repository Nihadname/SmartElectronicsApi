using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubCategoryCreateDto subCategoryCreateDto)
        {
                return Ok(await _subCategoryService.Create(subCategoryCreateDto));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await _subCategoryService?.Delete(id));
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber, int pageSize)
        {
            return Ok(await _subCategoryService.GetAllForAdmin(pageNumber,pageSize));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await _subCategoryService.GetById(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int? id , SubCategoryUpdateDto subCategoryUpdateDto)
        {
            return Ok(await _subCategoryService.Update(id, subCategoryUpdateDto));
        }
    }
}
