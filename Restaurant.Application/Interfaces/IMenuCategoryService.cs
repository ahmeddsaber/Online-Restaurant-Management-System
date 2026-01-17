using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IMenuCategoryService
    {
        IEnumerable<MenuCategory> GetAllCategories();
        //MenuCategory GetCategoryById(int id);
        //void AddCategory(MenuCategory category);
        //void UpdateCategory(MenuCategory category);
        //void DeleteCategory(int id);
    }
}
