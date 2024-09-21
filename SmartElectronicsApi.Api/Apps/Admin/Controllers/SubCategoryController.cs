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
    }
}
