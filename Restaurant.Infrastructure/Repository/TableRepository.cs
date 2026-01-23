using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Repository
{
    public class TableRepository : GenaricRepository<Table>, ITableRepo
    {
        private readonly ApplicationDbContext _context;

        public TableRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Table>> GetAvailableTablesAsync()
        {
            return await _context.Tables
                .Where(t => t.IsAvailable && !t.IsDeleted)
                .OrderBy(t => t.TableNumber)
                .ToListAsync();
        }

        public async Task<Table?> GetTableByNumberAsync(string tableNumber)
        {
            return await _context.Tables
                .FirstOrDefaultAsync(t => t.TableNumber == tableNumber && !t.IsDeleted);
        }

        public async Task<Table?> GetTableWithCurrentOrderAsync(int tableId)
        {
            return await _context.Tables
                .Include(t => t.Orders.Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled))
                .FirstOrDefaultAsync(t => t.Id == tableId && !t.IsDeleted);
        }
    }
}
