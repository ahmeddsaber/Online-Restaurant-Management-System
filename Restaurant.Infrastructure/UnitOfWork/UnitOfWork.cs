using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.Contract.payment;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.DbContext;
using Restaurant.Infrastructure.Repository;

namespace Restaurant.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;

        private IMenuCategoryRepo? _menuCategoryRepo;
        private IMenuItemRepo? _menuItemRepo;
        private IOrderRepo? _orderRepo;
        private IOrderItemRepo? _orderItemRepo;
        private ITableRepo? _tableRepo;
        private IPaymentRepo? _paymentRepo;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        
        // ============================
        // Specific Repositories
        // ============================
        public IMenuCategoryRepo MenuCategoryRepo
            => _menuCategoryRepo ??= new MenuCategoryRepository(_context);
     

        public IMenuItemRepo MenuItem => _menuItemRepo ??= new MenuItemRepository(_context);
        public IOrderRepo Order => _orderRepo ??= new OrderRepository(_context);
        public IOrderItemRepo OrderItem => _orderItemRepo ??= new OrderItemRepository(_context);
        public ITableRepo Table => _tableRepo ??= new TableRepository(_context);
        public IPaymentRepo Payment => _paymentRepo ??= new PaymentRepository(_context);

        // ============================
        // Generic Repository
        // ============================
        public IGenaricRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                var repo = new GenaricRepository<TEntity>(_context);
                _repositories[type] = repo;
            }

            return (IGenaricRepository<TEntity>)_repositories[type];
        }

        // ============================
        // Save
        // ============================
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                Console.WriteLine("💾 [UOW] Saving changes...");

                var result = await _context.SaveChangesAsync();

                Console.WriteLine($"✅ [UOW] {result} changes saved");

                return result;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"❌ DbUpdateException: {ex.Message}");
                Console.WriteLine($"❌ Inner: {ex.InnerException?.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SaveChanges Error: {ex.Message}");
                throw;
            }
        }

        // ============================
        // Dispose
        // ============================
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
