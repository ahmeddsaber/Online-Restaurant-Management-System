using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        /// <summary>
        /// Get all tables (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTables([FromQuery] PaginationDto pagination)
        {
            var result = await _tableService.GetAllTablesAsync(pagination);
            return Ok(result);
        }

        /// <summary>
        /// Create a new table (Admin only)
        /// </summary>
        [HttpPost]
            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTable([FromBody] AdminCreateTableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tableService.CreateTableAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Update a table (Admin only)
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTable([FromBody] AdminUpdateTableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tableService.UpdateTableAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete a table (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var result = await _tableService.DeleteTableAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get available tables for customers
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTables()
        {
            var result = await _tableService.GetAvailableTablesForCustomerAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get tables for staff
        /// </summary>
        [HttpGet("staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetTablesForStaff()
        {
            var result = await _tableService.GetTablesForStaffAsync();
            return Ok(result);
        }

        /// <summary>
        /// Update table availability (Staff only)
        /// </summary>
        [HttpPut("availability")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateTableAvailability([FromBody] UpdateTableAvailabilityDto dto)
        {
            var result = await _tableService.UpdateTableAvailabilityAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
