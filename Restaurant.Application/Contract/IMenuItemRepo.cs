using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IMenuItemRepo:IGenaricRepository<MenuItem>
    {
        public Task<IEnumerable<MenuItem>> GetAllItems();
        public Task<IEnumerable<MenuItem>> GetItemsByPriceRange(decimal minPrice, decimal maxPrice);
        public Task<IEnumerable<MenuItem>> SearchItemsByName(string name);
        public Task<IEnumerable<MenuItem>> GetItemsWithPreparationTimeLessThan(int minutes);
        public Task<IEnumerable<MenuItem>> GetItemsWithCategory();
        public Task<IEnumerable<MenuItem>> GetItemsSortedByPrice(bool ascending);
        public Task<IEnumerable<MenuItem>> GetItemsSortedByPreparationTime(bool ascending);
        public Task<IEnumerable<MenuItem>> GetItemsAddedInLastNDays(int days);
       
        public Task<IEnumerable<MenuItem>> GetAvailableItems();
        public Task<IEnumerable<MenuItem>> GetNotAvailableItems();
        public Task<IEnumerable<MenuItem>> GetItemsforCustomer();

        public Task<MenuItem> TopSellingItemDto();


    }
}
