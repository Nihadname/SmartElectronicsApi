using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using SmartElectronicsApi.Mvc.Helpers;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.pagination;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class UserController : Controller
    {
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 4)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
         new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Auth/GetAll?pageNumber={pageNumber}&pageSize={pageSize}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PaginatedResponseVM<UserGetVm>>(ContentStream);
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
                return RedirectToAction("Error404", "Home", new { area = "" } );
            }
        }
        //   public async Task<IActionResult> 
        public async Task<IActionResult> ChangeStatus(string id)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var currentUserId = JwtHelper.GetUserIdFromJwt(Request.Cookies["JwtToken"]);
            if (currentUserId == id)
            {
                TempData["BanningError"] = "you cant controll   status of yourself";
                return RedirectToAction("Index"); 
            }
            using HttpResponseMessage httpResponseMessage = await client.PatchAsync($"http://localhost:5246/api/Auth/{id}", null);

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
                        TempData["BanningError"] = (error.Key, error.Value);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return RedirectToAction("Index");
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

            var response = await client.DeleteAsync($"http://localhost:5246/api/Auth/{id}");

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
        //private string GetUserIdFromJwt(string jwtToken)
        //{
        //    var handler = new JwtSecurityTokenHandler();

        //    var jwtToken11 = handler.ReadJwtToken(jwtToken);

        //    var userIdClaim = jwtToken11.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId)?.Value;
                        
        //    // Check if the claim exists
        //    if (userIdClaim == null)
        //    {
        //        throw new InvalidOperationException("User ID claim not found in the JWT token.");
        //    }

        //    return userIdClaim; // Return the user ID
        //}


    }
}
