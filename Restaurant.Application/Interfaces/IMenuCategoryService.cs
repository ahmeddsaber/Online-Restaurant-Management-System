using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IMenuCategoryService
    {
        Task<IEnumerable<AdminCategoryDto>> GetAllCategories();
        Task<IEnumerable<AdminCategoryDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<AdminCategoryDto>> GetDeletedCategoriesAsync();

        Task<AdminCategoryDto?> GetCategoryById(int id);
        Task<AdminCategoryDto?> GetCategoryByIdWithItemsAsync(int id);
        Task <IEnumerable<AdminCategoryDto?>> SearchAdminCategory(string? name);
        Task<IEnumerable<CustomerMenuCategoryDto>> GetActiveCategoriesforCustomerAsync();

        Task<AdminCategoryDto?> CreateCategory(AdminCreateCategoryDto category);

        Task<AdminCategoryDto?> UpdateCategory(AdminUpdateCategoryDto category);
        Task DeleteCategory(int id);

        Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync();
    }
}
