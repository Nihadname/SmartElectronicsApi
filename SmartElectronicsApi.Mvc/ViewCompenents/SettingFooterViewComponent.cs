using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartElectronicsApi.Mvc.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SmartElectronicsApi.Mvc.ViewCompenents
{
    public class SettingFooterViewComponent:ViewComponent
    {
        public SettingFooterViewComponent()
        {
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
         
            return View();

        }

    }
}
