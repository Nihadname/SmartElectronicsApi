using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels.Auth;

namespace SmartElectronicsApi.Mvc.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {
            using var client = new HttpClient();
            var stringData=JsonConvert.SerializeObject(loginVm);
           var content=new StringContent(stringData);
            var response=await client.PostAsync("",content);
            if (response.IsSuccessStatusCode)
            {
                var dataFromApi=await response.Content.ReadAsStringAsync();
                var tokenResponse=JsonConvert.DeserializeObject<TokenResponse>(dataFromApi);
                Request.Headers.Append("token", JsonConvert.SerializeObject(tokenResponse));


            }
        }
        
    }
}
