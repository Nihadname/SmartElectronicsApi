using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Brand;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index(int take = 7)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Slider/GetSliderForUi/{take}");
            using HttpResponseMessage httpResponseMessage1 = await client.GetAsync("Category/GetAllForUserInterface");
            using HttpResponseMessage httpResponseMessage2 = await client.GetAsync("Brand/GetForUi");

            if (httpResponseMessage.IsSuccessStatusCode)
            {

                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SliderListItemVm>>(ContentStream);
                HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.Sliders = data;
                if (httpResponseMessage1.IsSuccessStatusCode)
                {
                    var Categories = await httpResponseMessage1.Content.ReadAsStringAsync();
                    var FinalResult = JsonConvert.DeserializeObject<List<CategoryListItemVM>>(Categories);
                    homeViewModel.Categories = FinalResult;
                    if (httpResponseMessage2.IsSuccessStatusCode)
                    {
                        var brands = await httpResponseMessage2.Content.ReadAsStringAsync();
                        var FinalResultBrand = JsonConvert.DeserializeObject<List<BrandListItemVM>>(brands);
                        homeViewModel.Brands = FinalResultBrand;
                    }
                }
                return View(homeViewModel);
            }

            return BadRequest();
        }
        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubscriberCreate([FromBody] SubscriberVM subscriberVM)
        {
            using var client = new HttpClient();
            var stringData = JsonConvert.SerializeObject(subscriberVM);
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:5246/api/Subscriber", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "You have subscribed successfully." });
            }

            var errorResponseString = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponseString);

            if (errorResponse?.Errors != null && errorResponse.Errors.Any())
            {
                return Json(new { success = false, message = errorResponse.Errors.FirstOrDefault().Value });
            }

            return Json(new { success = false, message = errorResponse?.Message ?? "An unknown error occurred." });
        }

    }
}
