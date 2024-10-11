using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.ProductVariation;
using System.Net;
using System.Net.Http.Headers;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ProductVariationController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/ProductVariation?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ProductVariationListItemVM>>(ContentStream);
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
        public async Task<IActionResult> Delete(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.DeleteAsync($"http://localhost:5246/api/ProductVariation/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "ProductVariation successfully deleted." });
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    return Json(new { success = false, errors = errorResponse.Errors });
                }
                else
                {
                    return Json(new { success = false, message = errorResponse?.Message ?? "An unknown error occurred." });
                }
            }
        }
        public async Task<IActionResult> Create()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage ColorResponse = await client.GetAsync("http://localhost:5246/api/Color/GetAll");
            if (ColorResponse.IsSuccessStatusCode)
            {
                string contentStream = await ColorResponse.Content.ReadAsStringAsync();
                var Colors = JsonConvert.DeserializeObject<List<ColorListItemVM>>(contentStream);
                ViewBag.Colors = new SelectList(Colors, "Id", "Name");
            }
            using HttpResponseMessage productResponse = await client.GetAsync("http://localhost:5246/api/Product/GetAll");
            if (productResponse.IsSuccessStatusCode)
            {

                string contentStream = await productResponse.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(contentStream);
                ViewBag.products = new SelectList(products, "Id", "Name");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductVariationLCreateVM productVariationLCreateVM)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View(productVariationLCreateVM);
            }
            using var content = new MultipartFormDataContent();

            // Validate product properties before adding to content
            if (string.IsNullOrEmpty(productVariationLCreateVM.VariationName))
            {
                ModelState.AddModelError("VariationName", "Product name is required.");
            }
            if (ModelState.IsValid)
            {
                content.Add(new StringContent(productVariationLCreateVM.VariationName), "VariationName");
                content.Add(new StringContent(productVariationLCreateVM.Price.ToString()), "Price");
                content.Add(new StringContent(productVariationLCreateVM.DiscountPercentage?.ToString() ?? "0"), "DiscountPercentage");
                content.Add(new StringContent(productVariationLCreateVM.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(productVariationLCreateVM.ProductId.ToString()), "ProductId");
                if (productVariationLCreateVM.ColorIds != null)
                {
                    foreach (var colorId in productVariationLCreateVM.ColorIds)
                    {
                        content.Add(new StringContent(colorId.ToString()), "ColorIds");
                    }
                }
                if (productVariationLCreateVM.Images != null && productVariationLCreateVM.Images.Count > 0)
                {
                    foreach (var image in productVariationLCreateVM.Images)
                    {
                        if (image.Length > 0)
                        {
                            var stream = image.OpenReadStream();
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                            content.Add(fileContent, "Images", image.FileName);
                        }
                    }
                }


            }
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/ProductVariation", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductVariation");
            }
            else
            {
                var errorResponseString = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);
                if (errorResponse?.Errors != null)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                // Reload necessary data for the view again
                await LoadViewBagData();
                return View(productVariationLCreateVM);
            }

        }
        private async Task LoadViewBagData()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage colorResponse = await client.GetAsync("http://localhost:5246/api/Color/GetAll");
            if (colorResponse.IsSuccessStatusCode)
            {
                string colorContentStream = await colorResponse.Content.ReadAsStringAsync();
                var colors = JsonConvert.DeserializeObject<List<ColorListItemVM>>(colorContentStream);
                ViewBag.Colors = new SelectList(colors, "Id", "Name");
            }
            using HttpResponseMessage productResponse = await client.GetAsync("http://localhost:5246/api/Product/GetAll");
            if (productResponse.IsSuccessStatusCode)
            {

                string contentStream = await productResponse.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProdutListItemVM>>(contentStream);
                ViewBag.products = new SelectList(products, "Id", "Name");
            }
        }
    }


}