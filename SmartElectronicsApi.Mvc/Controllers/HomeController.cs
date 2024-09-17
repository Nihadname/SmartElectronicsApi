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
    
        public async Task<IActionResult> Index(int skip=0, int take = 0)
        {
            using var client=new HttpClient();
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"http://localhost:5246/api/Slider/GetSliderForUi/{skip}/{take}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string  ContentStream=await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SliderListItemVm>>(ContentStream);
               HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.Sliders = data;
               return View(homeViewModel);
            }
            return View();
        }

       
    }
}
