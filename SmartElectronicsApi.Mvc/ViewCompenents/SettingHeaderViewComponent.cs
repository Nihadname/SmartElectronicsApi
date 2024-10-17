using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Basket;
using SmartElectronicsApi.Mvc.ViewModels.WishList;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

public class SettingHeaderViewComponent : ViewComponent
{
    public SettingHeaderViewComponent()
    {
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var token = HttpContext.Request.Cookies["jwtToken"]; // Access Request via HttpContext
        ViewBag.ExistenceOfIcon = false;

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);

                var expirationClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (!string.IsNullOrEmpty(expirationClaim) && long.TryParse(expirationClaim, out var expTimestamp))
                {
                    var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (expirationDate < DateTime.UtcNow)
                    {
                        HttpContext.Response.Cookies.Delete("jwtToken");
                        ViewBag.ExistenceOfIcon = false;
                        ViewBag.FullName = null;
                        ViewBag.Count = 0;
                        ViewBag.CountWish = 0;
                        return View();
                    }
                }

                ViewBag.ExistenceOfIcon = true;

                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value
                               ?? jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

                if (!string.IsNullOrEmpty(fullName))
                {
                    ViewBag.FullName = fullName;
                }

                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5246/api/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Basket");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<UserBasketGetVM>(contentStream);
                    var count = data.BasketProducts.Sum(item => item.Quantity);
                    ViewBag.Count = count;
                    using HttpResponseMessage httpResponseMessage2 = await client.GetAsync("http://localhost:5246/api/WishList");
                    string contentStream2 = await httpResponseMessage2.Content.ReadAsStringAsync();

                    var data2 = JsonConvert.DeserializeObject<UserWishListItemVM>(contentStream2);
                    var count2 = data2.Count;
                    ViewBag.CountWish = count2;
                }
            }
            catch (Exception)
            {
                HttpContext.Response.Cookies.Delete("jwtToken");
                ViewBag.ExistenceOfIcon = false;
                ViewBag.Count = 0;
                ViewBag.CountWish = 0;
                return View();
            }
        }
        else
        {
            ViewBag.Count = 0;
            ViewBag.CountWish = 0;
        }

        return View();
    }
}
