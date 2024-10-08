using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryCreateDto categoryCreateDto)
        {
                return Ok(await categoryService.Create(categoryCreateDto));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> get(int pageNumber, int pageSize)
        {
            try
            {
                return Ok(await categoryService.GetAllForAdmin(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message,ex);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await categoryService.Delete(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int? id,CategoryUpdateDto categoryUpdateDto)
        {
           
                return Ok(await categoryService.Update(id, categoryUpdateDto));
            
                        
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await categoryService.GetById(id));
        }

    }
}
