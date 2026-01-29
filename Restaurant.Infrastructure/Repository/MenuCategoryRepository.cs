using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class MenuCategoryRepository
        : GenaricRepository<MenuCategory>, IMenuCategoryRepo
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuCategory>> SearchAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<MenuCategory>();

            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c => !c.IsDeleted &&
                            ((c.NameEn != null && c.NameEn.Contains(name)) ||
                             (c.NameAr != null && c.NameAr.Contains(name))))
                .ToListAsync();
        }


        public async Task<MenuCategory?> GetCategoryByIdWithItemsAsync(int id)
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Include(c => c.MenuItems
                    .Where(i => i.IsAvailable && !i.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<MenuCategory>> GetActiveCategoriesAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c =>
                    !c.IsDeleted &&
                    c.MenuItems.Any(i => i.IsAvailable && !i.IsDeleted))
                .Include(c => c.MenuItems.Where(i => i.IsAvailable && !i.IsDeleted))
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuCategory>> GetDeletedCategoriesAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c => c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync()
        {
            var categorySales = await _context.MenuCategories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategorySalesDto
                {
                    CategoryNameEn = c.NameEn,
                    CategoryNameAr = c.NameAr,

                    ItemsSold = c.MenuItems
                        .Where(mi => !mi.IsDeleted)
                        .SelectMany(mi => mi.OrderItems)
                        .Where(oi => !oi.IsDeleted)
                        .Sum(oi => oi.Quantity),

                    Revenue = c.MenuItems
                        .Where(mi => !mi.IsDeleted)
                        .SelectMany(mi => mi.OrderItems)
                        .Where(oi => !oi.IsDeleted)
                        .Sum(oi => oi.UnitPrice * oi.Quantity)
                })
                .ToListAsync();

            var totalRevenue = categorySales.Sum(c => c.Revenue);

            foreach (var item in categorySales)
            {
                item.Percentage = totalRevenue == 0
                    ? 0
                    : Math.Round((item.Revenue / totalRevenue) * 100, 2);
            }

            return categorySales;
        }

        public  async Task<IEnumerable<MenuCategory>> GetActiveCategoriesforCustomerAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c =>
                    !c.IsDeleted &&
                    c.MenuItems.Any(i => i.IsAvailable && !i.IsDeleted))
                .Include(c => c.MenuItems.Where(i => i.IsAvailable && !i.IsDeleted))
                .ToListAsync();
        }
    }
}
