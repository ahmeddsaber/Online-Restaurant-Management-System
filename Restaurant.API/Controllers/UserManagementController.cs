using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Admin User Management
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationDto pagination)
        {
            var result = await _userManagementService.GetAllUsersAsync(pagination);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var result = await _userManagementService.GetUserByIdAsync(userId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Create a new user (Manager/Staff)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userManagementService.CreateUserAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Assign roles to a user
        /// </summary>
        [HttpPut("assign-roles")]
        public async Task<IActionResult> AssignRoles([FromBody] AssignRolesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userManagementService.AssignRolesToUserAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Toggle user status (activate/deactivate)
        /// </summary>
        [HttpPut("{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var result = await _userManagementService.ToggleUserStatusAsync(userId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete (soft delete) a user
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userManagementService.DeleteUserAsync(userId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all available roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _userManagementService.GetAllRolesAsync();
            return Ok(result);
        }
    }
}

