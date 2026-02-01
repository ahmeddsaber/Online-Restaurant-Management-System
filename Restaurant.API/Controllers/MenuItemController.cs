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
        [Produces("application/json")]
        [HttpGet]
        [ActionName("GetMenuItems")]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems = await muneItemService.GetAllMenuItemsAsyncForAdmin();
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(menuItems);
        }
        [HttpGet]
        [ActionName("GetMenuItemsForUser")]
        public async Task<IActionResult> GetMenuItemsForUser()
        {
            var menuItems = await muneItemService.GetItemsforCustomer();
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("{id}")]

        [ActionName("GetMenuItemById")]
        public async Task<IActionResult> GetMenuItemById(int id)


        {
            var menuItem = await muneItemService.GetMenuItemByIdAsync(id);
            return Ok(menuItem);
        }
        [Produces("application/json")]
        [HttpGet("customer/{id}")]
        [ActionName("GetMenuItemByIdForCustomer")]
        public async Task<IActionResult> GetMenuItemByIdForCustomer(int id)
        {
            var menuItem = await muneItemService.GetMenuItemByIdAsyncForCustomer(id);
            if (menuItem == null)
            {
                return NotFound("Menu item not found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            return Ok(menuItem);
        }
        [Produces("application/json")]
        [HttpGet("top-selling")]
        [ActionName("GetTopSellingItem")]
        public async Task<IActionResult> GetTopSellingItem()
        {
            var topItem = await muneItemService.TopSellingItemDto();
            if (topItem == null)
            {
                return NotFound("No top selling item found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(topItem);
        }
        [Produces("application/json")]
        [HttpGet("available")]
        [ActionName("GetAvailableMenuItems")]
        public async Task<IActionResult> GetAvailableMenuItems()
        {
            var menuItems = await muneItemService.GetAvailableItemsForCustmer();
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No available menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("not-available")]
        [ActionName("GetNotAvailableMenuItems")]
       public async Task<IActionResult> GetNotAvailableMenuItems()
        {
            var menuItems = await muneItemService.GetNotAvailableItems();
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No not available menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("added-in-last-{days}-days")]  
        [ActionName("GetMenuItemsAddedInLastNDays")]
        public async Task<IActionResult> GetMenuItemsAddedInLastNDays(int days)
        {
            var menuItems = await muneItemService.GetItemsAddedInLastNDaysForCustomer(days);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found added in last " + days + " days.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(menuItems);
        }
    }

}
