using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Subscriber;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
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

        [HttpPost]
        public async Task<IActionResult> Create(SubscriberDto subscriber)
        {
            return Ok(await _ISubscriberService.Create(subscriber));
        }
    }
}
