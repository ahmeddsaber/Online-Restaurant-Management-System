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
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Entity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Entity>();
        }

        public IQueryable<Entity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        /// <summary>
        /// Gets entity by ID with tracking enabled (for updates)
        /// </summary>
        public async Task<Entity?> GetById(int id)
        {
            // ✅ Use FirstOrDefaultAsync with AsTracking for update scenarios
            return await _dbSet
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Gets entity by ID without tracking (read-only scenario)
        /// </summary>
        public async Task<Entity?> GetByIdNoTracking(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Entity> Create(Entity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public Entity Update(Entity entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        /// <summary>
        /// Soft-deletes the entity by setting IsDeleted = true.
        /// SaveChanges is NOT called here — the caller owns the transaction.
        /// </summary>
        public async Task<Entity> Delete(int id)
        {
            var entity = await GetById(id);
            if (entity == null)
                throw new KeyNotFoundException($"Entity with id {id} not found.");

            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return entity;
        }
    }
}

