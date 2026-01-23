using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasDefaultValue("EGP");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentIntentId)
                    .HasMaxLength(200);

                entity.Property(e => e.ReceiptUrl)
                    .HasMaxLength(500);

                entity.HasOne(p => p.Order)
                    .WithMany()
                    .HasForeignKey(p => p.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.TransactionId)
                    .IsUnique()
                    .HasDatabaseName("IX_Payment_TransactionId");

                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_Payment_OrderId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Payment_Status");
            });

            modelBuilder.Entity<Payment>().HasQueryFilter(e => !e.IsDeleted);

            // ============================================================================
            // MenuCategory Configuration
            // ============================================================================
            modelBuilder.Entity<MenuCategory>(entity =>
            {


                entity.HasKey(e => e.Id);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(500);

                entity.Property(e => e.DescriptionAr)
                    .HasMaxLength(500);

                entity.Property(e => e.DisplayOrder)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasMany(c => c.MenuItems)
                    .WithOne(m => m.Category)
                    .HasForeignKey(m => m.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.DisplayOrder)
                    .HasDatabaseName("IX_MenuCategory_DisplayOrder");

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("IX_MenuCategory_IsActive");
            });

            // ============================================================================
            // MenuItem Configuration
            // ============================================================================
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameAr)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DescriptionEn)
                    .HasMaxLength(1000);

                entity.Property(e => e.DescriptionAr)
                    .HasMaxLength(1000);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(e => e.PreparationTime)
                    .IsRequired();

                entity.Property(e => e.DailyOrderCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(m => m.Category)
                    .WithMany(c => c.MenuItems)
                    .HasForeignKey(m => m.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(m => m.OrderItems)
                    .WithOne(oi => oi.MenuItem)
                    .HasForeignKey(oi => oi.MenuItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("IX_MenuItem_CategoryId");

                entity.HasIndex(e => e.IsAvailable)
                    .HasDatabaseName("IX_MenuItem_IsAvailable");

                entity.HasIndex(e => new { e.CategoryId, e.IsAvailable })
                    .HasDatabaseName("IX_MenuItem_CategoryId_IsAvailable");
            });

            // ============================================================================
            // Table Configuration
            // ============================================================================
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TableNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Capacity)
                    .IsRequired();

                entity.Property(e => e.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(e => e.Location)
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasMany(t => t.Orders)
                    .WithOne(o => o.Table)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.TableNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Table_TableNumber");

                entity.HasIndex(e => e.IsAvailable)
                    .HasDatabaseName("IX_Table_IsAvailable");
            });

            // ============================================================================
            // Order Configuration
            // ============================================================================
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrderType)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(OrderStatus.Pending);

                entity.Property(e => e.Subtotal)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.TaxAmount)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountAmount)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.Total)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(500);

                entity.Property(e => e.SpecialInstructions)
                    .HasMaxLength(1000);

                entity.Property(e => e.OrderDate)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(o => o.Customer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Table)
                    .WithMany(t => t.Orders)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_Order_OrderNumber");

                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("IX_Order_CustomerId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Order_Status");

                entity.HasIndex(e => e.OrderDate)
                    .HasDatabaseName("IX_Order_OrderDate");

                entity.HasIndex(e => new { e.Status, e.CustomerId })
                    .HasDatabaseName("IX_Order_Status_CustomerId");

                entity.HasIndex(e => new { e.OrderDate, e.Status })
                    .HasDatabaseName("IX_Order_OrderDate_Status");
            });

            // ============================================================================
            // OrderItem Configuration
            // ============================================================================
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.UnitPrice)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.SpecialInstructions)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.MenuItem)
                    .WithMany(m => m.OrderItems)
                    .HasForeignKey(oi => oi.MenuItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("IX_OrderItem_OrderId");

                entity.HasIndex(e => e.MenuItemId)
                    .HasDatabaseName("IX_OrderItem_MenuItemId");
            });

            // ============================================================================
            // ApplicationUser Configuration
            // ============================================================================
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PreferredLanguage)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasDefaultValue("en");

                entity.Property(e => e.Address)
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                // Relationships
                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_ApplicationUser_Email");

                entity.HasIndex(e => e.PreferredLanguage)
                    .HasDatabaseName("IX_ApplicationUser_PreferredLanguage");
            });

            // ============================================================================
            // Global Query Filters - Soft Delete
            // ============================================================================
            modelBuilder.Entity<MenuCategory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<MenuItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Table>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ApplicationUser>().HasQueryFilter(e => !e.IsDeleted);
        }

        // ============================================================================
        // SaveChangesAsync Override - Audit & Soft Delete
        // ============================================================================
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = null;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = null;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;
                }
            }

            return base.SaveChanges();
        }
    }
}
