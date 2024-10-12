using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Color;
using System.Text;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(ContactVM contactVM)
        {
            using var client = new HttpClient();
            var stringData = JsonConvert.SerializeObject(contactVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            using HttpResponseMessage httpResponseMessage = await client.PostAsync("http://localhost:5246/api/Contact", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {

                TempData["Send"] = "Sent successfully";
                return View();
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
                return View(contactVM);
            }
        }
    }
}
