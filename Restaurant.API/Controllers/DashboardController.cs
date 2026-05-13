using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Dashboard statistics for all roles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get admin dashboard with full statistics
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _dashboardService.GetAdminDashboardAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get manager dashboard with operational stats
        /// </summary>
        [HttpGet("manager")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetManagerDashboard()
        {
            var result = await _dashboardService.GetManagerDashboardAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get staff dashboard with kitchen and table stats
        /// </summary>
        [HttpGet("staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetStaffDashboard()
        {
            var result = await _dashboardService.GetStaffDashboardAsync();
            return Ok(result);
        }
    }
}

