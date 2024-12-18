using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartElectronicsApi.Mvc.Interfaces;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailService emailService;

        public AccountController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
                if(vm.CountryCode is null)
                {
                    ModelState.AddModelError(string.Empty,"country code cant be null");

                }
                return View(vm);
            }
            if (!string.IsNullOrEmpty(vm.CountryCode) && !string.IsNullOrEmpty(vm.PhoneNumber))
            {
                vm.PhoneNumber = vm.CountryCode + vm.PhoneNumber;
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
                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
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
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

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
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roles = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

                TempData["LoginSuccess"] = true;

                // Check if the user is an admin
              
                Response.Cookies.Append("JwtToken",token);
                TempData["LoginSuccess"] = true;
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "AdminArea" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
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
        public IActionResult LogOut(bool tokenExpired = false)
        {
            Response.Cookies.Delete("JwtToken");
            if (tokenExpired)
            {
                TempData["TokenExpiredMessage"] = "Your session has expired. Please log in again.";
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVm forgetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordVm);
            }
            var resetPasswordEmailDto = new ForgetPasswordEmailVm
            {
                Email = forgetPasswordVm.Email

            };
            var jsonContent = JsonConvert.SerializeObject(resetPasswordEmailDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            using var client = new HttpClient();

            var apiUrl = "http://localhost:5246/api/Auth/ResetPasswordSendEmail"; 
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ResetPasswordEmailVm>(responseContent);
                var email = apiResponse.Message.Email;
                var token = apiResponse.Message.Token;
                //token = HttpUtility.UrlDecode(token);
                var resetLink = Url.Action(
                "ResetPassword", 
                "Account", 
                new { email = email, token=token },
                protocol: HttpContext.Request.Scheme);
                string body;
                using (StreamReader sr = new StreamReader("wwwroot/Template/ForgetPassword.html"))
                {
                    body = sr.ReadToEnd();
                }
                body = body.Replace("{{link}}", resetLink).Replace("{{UserName}}", email);
                emailService.SendEmail(
                from: "nihadmi@code.edu.az\r\n",
                to: email,
                subject: "Verify Email",
                body: body,
                smtpHost: "smtp.gmail.com",
                smtpPort: 587,
                enableSsl: true,
                smtpUser: "nihadmi@code.edu.az\r\n",
                    smtpPass: "wmgd lwju ehhs aoaq\r\n"
            );

                TempData["EmailSendingSuccess"] = "An email with instructions has been sent to the provided address.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                        TempData["ForgetPasswordError"] = $"{error.Key}: {error.Value}";
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
                TempData["ResetPasswordError"] = "Email or token is missing.";
                return RedirectToAction("Index", "Home");
            }
            using var client = new HttpClient();
            var apiUrl = $"http://localhost:5246/api/Auth/CheckExperySutiationOfToken?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                ViewData["email"] = email;
                ViewData["token"] = token;
                return View();
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                        TempData["ResetPasswordError"] = $"{error.Key}: {error.Value}";
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }

                TempData["ResetPasswordError"] = "The token is either invalid or has expired.";
                return RedirectToAction("Index", "Home");

            }

        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, ResetPasswordVm resetPasswordDto)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Email or token is missing.");
                ViewData["email"] = email; // Preserve email
                ViewData["token"] = token; // Preserve token
                return View(resetPasswordDto);
            }

            if (!ModelState.IsValid)
            {
                ViewData["email"] = email; // Preserve email
                ViewData["token"] = token;
                return View(resetPasswordDto);
            }
          

            using var client = new HttpClient();
            var stringData = JsonConvert.SerializeObject(resetPasswordDto);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"http://localhost:5246/api/Auth/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["resetPasswordSuccess"] = "succesfull Reseting Password";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
                {
                    foreach (var error in errorResponse.Errors)
                    {
                        ModelState.AddModelError(error.Key, error.Value);
                        TempData["ResetPasswordError"] = (error.Key, error.Value);

                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorResponse?.Message ?? "An unknown error occurred.");
                }
                ViewData["email"] = email; // Preserve email
                ViewData["token"] = token;
                return View(resetPasswordDto);
            }

        }
        public async Task<IActionResult> ChangePassword()
        {
            using var client = new HttpClient();
            var apiUrl = "http://localhost:5246/api/Auth/CheckingAuth";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "" });
            }
            else
            {
                return RedirectToAction("Error404", "Home", new { area = "" });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword( ChangePasswordVm changePasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVm);
            }           

            var stringData = JsonConvert.SerializeObject(changePasswordVm);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var apiUrl = "http://localhost:5246/api/Auth/ChangePassword";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            // Make an API request to change the password
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["ChangePasswordSuccess"] = "Password changed successfully.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Handle error response from the API
                var errorResponseString = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

                if (errorResponse?.Errors != null && errorResponse?.Errors.Count() != 0)
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

                return View(changePasswordVm);
            }
        }

            public IActionResult AccessDenied()
        {
            return View("AccessDenied"); 
        }
    }
}
