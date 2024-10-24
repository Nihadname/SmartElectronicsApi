using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectronicsApi.Application.Interfaces;

namespace SmartElectronicsApi.Api.Apps.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;

        public DashBoardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklySales()
        {
            var salesData = await dashboardService.GetWeeklySalesAsync();
            return Ok(salesData);
        }

        
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var salesData = await dashboardService.GetMonthlySalesAsync();
            return Ok(salesData);
        }

     
        [HttpGet("yearly")]
        public async Task<IActionResult> GetYearlySales()
        {
            var salesData = await dashboardService.GetYearlySalesAsync();
            return Ok(salesData);
        }
    }
}
