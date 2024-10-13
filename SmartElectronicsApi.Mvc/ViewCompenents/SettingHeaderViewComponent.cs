using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Mvc.ViewModels.Basket;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace SmartElectronicsApi.Mvc.ViewCompenents
{
    public class SettingHeaderViewComponent : ViewComponent
    {

        public SettingHeaderViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = Request.Cookies["jwtToken"];
           ViewBag.ExistenceOfIcon=false;

            if (!string.IsNullOrEmpty(token))
            {
                ViewBag.ExistenceOfIcon = true;
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(token);

                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value
                               ?? jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

                if (!string.IsNullOrEmpty(fullName))
                {
                    ViewBag.FullName = fullName;
                }
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5246/api/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
                using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Basket");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<UserBasketGetVM>(ContentStream);
                var Count=  data.BasketProducts.Sum(item => item.Quantity);
                    ViewBag.Count = Count;
                }
                
            }
            else
            {
                ViewBag.Count = 0;
            }
            return View();
        }
    }
}
