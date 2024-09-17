using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SliderController : ControllerBase
    {
        public ISliderService _sliderService { get; set; }

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }
        [HttpGet("GetSliderForUi/{skip}/{take}")]
        public async Task<IActionResult> GetSliderForUi(int skip, int take)
        {
        return Ok(await _sliderService.GetAll(skip, take));
        }
    }
}
