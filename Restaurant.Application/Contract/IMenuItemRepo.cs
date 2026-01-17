using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IMenuItemRepo:IGenaricRepository<MenuItem>
    {
        public Task<IEnumerable<MenuItem>> GetAllItems();
      
    }
}
