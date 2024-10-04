using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Role;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1, int pageSize = 10)
        {
            return Ok(await _roleService.GetAll(pageNumber, pageSize));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string? id)
        {
            return Ok(await _roleService.GetByid(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            return Ok(await _roleService.Delete(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            return Ok(await _roleService.Create(roleDto));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id,RoleDto roleDto)
        {
            return Ok(await _roleService.Update(id, roleDto));
        }
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            return Ok(await _roleService.GetUserRoles(id));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

        [HttpPut("UpdateRoles")]
        public async Task<IActionResult> UpdateRoles(string id, List<string> NewRoles)
        {
            return Ok(await _roleService.UpdateRole(id, NewRoles));
        }
    }
}
