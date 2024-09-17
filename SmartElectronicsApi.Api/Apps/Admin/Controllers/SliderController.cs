using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Slider;
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
        public async Task<IActionResult> Get(int skip, int take)
        {
            return Ok(await _sliderService.GetAll(skip,take));

        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int? Id)
        {
            return Ok(await _sliderService.GetById(Id));
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int? Id)
        {
            return Ok(await _sliderService.Delete(Id));
        }
        [HttpPost]
        public async Task<IActionResult> Create( SliderCreateDto sliderCreateDto)
        {
            return Ok(await _sliderService.Create(sliderCreateDto));
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int? Id,SliderUpdateDto sliderUpdateDto)
        {
            return Ok(await _sliderService.Update(Id, sliderUpdateDto));
        }
    }
}
