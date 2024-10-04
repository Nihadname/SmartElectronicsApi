using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.Helpers;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class RoleController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 2)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Role?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<RoleVM>>(ContentStream);
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
            var response = await client.DeleteAsync($"http://localhost:5246/api/Role/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "User successfully deleted." });
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
            return View();  
        }
        [HttpPost]
        public async Task<IActionResult> Create(RolePostVM rolePostVM)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var stringData = JsonConvert.SerializeObject(rolePostVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Role",content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {

                TempData["CreatingSucces"] = "role  yaradildi";
                return RedirectToAction("Index", "Role");
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
                return View(rolePostVM);
            }
        }

        public async Task<IActionResult> Update(string id)
        {
            if (string.IsNullOrWhiteSpace(id)||id==null)
            {
                return RedirectToAction("Error404", "Home", new { area = "" });

            }
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Role/{id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {

                var role = await httpResponseMessage.Content.ReadAsStringAsync();
                var FinalResult = JsonConvert.DeserializeObject<RolePostVM>(role);
                return View(FinalResult);

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
        public async Task<IActionResult> Update(string id, RolePostVM rolePostVM)
        {
           
                if (rolePostVM.Name == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid input.");
                    return View(rolePostVM);
                }

                var handler = new HttpClientHandler();
                using var client = new HttpClient(handler);
                var jwtToken = Request.Cookies["JwtToken"];
                if (string.IsNullOrEmpty(jwtToken))
                {
                    ModelState.AddModelError("", "User not authenticated.");
                    return View(rolePostVM);
                }

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var stringData = JsonConvert.SerializeObject(rolePostVM);
                var content = new StringContent(stringData, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:5246/api/Role/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["UpdateRole"] = "ProfileImage deyisdirildi";
                    return RedirectToAction("Index", "Role");
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
                            TempData["AddingError"] = (error.Key, error.Value);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                    }
                    return View(rolePostVM);
                }
            }
         
        }


    }
