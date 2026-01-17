using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Interfaces
{
    public interface IOrderService
    {
        public Task<IEnumerable<Order>> GetAllOrders();
    }
}
