using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Net.Http.Headers;
using System.Net;
using SmartElectronicsApi.Mvc.ViewModels.SubCategory;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.Helpers;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SubCategoryController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/SubCategory?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<SubCategoryListItemVM>>(ContentStream);
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
        public async Task<IActionResult> Create()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);


            using HttpResponseMessage categoryResponse = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface?skip=0&take=35");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string contentStream = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            using HttpResponseMessage BrandResponse = await client.GetAsync("http://localhost:5246/api/Brand/GetForUi?skip=0&take=0");
            if (BrandResponse.IsSuccessStatusCode)
            {
                string contentStream = await BrandResponse.Content.ReadAsStringAsync();
                var Brands = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Brands = new SelectList(Brands, "Id", "Name");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubCategoryCreateVM subCategoryCreateVM)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            if (!ModelState.IsValid)
            {
                // Load necessary data again for the view
                await LoadViewBagData();
                return View(subCategoryCreateVM);
            }
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(subCategoryCreateVM.Name), "Name");
            content.Add(new StringContent(subCategoryCreateVM.Description), "Description");
            content.Add(new StringContent(subCategoryCreateVM.CategoryId.ToString()), "CategoryId");
            if(subCategoryCreateVM.BrandIds != null)
                {
                foreach (var colorId in subCategoryCreateVM.BrandIds)
                {
                    content.Add(new StringContent(colorId.ToString()), "BrandIds");
                }
            }
            if (subCategoryCreateVM.formFile != null)
            {
                var fileContent = new StreamContent(subCategoryCreateVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(subCategoryCreateVM.formFile.ContentType);
                content.Add(fileContent, nameof(subCategoryCreateVM.formFile), subCategoryCreateVM.formFile.FileName);
            }
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/SubCategory", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "SubCategory");
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
                await LoadViewBagData();
                return View(subCategoryCreateVM);
            }

        }
        private async Task LoadViewBagData()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);


            using HttpResponseMessage categoryResponse = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface?skip=0&take=35");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string contentStream = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            using HttpResponseMessage BrandResponse = await client.GetAsync("http://localhost:5246/api/Brand/GetForUi?skip=0&take=0");
            if (BrandResponse.IsSuccessStatusCode)
            {
                string contentStream = await BrandResponse.Content.ReadAsStringAsync();
                var Brands = JsonConvert.DeserializeObject<List<CategoryAdminVM>>(contentStream);
                ViewBag.Brands = new SelectList(Brands, "Id", "Name");
            }
        }
        public async Task<IActionResult> Delete(string? id)
        {
            using var client = new HttpClient();
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
            var currentUserId = JwtHelper.GetUserIdFromJwt(jwtToken);
            if (currentUserId == id)
            {
                return Json(new { success = false, message = "You cannot delete your own account." });
            }

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var response = await client.DeleteAsync($"http://localhost:5246/api/SubCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "SubCategory successfully deleted." });
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
            await LoadViewBagData();
            if (id == null)
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/SubCategory/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<SubCategoryUpdateVM>(ContentStream);
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
        public  async Task<IActionResult> Update(int? id, SubCategoryUpdateVM subCategoryUpdateVM)
        {
            await LoadViewBagData();
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
   

            var content = new MultipartFormDataContent();


            content.Add(new StringContent(subCategoryUpdateVM.Name ?? string.Empty), "Name");

            // Add Description (even if null or empty)
            content.Add(new StringContent(subCategoryUpdateVM.Description ?? string.Empty), "Description");
            content.Add(new StringContent(subCategoryUpdateVM.CategoryId.ToString()??string.Empty), "CategoryId");
            if (subCategoryUpdateVM.BrandIds != null)
            {
                foreach (var colorId in subCategoryUpdateVM.BrandIds)
                {
                    content.Add(new StringContent(colorId.ToString()), "BrandIds");
                }
            }
            if (subCategoryUpdateVM.formFile != null)
            {
                var fileContent = new StreamContent(subCategoryUpdateVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(subCategoryUpdateVM.formFile.ContentType);
                content.Add(fileContent, nameof(subCategoryUpdateVM.formFile), subCategoryUpdateVM.formFile.FileName);
            }
            using HttpResponseMessage httpResponseMessage = await client.PutAsync($"http://localhost:5246/api/SubCategory/{id}", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
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
            }
            await LoadViewBagData();
            return View(subCategoryUpdateVM);
        }
    }
}
