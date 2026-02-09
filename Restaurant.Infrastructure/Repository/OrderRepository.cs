using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;
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

        public class OrderRepository : GenaricRepository<Order>, IOrderRepo
        {
            private readonly ApplicationDbContext _context;

            public OrderRepository(ApplicationDbContext context) : base(context)
            {
                _context = context;
            }

            public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<Order?> GetOrderByIdAsync(int orderId)
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                            .ThenInclude(mi => mi.Category)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }

            public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
            }

            public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
            {
                return await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.Table)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Where(o => o.Status == status)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Where(o => o.Status == OrderStatus.Pending)
                    .OrderBy(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Where(o => o.Status == OrderStatus.Pending ||
                               o.Status == OrderStatus.Preparing ||
                               o.Status == OrderStatus.Ready)
                    .OrderBy(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Order>> GetTodayOrdersAsync()
            {
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);

                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
            {
                var query = _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered);

                if (startDate.HasValue)
                    query = query.Where(o => o.OrderDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(o => o.OrderDate <= endDate.Value);

                return await query.SumAsync(o => o.Total);
            }

            public async Task<int> GetOrdersCountByStatusAsync(OrderStatus status)
            {
                return await _context.Orders
                    .CountAsync(o => o.Status == status);
            }
        }

        

    
}
