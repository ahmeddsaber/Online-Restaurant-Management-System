using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
       IMenuItemService muneItemService;
        public MenuItemController(IMenuItemService muneItemService)
        {
            this.muneItemService = muneItemService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await muneItemService.GetAllMenuItemsAsync();
            return Ok(menuItems);
        }
    }
}
