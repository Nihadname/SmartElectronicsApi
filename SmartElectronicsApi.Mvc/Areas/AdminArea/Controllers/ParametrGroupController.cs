﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.ParametrGroup;
using SmartElectronicsApi.Mvc.ViewModels.Product;
using SmartElectronicsApi.Mvc.ViewModels.ProductVariation;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ParametrGroupController : Controller
    {
        public async  Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/ParametrGroup?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<ParametrGroupListItemVM>>(ContentStream);
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

            var response = await client.DeleteAsync($"http://localhost:5246/api/ParametrGroup/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Category successfully deleted." });
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
        public async Task<IActionResult> Create(ParametrGroupCreateVM parametrGroupCreateVM)
        {
            using var client = new HttpClient();
            new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View(parametrGroupCreateVM);
            }
            var stringData = JsonConvert.SerializeObject(parametrGroupCreateVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/ParametrGroup", content);
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
                await LoadViewBagData();
                return View(parametrGroupCreateVM);
            }
        }
        private async Task LoadViewBagData()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
           
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
