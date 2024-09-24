using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Category;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class HomeController : Controller
    {
    
        public async Task<IActionResult> Index(int take = 7)
        {
            using var client=new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Slider/GetSliderForUi/{take}");
            using HttpResponseMessage httpResponseMessage1 = await client.GetAsync("http://localhost:5246/api/Category/GetAllForUserInterface");
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

    }
}
