using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface ITableRepo : IGenaricRepository<Table>
    {
        Task<IEnumerable<Table>> GetAvailableTablesAsync();
        Task<Table?> GetTableByNumberAsync(string tableNumber);
        Task<Table?> GetTableWithCurrentOrderAsync(int tableId);
    }
}
