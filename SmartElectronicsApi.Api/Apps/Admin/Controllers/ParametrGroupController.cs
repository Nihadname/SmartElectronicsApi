using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.ParametrGroup;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParametrGroupController : ControllerBase
    {
        private readonly IParametrGroupService _parametrGroupService;

        public ParametrGroupController(IParametrGroupService parametrGroupService)
        {
            _parametrGroupService = parametrGroupService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ParametrGroupCreateDto parametrGroupCreateDto)
        {
           
                return Ok( await _parametrGroupService.Create(parametrGroupCreateDto));
            
            
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await _parametrGroupService.Delete(id));
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _parametrGroupService.GetAll(pageNumber, pageSize));
        }
    }
}
