using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestOrderController : ControllerBase
    {
        private readonly IGuestOrderService _guestOrderService;

        public GuestOrderController(IGuestOrderService guestOrderService)
        {
            _guestOrderService = guestOrderService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(GuestOrderCreateDto guestOrderCreateDto)
        {
            return Ok(await _guestOrderService.Create(guestOrderCreateDto));
        }
    }
}
