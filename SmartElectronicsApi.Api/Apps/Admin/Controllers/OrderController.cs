using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Order;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await orderService.GetAll(pageNumber, pageSize));
        }
        [HttpPut("AcceptanceOfBeingShipped/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        public async Task<IActionResult> ShippingOrder(int id)
        {
            return Ok(await orderService.ShippingOrder(id));
        }
        [HttpPost("VerifyOrder")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> VerifyDelivery(OrderVerifyDto orderVerifyDto)
        {
            return Ok(await orderService.VerifyOrderAsDelivered(orderVerifyDto));
        }
        [HttpGet("{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Get(int? Id)
        {
            return Ok(await orderService.GetById(Id));
        }
        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async  Task<IActionResult> Delete(int? id)
        {
            if (id == null) throw new CustomException(400, "Id", "it can not be null");
            return Ok(await orderService.Delete(id));
        }
       
    }
}
