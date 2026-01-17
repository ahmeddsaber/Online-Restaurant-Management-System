using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using System.Threading;

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

        // 🔍 Search by Arabic OR English name
        public async Task<MenuCategory?> SearchAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c =>
                    c.NameEn.Contains(name) ||
                    c.NameAr.Contains(name))
                .FirstOrDefaultAsync();
        }

        // 📦 Category with active menu items only
        public async Task<MenuCategory?> GetCategoryByIdWithItemsAsync(int id)
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Include(c => c.MenuItems
                    .Where(i => i.IsAvailable && !i.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        // 📋 Only categories that have available menu items
        public async Task<List<MenuCategory>> GetActiveCategoriesAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c =>
                    !c.IsDeleted &&
                    c.MenuItems.Any(i => i.IsAvailable && !i.IsDeleted))
                .Include(c => c.MenuItems.Where(i => i.IsAvailable))
                .ToListAsync();
        }

        public async Task<MenuCategory?> Search(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c => c.NameEn.Contains(name) || c.NameAr.Contains(name))
                .FirstOrDefaultAsync();
        }



        public async Task<IEnumerable<MenuCategory>> GetAllCategoriesAsync()
        {
            return await _context.MenuCategories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
      
    }
}
