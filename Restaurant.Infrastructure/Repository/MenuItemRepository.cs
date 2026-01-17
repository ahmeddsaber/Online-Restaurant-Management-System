using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class MenuItemRepository : GenaricRepository<MenuItem>, IMenuItemRepo

    {
        private readonly ApplicationDbContext _context;
        public MenuItemRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }


        // Get all menu items asynchronously
        public async Task<IEnumerable<MenuItem>> GetAllItems()
        {
            return await _context.MenuItems
                                 .Where(m => !m.IsDeleted)
                                 .Include(m => m.Category) // optional: include navigation property
                                 .ToListAsync();
        }
    
}
}
