using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.GuestOrder;
using SmartElectronicsApi.Application.Exceptions;
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
        [HttpGet("VerifyGuestOrder/{id}")]
        public async Task<IActionResult> VerifyGuestOrder(int? id)
        {
            if (id == null)
            {
                return BadRequest("Invalid order ID.");
            }

            var result = await _guestOrderService.VerifyEmail(id);

            if (result == "it is completed")
            {
                return Redirect("https://localhost:7170/");
            }

            throw new CustomException(400, "GuestOrder", "GuestOrder failed");
        }

    }
}
