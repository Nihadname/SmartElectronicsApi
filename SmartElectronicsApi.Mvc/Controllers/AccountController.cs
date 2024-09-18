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
        public  IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            using var client = new HttpClient();
            var stringData = JsonConvert.SerializeObject(vm);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5246/api/Auth", content);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var FinalResult = JsonConvert.DeserializeObject<UserGetVm>(data);
                TempData["RegisterSuccess"] = true;

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
                        TempData["RegisterError"] = (error.Key, error.Value);

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return View(vm);

            }
        }
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
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordVm forgetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVm);
            }
            using var client = new HttpClient();
            var email = Uri.EscapeDataString(forgetPasswordVm.Email);
            var stringData = JsonConvert.SerializeObject(forgetPasswordVm);
            var url = $"http://localhost:5246/api/Auth/ResetPasswordSendEmail?email={email}";

            var response = await client.PostAsync(url, null); 
            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();

                TempData["EmailSendingSuccess"] = responseAsString;
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
                        TempData["ForgetPasswordError"] = (error.Key, error.Value);

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                return View(forgetPasswordVm);

            }
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Email or token is missing.");
                TempData["ResetPasswordExperingError"] = "Email or token is missing.";
                return View(); 
            }
            using var client = new HttpClient();
            var emailEncoded = Uri.EscapeDataString(email);
            var tokenEncoded = Uri.EscapeDataString(token);
            var url = $"http://localhost:5246/api/Auth/CheckExperySutiationOfToken?email={emailEncoded}&token={tokenEncoded}";

            var response = await client.PostAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                return View(); // Show reset password form here
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
                        TempData["ResetPasswordExperingError"] = (error.Key, error.Value);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }

                TempData["ResetPasswordExperingError"] = "The token is either invalid or has expired.";
                return View(); // Redirect to an error page or show a message
            }
        }
    }
}
