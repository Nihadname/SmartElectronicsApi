using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Net.Http.Headers;
using System.Net;
using SmartElectronicsApi.Mvc.ViewModels.Campaign;
using System;
using System.Text;
using SmartElectronicsApi.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.Branch;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CampaignController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Campaign/GetAllAdmin?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<CampaignListItemVM>>(ContentStream);
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
            await LoadViewBagData(client);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCampaignVM createCampaignVM)
        {
            using var client = new HttpClient();
            if (!ModelState.IsValid)
            {
                await LoadViewBagData(client);
                return View(createCampaignVM);
            }
            
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(createCampaignVM.Title), "Title");
            if (!string.IsNullOrWhiteSpace(createCampaignVM.Description))
            {
                content.Add(new StringContent(createCampaignVM.Description), "Description");
            }
            string startDateTimeString = createCampaignVM.StartDate.ToString("yyyy-MM-ddTHH:mm:ss");
            string endDateTimeString = createCampaignVM.EndDate.ToString("yyyy-MM-ddTHH:mm:ss");
            content.Add(new StringContent(startDateTimeString), "StartDate");
            content.Add(new StringContent(endDateTimeString), "EndDate");
            if (createCampaignVM.DiscountPercentage != null)
            {
                string? decimalString = createCampaignVM.DiscountPercentage?.ToString("F2");
                content.Add(new StringContent(endDateTimeString), "DiscountPercentage");
            }
            if (createCampaignVM.formFile != null)
            {
                var fileContent = new StreamContent(createCampaignVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(createCampaignVM.formFile.ContentType);
                content.Add(fileContent, nameof(BrandCreateVM.formFile), createCampaignVM.formFile.FileName);
            }
            if (createCampaignVM.ProductIds is not null)
            {
                foreach (int number in createCampaignVM.ProductIds)
                {
                    content.Add(new StringContent(number.ToString(), Encoding.UTF8, "text/plain"), "ProductIds");
                }
            }
            if (createCampaignVM.BranchIds is not null)
            {
                foreach (int number in createCampaignVM.BranchIds)
                {
                    content.Add(new StringContent(number.ToString(), Encoding.UTF8, "text/plain"), "BranchIds");
                }
            }
            var response = await client.PostAsync("Campaign", content);
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
                await LoadViewBagData(client);
                return View(createCampaignVM);
            }

        }
        private async Task LoadViewBagData(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage productResponse = await client.GetAsync("http://localhost:5246/api/Product/GetAllForSelect");
            if (productResponse.IsSuccessStatusCode)
            {
                string contentStream = await productResponse.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductSelectVM>>(contentStream);
                ViewBag.Products = new SelectList(products, "Id", "Name");
            }
            using HttpResponseMessage branchResponse = await client.GetAsync("http://localhost:5246/api/Branch/GetForSelect");
            if (productResponse.IsSuccessStatusCode)
            {
                string contentStream = await branchResponse.Content.ReadAsStringAsync();
                var branches = JsonConvert.DeserializeObject<List<BranchSelectVM>>(contentStream);
                ViewBag.Branches = new SelectList(branches, "Id", "Name");
            }
        }
    }
}
