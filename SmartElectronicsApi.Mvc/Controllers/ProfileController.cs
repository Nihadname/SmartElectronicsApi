using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
       
    }
}
