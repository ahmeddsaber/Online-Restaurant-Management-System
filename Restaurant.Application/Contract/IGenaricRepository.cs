using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Contract
{
    public interface IGenaricRepository<TEntity> where TEntity : BaseEntity
    {

        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> Create(TEntity entity);
        TEntity Update(TEntity entity);
        Task<TEntity> Delete(int id);




    }
}
