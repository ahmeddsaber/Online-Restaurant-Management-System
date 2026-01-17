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
    public class OrderRepository :GenaricRepository<Order>, IOrderRepo
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllOrder()
        {
            return await Task.FromResult(_context.Orders.ToList());
        }


    }
}
