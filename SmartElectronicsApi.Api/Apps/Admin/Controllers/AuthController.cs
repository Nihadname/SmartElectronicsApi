using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Auth;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(IAuthService authService, RoleManager<IdentityRole> roleManager)
        {
            _authService = authService;
            _roleManager = roleManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get(int pageNumber, int pageSize)
        {
            return Ok(await _authService.GetAll(pageNumber, pageSize));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            return Ok(await _authService.Delete(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpPatch("{id}")]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            return Ok(await _authService.ChangeStatus(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("GetAdmin")]
        public async Task<IActionResult> GetProfileAdmin()
        {
            return Ok(await _authService.Profile());
        }
       
    }
}
