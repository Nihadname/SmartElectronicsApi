using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Dtos.Branch;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> Create(BranchCreateDto branchCreateDto)
        {
            return Ok(await _branchService.CreateBranch(branchCreateDto));
        }
        [HttpGet("GetForSelect")]
        public async Task<IActionResult> GetForSelect()
        {
            return Ok(await _branchService.GetAllSelect());
        }
    }
}
