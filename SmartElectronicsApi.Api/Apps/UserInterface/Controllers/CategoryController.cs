using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
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
        [HttpGet("GetAllForUserInterface")]
        public async Task<IActionResult> GetAllForUserInterface(int skip, int take)
        {
            try
            {
                return Ok(await categoryService.GetAllForUserInterface(skip, take));

            }
            catch (Exception ex)
            {
             throw new Exception(ex.Message);
            }



        }
    }
}
