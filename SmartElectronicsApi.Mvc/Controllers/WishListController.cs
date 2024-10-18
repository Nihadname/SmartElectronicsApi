using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Basket;
using SmartElectronicsApi.Mvc.ViewModels.WishList;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class WishListController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/WishList");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserWishListItemVM>(ContentStream);
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
                    ? $"http://localhost:5246/api/WishList?productId={productId}&variationId={variationId}"
                    : $"http://localhost:5246/api/WishList?productId={productId}";

                using HttpResponseMessage httpResponseMessage = await client.PostAsync(url, null);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                    // If you expect a specific number from the API (e.g., basket count), you can parse it
                    int WishProductCount = int.Parse(responseBody);
                   

                    return Json(new { success = true, message = "Product added to the wishList.", WishProductCount });
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
        [HttpPost]
      
        public async Task<IActionResult> Delete(int? productId, int? variationId = null)
        {
            using var client = new HttpClient();

            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = variationId.HasValue
                ? $"http://localhost:5246/api/WishList/?productId={productId}&variationId={variationId}"
                : $"http://localhost:5246/api/WishList/?productId={productId}";
            using HttpResponseMessage httpResponseMessage = await client.DeleteAsync(url);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                // If you expect a specific number from the API (e.g., basket count), you can parse it
                int WishProductCount = int.Parse(responseBody);

                return Json(new { success = true, message = "wisglist product Deleted successfully.",WishProductCount });
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Json(new { success = false, message = "User has not authozrized yet" });
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else
            {
                return Json(new { success = false, message = "An error occurred while changing the quantity." });
            }

        }
    }
}
