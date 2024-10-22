using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("checkout-session")]
        public async Task<IActionResult> Create(int addressId)
        {

            var sessionId = await orderService.CreateStripeCheckoutSessionAsync(addressId);
            return Ok(new { id = sessionId });
        }
        [HttpGet("verify-payment")]
        public async Task<IActionResult> VerifyPayment(string sessionId)
        {
            var result = await orderService.VerifyPayment(sessionId);

            if (result == "Payment successful")
            {
                // Redirect the user to a success page
                return Redirect("https://localhost:7170/Success/PaymentSuccess");
            }
            else
            {
                // If the payment failed, redirect to a failure page or show an error message
                return RedirectToAction("https://localhost:7170/Cancel");
            }
        }
    }
}
