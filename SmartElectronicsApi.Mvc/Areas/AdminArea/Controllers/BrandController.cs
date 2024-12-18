using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class BrandController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Brand?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<BrandAdminReturnVM>>(ContentStream);
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
        public async  Task<IActionResult> Create()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            using HttpResponseMessage AuthCheck = await client.GetAsync("http://localhost:5246/api/Auth/CheckAdmin");
            if (AuthCheck.IsSuccessStatusCode)
            {

            }
            else if (AuthCheck.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else if (AuthCheck.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "" });
            }
            else
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BrandCreateVM BrandCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(BrandCreateVM);
            } 
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(BrandCreateVM.Name), "Name");
            content.Add(new StringContent(BrandCreateVM.Description), "Description");
            if (BrandCreateVM.formFile != null)
            {
                var fileContent = new StreamContent(BrandCreateVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(BrandCreateVM.formFile.ContentType);
                content.Add(fileContent, nameof(BrandCreateVM.formFile), BrandCreateVM.formFile.FileName);
            }
            var response = await client.PostAsync("Brand", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
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
                return View(BrandCreateVM);
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
                // Return a JSON response indicating that the user is not authenticated
                return Json(new { success = false, message = "User not authenticated." });
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await client.DeleteAsync($"http://localhost:5246/api/Brand/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Branf successfully deleted." });
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
        public async Task<IActionResult> Update(int? id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Brand/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<BrandUpdateVM>(ContentStream);
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
        public async Task<IActionResult> Update(int? id, BrandUpdateVM brandUpdateVM)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);

            var content = new MultipartFormDataContent();


            content.Add(new StringContent(brandUpdateVM.Name ?? string.Empty), "Name");

            // Add Description (even if null or empty)
            content.Add(new StringContent(brandUpdateVM.Description ?? string.Empty), "Description");

            // Add file if available
            if (brandUpdateVM.formFile != null)
            {
                var fileContent = new StreamContent(brandUpdateVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(brandUpdateVM.formFile.ContentType);
                content.Add(fileContent, nameof(brandUpdateVM.formFile), brandUpdateVM.formFile.FileName);
            }


            var response = await client.PutAsync($"Brand/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorResponseString = await response.Content.ReadAsStringAsync();
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
                }

            return View(brandUpdateVM);

        }
    }
}
