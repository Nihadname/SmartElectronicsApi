using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using SmartElectronicsApi.Mvc.ViewModels.Slider;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class HomeController : Controller
    {
    
        public async Task<IActionResult> Index()
        {
            using var client=new HttpClient();
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Slider/GetSliderForUi");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string  ContentStream=await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SliderListItemVm>>(ContentStream);
               HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.Sliders = data;
                View(homeViewModel);
            }
            return View();
        }

       
    }
}
