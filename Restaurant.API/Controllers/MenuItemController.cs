using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.Interfaces;
using Asp.Versioning;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Menu item management for Admin and Customer
    /// </summary>
    [ApiVersion("1.0")]
    public class MenuItemController : BaseController
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // =====================================
        // 🔐 ADMIN ENDPOINTS
        // =====================================

        /// <summary>
        /// Get all menu items (Admin view)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await _menuItemService.GetAllMenuItemsAsyncForAdmin();
            if (!menuItems.Any())
                return Error("No menu items found", statusCode: 404);

            return Success(menuItems);
        }

        /// <summary>
        /// Get paginated menu items (Admin)
        /// </summary>
        [HttpGet("paginated")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPaginated([FromQuery] PaginationDto pagination, [FromQuery] string? search)
        {
            var result = await _menuItemService.GetPaginatedMenuItems(pagination, search);
            return Success(result);
        }

        /// <summary>
        /// Get available menu items (Admin)
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAvailableMenuItems()
        {
            // ✅ FIXED: Use admin projection (GetAvailableItems), not the customer one
            var menuItems = await _menuItemService.GetAvailableItems();
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No available menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get not available menu items (Admin)
        /// </summary>
        [HttpGet("not-available")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetNotAvailableMenuItems()
        {
            var menuItems = await _menuItemService.GetNotAvailableItems();
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No unavailable menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get top selling items (Admin)
        /// </summary>
        [HttpGet("top-selling")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetTopSellingItem()
        {
            var topItem = await _menuItemService.TopSellingItemDto();
            if (topItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("No top selling item found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(topItem));
        }

        /// <summary>
        /// Get menu items added in last N days (Admin)
        /// </summary>
        [HttpGet("added-in-last-days-admin/{days:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuItemsAddedInLastNDaysForAdmin(int days)
        {
            var menuItems = await _menuItemService.GetItemsAddedInLastNDays(days);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse($"No menu items found added in last {days} days."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Search menu items by name (Admin)
        /// </summary>
        [HttpGet("search-admin/{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchItemsByNameForAdmin(string name)
        {
            var menuItems = await _menuItemService.SearchItemsByNameforAdmin(name);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse($"No menu items found matching: {name}"));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Create a new menu item (Admin)
        /// </summary>
        [HttpPost("CreateMenuItem")]
        [Authorize(Roles = "Admin")]
        // ✅ FIXED: [FromForm] required so that IFormFile (ImageFile) can be bound from multipart/form-data
        public async Task<IActionResult> CreateMenuItem([FromForm] AdminCreateMenuItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid data"));

            if (dto.Price < 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Price cannot be negative."));

            if (dto.PreparationTime < 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Preparation time cannot be negative."));

            var existingItem = await _menuItemService.SearchItemsByNameforAdmin(dto.NameEn);
            if (existingItem != null && existingItem.Any())
                return Conflict(ApiResponseDto<object>.ErrorResponse("A menu item with the same name already exists."));

            var createdItem = await _menuItemService.CreateItemMenu(dto);

            return StatusCode(StatusCodes.Status201Created,
                ApiResponseDto<object>.SuccessResponse(createdItem, "Menu item created successfully"));
        }

        /// <summary>
        /// Update menu item (Admin)
        /// </summary>
        [HttpPut("UpdateMenuItem")]
        [Authorize(Roles = "Admin")]
        // ✅ FIXED: [FromForm] required so that IFormFile (ImageFile) can be bound from multipart/form-data
        public async Task<IActionResult> UpdateMenuItem([FromForm] AdminUpdateMenuItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid data"));

            if (dto.Id <= 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid menu item Id."));

            if (dto.Price < 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Price cannot be negative."));

            if (dto.PreparationTime < 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Preparation time cannot be negative."));

            var existingItem = await _menuItemService.GetMenuItemByIdAsync(dto.Id);
            if (existingItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Menu item not found."));

            var updatedItem = await _menuItemService.UpdateMenuItem(dto);
            return Ok(ApiResponseDto<object>.SuccessResponse(updatedItem, "Menu item updated successfully"));
        }

        /// <summary>
        /// Delete menu item (Admin)
        /// </summary>
        [HttpDelete("DeleteMenuItem/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid menu item Id."));

            var existingItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (existingItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Menu item not found."));

            await _menuItemService.DeleteMenuItem(id);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Menu item deleted successfully"));
        }

        // =====================================
        // 🟢 CUSTOMER ENDPOINTS
        // =====================================

        /// <summary>
        /// Get all menu items (Customer view)
        /// </summary>
        [HttpGet("GetAllMenuItemForCustomer")]
        public async Task<IActionResult> GetMenuItemsForUser()
        {
            var menuItems = await _menuItemService.GetItemsforCustomer();
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Menu item not found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItem));
        }

        /// <summary>
        /// Get menu item by ID (Customer view)
        /// </summary>
        [HttpGet("customer/{id:int}")]
        public async Task<IActionResult> GetMenuItemByIdForCustomer(int id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsyncForCustomer(id);
            if (menuItem == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Menu item not found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItem));
        }

        /// <summary>
        /// Get available menu items for customer
        /// </summary>
        [HttpGet("available-for-customer")]
        public async Task<IActionResult> GetAvailableMenuItemsForCustomer()
        {
            var menuItems = await _menuItemService.GetAvailableItemsForCustmer();
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No available menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get menu items added in last N days (Customer)
        /// </summary>
        [HttpGet("added-in-last-days/{days:int}")]
        public async Task<IActionResult> GetMenuItemsAddedInLastNDays(int days)
        {
            var menuItems = await _menuItemService.GetItemsAddedInLastNDaysForCustomer(days);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse($"No menu items found added in last {days} days."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get menu items by price range
        /// </summary>
        [HttpGet("GetitemByPriceRange")]
        public async Task<IActionResult> GetItemsByPriceRange([FromQuery] decimal minPrice = 50, [FromQuery] decimal maxPrice = 100)
        {
            var menuItems = await _menuItemService.GetItemsByPriceRangeForCustomer(minPrice, maxPrice);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No menu items found in the specified price range."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Search menu items by name (Customer)
        /// </summary>
        [HttpGet("search/{name}")]
        public async Task<IActionResult> SearchItemsByNameForCustomer(string name)
        {
            var menuItems = await _menuItemService.SearchItemsByNameforCustomer(name);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse($"No menu items found matching: {name}"));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get items with preparation time less than specified minutes
        /// </summary>
        [HttpGet("preparation-time/{minutes:int}")]
        public async Task<IActionResult> GetItemsWithPreparationTimeLessThan(int minutes)
        {
            var menuItems = await _menuItemService.GetItemsWithPreparationTimeLessThan(minutes);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse($"No menu items found with preparation time less than {minutes} minutes."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get items sorted by price
        /// </summary>
        [HttpGet("sort-by-price/{ascending:bool}")]
        public async Task<IActionResult> GetItemsSortedByPrice(bool ascending)
        {
            var menuItems = await _menuItemService.GetItemsSortedByPrice(ascending);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }

        /// <summary>
        /// Get items sorted by preparation time
        /// </summary>
        [HttpGet("sort-by-preparation-time/{ascending:bool}")]
        public async Task<IActionResult> GetItemsSortedByPreparationTime(bool ascending)
        {
            var menuItems = await _menuItemService.GetItemsSortedByPreparationTime(ascending);
            if (menuItems == null || !menuItems.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No menu items found."));

            return Ok(ApiResponseDto<object>.SuccessResponse(menuItems));
        }
    }
}

