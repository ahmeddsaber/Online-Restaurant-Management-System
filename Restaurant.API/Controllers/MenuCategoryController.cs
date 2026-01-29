using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize] // أي Endpoint محتاج Login
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        // =====================================
        // 🔐 ADMIN ENDPOINTS
        // =====================================

        [HttpGet("GetAllMenuCategories")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMenuCategories()
        {
            var categories = await _menuCategoryService.GetAllCategories();
            if (categories == null || !categories.Any())
                return NotFound(new { success = false, message = "No categories found" });

            return Ok(new { success = true, data = categories });
        }

        [HttpGet("GetActiveMenuCategories")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveMenuCategories()
        {
            var categories = await _menuCategoryService.GetActiveCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound(new { success = false, message = "No active categories found" });

            return Ok(new { success = true, data = categories });
        }
        [HttpGet("GetActiveMenuCategoriesForCustomer")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveMenuCategoriesForCustomer()
        {
            var categories = await _menuCategoryService.GetActiveCategoriesforCustomerAsync();
            if (categories == null || !categories.Any())
                return NotFound(new { success = false, message = "No  categories found" });

            return Ok(new { success = true, data = categories });
        }

        [HttpGet("GetDeletedMenuCategories")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeletedMenuCategories()
        {
            var categories = await _menuCategoryService.GetDeletedCategoriesAsync();
            if (categories == null || !categories.Any())
                return NotFound(new { success = false, message = "No deleted categories found" });

            return Ok(new { success = true, data = categories });
        }

        [HttpGet("GetMenuCategoryById/{id:int}")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuCategoryById(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid category Id" });

            var category = await _menuCategoryService.GetCategoryById(id);
            if (category == null)
                return NotFound(new { success = false, message = "Category not found" });

            return Ok(new { success = true, data = category });
        }

        [HttpGet("GetMenuCategoryWithItems/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMenuCategoryWithItems(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid category Id" });

            var category = await _menuCategoryService.GetCategoryByIdWithItemsAsync(id);
            if (category == null)
                return NotFound(new { success = false, message = "Category not found" });

            return Ok(new { success = true, data = category });
        }

        [HttpGet("SearchMenuCategory")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchMenuCategory([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { success = false, message = "Search text is required" });

            var categories = await _menuCategoryService.SearchAdminCategory(name);
            if (categories == null)
                return NotFound(new { success = false, message = "Category not found" });

            return Ok(new { success = true, data = categories });
        }

        [HttpPost("CreateMenuCategory")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuCategory([FromBody] AdminCreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

            var category = await _menuCategoryService.CreateCategory(dto);

            if (category == null)
                return BadRequest(new { success = false, message = "Category name cannot be empty" });

            // ارجع 201 + object الجديد
            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = "Category created successfully",
                data = category
            });
        }


        [HttpPut("UpdateMenuCategory")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuCategory([FromBody] AdminUpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

            var updatedCategory = await _menuCategoryService.UpdateCategory(dto);
            if (updatedCategory == null)
                return BadRequest(new { success = false, message = "Category update failed" });

            return Ok(new { success = true, message = "Category updated successfully", data = updatedCategory });
        }

        [HttpDelete("DeleteMenuCategory/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuCategory(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid category Id" });

            await _menuCategoryService.DeleteCategory(id);
            return Ok(new { success = true, message = "Category deleted successfully" });
        }

        // =====================================
        // 🔐 MANAGER ENDPOINT
        // =====================================

        [HttpGet("GetCategorySales")]
       // [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetCategorySales()
        {
            var result = await _menuCategoryService.GetCategorySalesAsync();
            if (result == null || !result.Any())
                return NotFound(new { success = false, message = "No sales data found" });

            return Ok(new { success = true, data = result });
        }
    }
}
