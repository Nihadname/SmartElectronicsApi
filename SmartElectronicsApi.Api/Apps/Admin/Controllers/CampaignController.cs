using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Campaign;
using SmartElectronicsApi.Application.Dtos.Category;
using SmartElectronicsApi.Application.Implementations;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CreateCampaignDto createCampaignDto)
        {
            return Ok(await _campaignService.CreateCampaign(createCampaignDto));
        }
    }
}
