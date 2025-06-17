using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Order;
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
        public async Task<IActionResult> Create()
        {

            var sessionId = await orderService.CreateStripeCheckoutSessionAsync();
            return Ok(new { id = sessionId });
        }
        [HttpGet("verify-payment")]
        public async Task<IActionResult> VerifyPayment(string sessionId)
        {
            var result = await orderService.VerifyPayment(sessionId);

            if (result == "Payment successful")
            {
                return Redirect("https://localhost:7170/Success/PaymentSuccess");
            }
            else
            {
                return RedirectToAction("https://localhost:7170/Cancel");
            }
        }
       
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllForUser")]
        public async Task<IActionResult> GetAllForUser(int pageNumber = 1, int pageSize = 2)
        {
            return Ok(await orderService.GetAllForUser(pageNumber, pageSize));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("DeleteForUser/{id}")]
        public async Task<IActionResult> DeleteForUser(int id)
        {
            return Ok(await orderService.DeleteOrderForUser(id));
        }
    }
}
