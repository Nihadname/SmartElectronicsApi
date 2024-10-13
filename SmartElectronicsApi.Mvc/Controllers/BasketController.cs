using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Mvc.ViewModels.Basket;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class BasketController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Basket");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserBasketGetVM>(ContentStream);
                return View(data);
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "" });
            }
            else
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> Add(int? productId, int? variationId)
        {
            try
            {
                using var client = new HttpClient();

                var token = Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var url = variationId.HasValue
                    ? $"http://localhost:5246/api/Basket?productId={productId}&variationId={variationId}"
                    : $"http://localhost:5246/api/Basket?productId={productId}";

                using HttpResponseMessage httpResponseMessage = await client.PostAsync(url, null);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Product added to the basket."});
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }
                else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                else
                {
                    return Json(new { success = false, message = "An error occurred while adding the product." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
