using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace SmartElectronicsApi.Mvc.ViewCompenents
{
    public class SettingHeaderViewComponent : ViewComponent
    {

        public SettingHeaderViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = Request.Cookies["jwtToken"];
           ViewBag.ExistenceOfIcon=false;

            if (!string.IsNullOrEmpty(token))
            {
                ViewBag.ExistenceOfIcon = true;
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(token);

                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value
                               ?? jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

                if (!string.IsNullOrEmpty(fullName))
                {
                    ViewBag.FullName = fullName;
                }
              
            }
            return View();
        }
    }
}
