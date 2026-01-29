using Restaurant.Application.DTOS.Manager;
using Restaurant.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IMenuCategoryRepo : IGenaricRepository<MenuCategory>
    {
        Task< IEnumerable <MenuCategory?>> SearchAsync(string? name);
        Task<MenuCategory?> GetCategoryByIdWithItemsAsync(int id);
        Task<IEnumerable<MenuCategory>> GetActiveCategoriesAsync();
        Task<IEnumerable<MenuCategory>> GetActiveCategoriesforCustomerAsync();
        Task<IEnumerable<MenuCategory>> GetDeletedCategoriesAsync();
        Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync();

        Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync();
    }
}
