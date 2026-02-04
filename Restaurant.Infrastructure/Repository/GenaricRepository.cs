using Microsoft.EntityFrameworkCore;
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
   public class GenaricRepository<Entity>:IGenaricRepository<Entity> where Entity: BaseEntity
    {
        
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Entity> _dbSet;
        public GenaricRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Entity>();
        }
        public IQueryable<Entity> GetAll()
        {
            return _dbSet;
        }
        public async Task<Entity?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<Entity> Create(Entity entity)
        {
            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }   
        public Entity Update(Entity entity)
        {
            var result = _dbSet.Update(entity);
            return result.Entity;
        }
        public async Task<Entity> Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
               throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
            entity.IsDeleted = true;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

    }
}
