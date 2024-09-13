using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SmartElectronicsApi.Api.Apps.UserInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("SignInWithGoogle")]
        public IActionResult SignInWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleResponse)),
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result =await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) return BadRequest(); // Handle failed login

            // Extract user information from Google response
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var userName=claims?.FirstOrDefault(s=>s.Type==ClaimTypes.Name)?.Value;
            var Id = claims?.FirstOrDefault(s=>s.Type == ClaimTypes.NameIdentifier)?.Value;
            var GivenName = claims?.FirstOrDefault(s => s.Type == ClaimTypes.GivenName)?.Value;

            // Handle your business logic with the email or other user info

            return Ok(new { Email = email,UserName=userName,id=Id,fullName=GivenName });
        }
    }
}
