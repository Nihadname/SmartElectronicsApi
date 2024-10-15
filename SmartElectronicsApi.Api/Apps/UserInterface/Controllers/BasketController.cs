using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;
using System.Net;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public async Task<IActionResult> GetUserBasket()
        {
            return Ok(await _basketService.GetUserBasket());
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Add(int? productId,int? VariationId)
        {
            return Ok(await _basketService.Add(productId,VariationId));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> ChangeQuantity(int? productId, int? variationId = null, int quantityChange = 1)
        {
            return Ok(await _basketService.ChangeQuantity(productId, variationId, quantityChange));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int? productId, int? variationId = null)
        {
            return Ok(await _basketService.Delete(productId, variationId));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            return Ok(await _basketService.DeleteAll());
        }
        [HttpGet("GetUsersWhoAddedProduct")]
        public async Task<IActionResult> GetUsersWhoAddedProduct(int productId, DateTime startDate)
        {
            return Ok(await _basketService.GetUsersWhoAddedProduct(productId, startDate));
        }
    }
}
