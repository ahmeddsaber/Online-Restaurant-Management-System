using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;

namespace Restaurant.Infrastructure.Repository
{
    public class MenuItemRepository
        : GenaricRepository<MenuItem>, IMenuItemRepo
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllItems()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.Price >= minPrice &&
                    m.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> SearchItemsByName(string name)
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    (m.NameEn.Contains(name) || m.NameAr.Contains(name)))
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsWithPreparationTimeLessThan(int minutes)
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.PreparationTime <= minutes)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsWithCategory()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsSortedByPrice(bool ascending)
        {
            var query = _context.MenuItems
                .AsNoTracking()
                .Where(m => !m.IsDeleted);

            return ascending
                ? await query.OrderBy(m => m.Price).ToListAsync()
                : await query.OrderByDescending(m => m.Price).ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsSortedByPreparationTime(bool ascending)
        {
            var query = _context.MenuItems
                .AsNoTracking()
                .Where(m => !m.IsDeleted);

            return ascending
                ? await query.OrderBy(m => m.PreparationTime).ToListAsync()
                : await query.OrderByDescending(m => m.PreparationTime).ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsAddedInLastNDays(int days)
        {
            var date = DateTime.UtcNow.AddDays(-days);

            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.CreatedAt >= date)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItems()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetNotAvailableItems()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    !m.IsAvailable)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetItemsforCustomer()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.IsAvailable)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<MenuItem?> TopSellingItemDto()
        {
            return await _context.MenuItems
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.DailyOrderCount)
                .FirstOrDefaultAsync();
        }
    }
}
