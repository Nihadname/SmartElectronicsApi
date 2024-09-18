using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using System.Text;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVm);
            }
            using var client = new HttpClient();
            var stringData = JsonConvert.SerializeObject(loginVm);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5246/api/Auth/Login", content);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                Response.Cookies.Append("JwtToken",token);
                TempData["LoginSuccess"] = true;

                return RedirectToAction("Index", "Home");
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
                        TempData["LoginError"] = (error.Key, error.Value);

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return View(loginVm);
            }
        }
        [HttpPost]
        public IActionResult LoginWithGoogle()
        {
            return Redirect("http://localhost:5246/api/Auth/SignInWithGoogle");
        }

        [HttpGet]
        public IActionResult GoogleResponse(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                Response.Cookies.Append("JwtToken", token);

                TempData["LoginSuccess"] = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["LoginError"] = "Google authentication failed. No token received.";
                return RedirectToAction("Login");
            }
        }
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("JwtToken");

            return RedirectToAction("Index", "Home");
        }

    }
}
