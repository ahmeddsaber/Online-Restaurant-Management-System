using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.Interfaces;
using Asp.Versioning;

namespace Restaurant.API.Controllers
{
    /// <summary>
    /// Menu category management
    /// </summary>
    [ApiVersion("1.0")]
    public class MenuCategoryController : BaseController
    {
        private readonly IMenuCategoryService _menuCategoryService;
     

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        
        }

        // =====================================
        // 🔐 ADMIN ENDPOINTS
        // =====================================

        /// <summary>
        /// Get all menu categories (Admin)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMenuCategories()
        {
            var categories = await _menuCategoryService.GetAllCategories();
            if (!categories.Any())
                return Error("No categories found", statusCode: 404);

            return Success(categories);
        }

        /// <summary>
        /// Get active menu categories (Admin)
        /// </summary>
        [HttpGet("GetActiveMenuCategories")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveMenuCategories()
        {
            var categories = await _menuCategoryService.GetActiveCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No active categories found"));

            return Ok(ApiResponseDto<IEnumerable<AdminCategoryDto>>.SuccessResponse(categories));
        }

        /// <summary>
        /// Get active menu categories for customer view (Admin)
        /// </summary>
        [HttpGet("GetActiveMenuCategoriesForCustomer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveMenuCategoriesForCustomer()
        {
            var categories = await _menuCategoryService.GetActiveCategoriesforCustomerAsync();
            if (categories == null || !categories.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No categories found"));

            return Ok(ApiResponseDto<object>.SuccessResponse(categories));
        }

        /// <summary>
        /// Get deleted menu categories (Admin)
        /// </summary>
        [HttpGet("GetDeletedMenuCategories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedMenuCategories()
        {
            var categories = await _menuCategoryService.GetDeletedCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No deleted categories found"));

            return Ok(ApiResponseDto<object>.SuccessResponse(categories));
        }

        /// <summary>
        /// Get menu category by ID (Admin)
        /// </summary>
        [HttpGet("GetMenuCategoryById/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuCategoryById(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid category Id"));

            var category = await _menuCategoryService.GetCategoryById(id);
            if (category == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Category not found"));

            return Ok(ApiResponseDto<AdminCategoryDto>.SuccessResponse(category));
        }

        /// <summary>
        /// Get menu category with its items (Admin)
        /// </summary>
        [HttpGet("GetMenuCategoryWithItems/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuCategoryWithItems(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid category Id"));

            var category = await _menuCategoryService.GetCategoryByIdWithItemsAsync(id);
            if (category == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Category not found"));

            return Ok(ApiResponseDto<object>.SuccessResponse(category));
        }

        /// <summary>
        /// Search menu categories by name (Admin)
        /// </summary>
        [HttpGet("SearchMenuCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchMenuCategory([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Search text is required"));

            var categories = await _menuCategoryService.SearchAdminCategory(name);
            if (categories == null)
                return NotFound(ApiResponseDto<object>.ErrorResponse("Category not found"));

            return Ok(ApiResponseDto<object>.SuccessResponse(categories));
        }

        /// <summary>
        /// Create a new menu category (Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuCategory([FromForm] AdminCreateCategoryDto dto)
        {
            var category = await _menuCategoryService.CreateCategory(dto);

            if (category == null)
                return Error("Category name cannot be empty");

            return Created(category);
        }

        /// <summary>
        /// Update an existing menu category (Admin)
        /// </summary>
        [HttpPut("UpdateMenuCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuCategory([FromForm] AdminUpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid data"));

            var updatedCategory = await _menuCategoryService.UpdateCategory(dto);
            if (updatedCategory == null)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Category update failed"));

            return Ok(ApiResponseDto<AdminCategoryDto>.SuccessResponse(updatedCategory, "Category updated successfully"));
        }

        /// <summary>
        /// Delete a menu category (Admin)
        /// </summary>
        [HttpDelete("DeleteMenuCategory/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuCategory(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.ErrorResponse("Invalid category Id"));

            await _menuCategoryService.DeleteCategory(id);
            return Ok(ApiResponseDto<bool>.SuccessResponse(true, "Category deleted successfully"));
        }

        // =====================================
        // 🔐 MANAGER ENDPOINT
        // =====================================

        /// <summary>
        /// Get category sales data (Manager)
        /// </summary>
        [HttpGet("GetCategorySales")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCategorySales()
        {
            var result = await _menuCategoryService.GetCategorySalesAsync();
            if (result == null || !result.Any())
                return NotFound(ApiResponseDto<object>.ErrorResponse("No sales data found"));

            return Ok(ApiResponseDto<IEnumerable<CategorySalesDto>>.SuccessResponse(result));
        }
    }
}

 