using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IUnitOfWork : IDisposable
    {
        IGenaricRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        IMenuCategoryRepo MenuCategoryRepo { get; }
        IMenuItemRepo MenuItem { get; }
        IOrderRepo Order { get; }   
        IOrderItemRepo OrderItem { get; }

        Task<int> SaveChangesAsync();
        
    }
}
