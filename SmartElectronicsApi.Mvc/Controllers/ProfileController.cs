using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Auth/Profile");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var User = await httpResponseMessage.Content.ReadAsStringAsync();
                var FinalResult = JsonConvert.DeserializeObject<UserGetVm>(User);
                ProfileViewModel profileViewModel = new ProfileViewModel();
                profileViewModel.UserGetVm = FinalResult;
                return View(profileViewModel);
            } else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "Account");
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            else
            {
                return RedirectToAction("Error404", "Home");
            }
        }
        
        public async Task<IActionResult> UpdateImage()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateImage(UserUpdateImageVM userUpdateImageVM)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            var jwtToken = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                ModelState.AddModelError("", "User not authenticated.");
                return View(userUpdateImageVM);
            }
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

            var content = new MultipartFormDataContent();

            if (userUpdateImageVM.formFile != null)
            {
                var fileContent = new StreamContent(userUpdateImageVM.formFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(userUpdateImageVM.formFile.ContentType);
                content.Add(fileContent, nameof(userUpdateImageVM.formFile), userUpdateImageVM.formFile.FileName);
            }


            var response = await client.PatchAsync("Auth/UpdateImage", content);

            if (response.IsSuccessStatusCode)
            {
                
                TempData["AddingSuccess"] = "ProfileImage deyisdirildi";
                return RedirectToAction("Index", "Profile");
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
                return View(userUpdateImageVM);
            }
        }

    }
}
