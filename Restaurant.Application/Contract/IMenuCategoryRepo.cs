using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IMenuCategoryRepo : IGenaricRepository<MenuCategory>
    {

        
    
        Task<MenuCategory> GetCategoryByIdWithItemsAsync(int id);
        Task <MenuCategory> Search(string? name);





    }
}
