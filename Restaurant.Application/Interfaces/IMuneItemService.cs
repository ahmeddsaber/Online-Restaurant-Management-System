
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<AdminMenuItemDto>> GetAllMenuItemsAsync();
        public Task<IEnumerable<AdminMenuItemDto>> GetAllItems();
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsByPriceRange(decimal minPrice, decimal maxPrice);
        public Task<IEnumerable<AdminMenuItemDto>> SearchItemsByNameforAdmin(string name);
        public Task<IEnumerable<CustomerMenuItemDto>> SearchItemsByNameforCustomer(string name);
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsWithPreparationTimeLessThan(int minutes);
        public Task<IEnumerable<AdminMenuItemDto>> GetItemsWithCategory();
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsWithCategoryForCustomer();
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsSortedByPrice(bool ascending);
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsSortedByPreparationTime(bool ascending);
        public Task<IEnumerable<AdminMenuItemDto>> GetItemsAddedInLastNDays(int days);
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsAddedInLastNDaysForCustomer(int days);

        public Task<IEnumerable<AdminMenuItemDto>> GetAvailableItems();
        public Task<IEnumerable<CustomerMenuItemDto>> GetAvailableItemsForCustmer();
        public Task<IEnumerable<AdminMenuItemDto>> GetNotAvailableItems();
        public Task<IEnumerable<CustomerMenuItemDto>> GetItemsforCustomer();

        public Task<TopItemDto> TopSellingItemDto();
        public Task<AdminMenuItemDto> GetMenuItemByIdAsync(int id);
        public Task<CustomerMenuItemDto> GetMenuItemByIdAsyncForCustomer(int id);
        public Task<AdminCreateMenuItemDto> CreateItemMenu(AdminCreateMenuItemDto menuItem);
        public Task<AdminUpdateMenuItemDto> UpdateMenuItem(AdminUpdateMenuItemDto menuItem);
        public Task DeleteMenuItem(int id);

    }
}
