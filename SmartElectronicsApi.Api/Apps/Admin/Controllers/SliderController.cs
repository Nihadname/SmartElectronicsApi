using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Implementations;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _sliderService.GetAll());

        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int? Id)
        {
            return Ok(await _sliderService.GetById(Id));
        }
    }
}
