using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
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
            var token = Request.Cookies["JwtToken"];
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync($"Slider/GetSliderForUi/{take}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {

                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<SliderListItemVm>>(ContentStream);
                HomeViewModel homeViewModel = new HomeViewModel();
                homeViewModel.Sliders = data;
                return View(homeViewModel);
            }
            else if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
              return  RedirectToAction("Login","Account");
            }
            return BadRequest();
        }

       
    }
}
