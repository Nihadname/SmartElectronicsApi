using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Auth;
using System.Net.Http.Headers;

namespace SmartElectronicsApi.Mvc.Areas.AdminArea.ViewCompenents
{
    public class SettingAdminNavBarViewComponent:ViewComponent
    {
        public SettingAdminNavBarViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5246/api/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["JwtToken"]);
            using HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost:5246/api/Auth/GetAdmin");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string ContentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserGetVm>(ContentStream);
                ViewBag.UserImage = data?.Image;
                ViewBag.Name = data?.FullName;
                Console.WriteLine(ViewBag.UserImage);

                return View();

            }
            return View();
           
        }
    }
}
