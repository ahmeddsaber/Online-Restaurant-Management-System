using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;


namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        [HttpGet]
        [Route("GetMenuCategories")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult GetMenuCategories()
        {
            var menuCategories = _menuCategoryService.GetAllCategories();
            return Ok(menuCategories);
        }
    }
}
