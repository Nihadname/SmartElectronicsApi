using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Branch;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class BranchController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchCreateVM branchCreateVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            var stringData = JsonConvert.SerializeObject(branchCreateVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Branch", content);
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
                return View(branchCreateVM);
            }

        }
    }
}
