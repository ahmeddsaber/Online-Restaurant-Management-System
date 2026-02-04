using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Admin;
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
        [HttpGet("ForAdmin")]
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
          
            return Ok(menuItems);
        }
        [HttpGet("GetAllMenuItemForCustomer")]
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
           
            return Ok(topItem);
        }
        [Produces("application/json")]
        [HttpGet("available")]
        [ActionName("GetAvailableMenuItemsForAdmin")]
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
           
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("not-available")]
        [ActionName("GetNotAvailableMenuItemsForAdmin")]
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
          
            return Ok(menuItems);
        }
        [Produces("Application/json")]
        [HttpGet("Available For Customer")]
        [ActionName("GetAvailableMenuItemsForCustomer")]
        public async Task<IActionResult> GetAvailableMenuItemsForCustomer()
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

            return Ok(menuItems);
        }

        [Produces("application/json")]
        [HttpGet("added-in-last-{days}-days")]
        [ActionName("GetMenuItemsAddedInLastNDaysForCustomer")]
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
         
            return Ok(menuItems);
        }
        [Produces("Application/json")]
        [HttpGet("added-in-last-{days}-days For Admin")]
        [ActionName("GetMenuItemsAddedInLastNDaysForAdmin")]
        public async Task<IActionResult> GetMenuItemsAddedInLastNDaysForAdmin(int days)
        {
            var menuItems = await muneItemService.GetItemsAddedInLastNDays(days);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found added in last " + days + " days.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            return Ok(menuItems);
        }


        [Produces("application/json")]

        [HttpGet("GetitemByPriceRange")]
        [ActionName("GetItemsByPriceRangeForCustomer")]
        public async Task<IActionResult> GetItemsByPriceRange([FromQuery] decimal minPrice = 50, [FromQuery] decimal maxPrice = 100)
        {
            var menuItems = await muneItemService.GetItemsByPriceRangeForCustomer(minPrice, maxPrice);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found in the specified price range.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         
            return Ok(menuItems);
        }

        [Produces("application/json")]
        [HttpGet("search/{name}")]
        [ActionName("SearchItemsByNameforCustomer")]
        public async Task<IActionResult> SearchItemsByNameforCustomer(string name)
        {
            var menuItems = await muneItemService.SearchItemsByNameforCustomer(name);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found matching the name: " + name);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            return Ok(menuItems);
        }
        [Produces("Application/json")]
        [HttpGet("Search{Name}")]

        [ActionName("SearchItemsByDescriptionForAdmin")]
        public async Task<IActionResult> SearchItemsByDescriptionForAdmin(string name)
        {
            var menuItems = await muneItemService.SearchItemsByNameforAdmin(name);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found matching the description: " + name);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return Ok(menuItems);
        }


        [Produces("application/json")]
        [HttpGet("preparation-time/{minutes}")]
        [ActionName("GetItemsWithPreparationTimeLessThanForCustomer")]
        public async Task<IActionResult> GetItemsWithPreparationTimeLessThan(int minutes)
        {
            var menuItems = await muneItemService.GetItemsWithPreparationTimeLessThan(minutes);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found with preparation time less than " + minutes + " minutes.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("sort-by-price/{ascending}")]
        [ActionName("GetItemsSortedByPriceForCustomer")]
        public async Task<IActionResult> GetItemsSortedByPriceForCustomer(bool ascending)
        {
            var menuItems = await muneItemService.GetItemsSortedByPrice(ascending);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            return Ok(menuItems);
        }
        [Produces("application/json")]
        [HttpGet("sort-by-preparation-time/{ascending}")]
        [ActionName("GetItemsSortedByPreparationTimeForCustomer")]
        public async Task<IActionResult> GetItemsSortedByPreparationTimeForCustomer(bool ascending)
        {
            var menuItems = await muneItemService.GetItemsSortedByPreparationTime(ascending);
            if (menuItems == null || !menuItems.Any())
            {
                return NotFound("No menu items found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            return Ok(menuItems);
        }
        [HttpPost("CreateMenuItem")]
        [ActionName("CreateMenuItem")]
        public async Task<IActionResult> CreateMenuItem([FromBody] AdminCreateMenuItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Price < 0)
                return BadRequest("Price cannot be negative.");

            if (dto.PreparationTime < 0)
                return BadRequest("Preparation time cannot be negative.");

            // Check for duplicate name
            var existingItem = await muneItemService.SearchItemsByNameforAdmin(dto.NameEn);
            if (existingItem != null && existingItem.Any())
                return Conflict("A menu item with the same name already exists.");

            // Create item
            var createdItem = await muneItemService.CreateItemMenu(dto);

            // MenuItemService حاليا بيرجع DTO بدون Id، فلازم نرجع entity أو نعدل DTO
            return CreatedAtAction(nameof(GetMenuItemById), createdItem);
        }

        [Produces("application/json")]
        [HttpPut("UpdateMenuItem")]
        [ActionName("UpdateMenuItem")]
        public async Task<IActionResult> UpdateMenuItem([FromBody] AdminUpdateMenuItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (dto.Id <= 0)
            {
                return BadRequest("Invalid menu item Id.");
            }
            if (dto.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }
            if (dto.PreparationTime < 0)
            {
                return BadRequest("Preparation time cannot be negative.");
            }
            if (dto.Price > 0)
            {
                var existingItem = await muneItemService.GetMenuItemByIdAsync(dto.Id);
                if (existingItem == null)
                {
                    return NotFound("Menu item not found.");
                }
            }
            var updatedItem = await muneItemService.UpdateMenuItem(dto);
            return Ok(updatedItem);
        }
        [Produces("application/json")]

        [HttpDelete("DeleteMenuItem/{id}")]
        [ActionName("DeleteMenuItem")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid menu item Id.");
            }
            var existingItem = await muneItemService.GetMenuItemByIdAsync(id);
            if (existingItem == null)
            {
                return NotFound("Menu item not found.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            await muneItemService.DeleteMenuItem(id);
            return NoContent();
        }
    }
    }


