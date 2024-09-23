using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Setting;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingService settingService;

        public SettingController(ISettingService settingService)
        {
            this.settingService = settingService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SettingDto settingDto)
        {
            return Ok(await settingService.Create(settingDto));
        }
       [HttpGet]
       public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await settingService.GetForAdminUi(pageNumber, pageSize));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int? id)
        {
            return Ok(await settingService.GetById(id));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            return Ok(await settingService.Delete(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int? id, [FromQuery] SettingUpdateDto settingUpdateDto)
        {
            return Ok(await settingService.Update(id, settingUpdateDto));
        }
    }
}
