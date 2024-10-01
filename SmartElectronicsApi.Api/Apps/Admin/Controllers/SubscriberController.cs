using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Subscriber;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly ISubscriberService _ISubscriberService;

        public SubscriberController(ISubscriberService subCategoryService)
        {
            _ISubscriberService = subCategoryService;
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int? Id)
        {
            return Ok(await _ISubscriberService.Delete(Id));
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _ISubscriberService.GetAllForAdminUi(pageNumber, pageSize));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int?  id,SubscriberDto subscriberDto)
        {
            return Ok(await _ISubscriberService.Update(id, subscriberDto));
        }
    }
}
