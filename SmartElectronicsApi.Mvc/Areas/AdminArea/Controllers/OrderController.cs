﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Order;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Net.Http.Headers;
using System.Net;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class OrderController : Controller
    {

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Order/GetAll?pageNumber={pageNumber}&pageSize=10");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<OrderAdminListItemVM>>(ContentStream);
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
        public async  Task<IActionResult> AcceptanceOfBeingShipped(int? id)
        {
            if(id is  null) return RedirectToAction("Index", "Order");

            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", jwtToken);
            using HttpResponseMessage httpResponseMessage = await client.PutAsync($"Order/AcceptanceOfBeingShipped/{id}",null);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Order");
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

            if (id is null) return RedirectToAction("Index", "Order");
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Json(new { success = false, message = "User not authenticated." });
            }
            client.DefaultRequestHeaders.Authorization =
         new AuthenticationHeaderValue("Bearer", jwtToken);
            using HttpResponseMessage httpResponseMessage = await client.DeleteAsync($"Order/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Order successfully deleted." });
            }
            else
            {
                var errorResponseString = await httpResponseMessage.Content.ReadAsStringAsync();
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
        public async Task<IActionResult> VerifyOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOrder(OrderVerfiyVM orderVerfiyVM)
        {
            if (!ModelState.IsValid) return View(orderVerfiyVM);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                   new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var stringData = JsonConvert.SerializeObject(orderVerfiyVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Order/VerifyOrder", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                    TempData["CreatingOrdercolor"] = "Order  yaradildi";
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
                return View(orderVerfiyVM);
            }

        }

        
        public IActionResult VerifyOrderWithQrCode()
        {
            return View();
        }
    }
}
