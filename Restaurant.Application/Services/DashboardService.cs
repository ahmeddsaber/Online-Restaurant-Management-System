using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Common;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.DTOS.Staff;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Enums;

namespace Restaurant.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponseDto<AdminDashboardDto>> GetAdminDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var allOrders = _unitOfWork.Order.GetAll();
            var totalOrders = await allOrders.CountAsync();
            var pendingOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Pending);
            var totalRevenue = await allOrders.Where(o => o.Status == OrderStatus.Delivered).SumAsync(o => o.Total);
            var todayRevenue = await allOrders
                .Where(o => o.OrderDate.Date == today && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Total);
            var monthRevenue = await allOrders
                .Where(o => o.OrderDate >= monthStart && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Total);

            var totalMenuItems = await _unitOfWork.MenuItem.GetAll().CountAsync();
            var totalCategories = await _unitOfWork.MenuCategoryRepo.GetAll().CountAsync();

            // Recent orders
            var recentOrders = await allOrders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.Customer.FirstName + " " + o.Customer.LastName,
                    CustomerEmail = o.Customer.Email ?? string.Empty,
                    Total = o.Total,
                    Status = o.Status.ToString(),
                    OrderType = o.OrderType.ToString(),
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            // Top selling items
            var topItems = await _unitOfWork.OrderItem.GetAll()
                .Include(oi => oi.MenuItem)
                .GroupBy(oi => new { oi.MenuItemId, oi.MenuItem.NameEn, oi.MenuItem.NameAr, oi.MenuItem.ImageUrl })
                .Select(g => new TopSellingItemDto
                {
                    MenuItemId = g.Key.MenuItemId,
                    NameEn = g.Key.NameEn,
                    NameAr = g.Key.NameAr,
                    OrderCount = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice),
                    ImageUrl = g.Key.ImageUrl
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(5)
                .ToListAsync();

            // Revenue chart (last 7 days)
            var revenueChart = await allOrders
                .Where(o => o.OrderDate.Date >= today.AddDays(-6) && o.Status == OrderStatus.Delivered)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RevenueByDayDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.Total),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var dashboard = new AdminDashboardDto
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                TotalMenuItems = totalMenuItems,
                TotalCategories = totalCategories,
                TotalRevenue = totalRevenue,
                TodayRevenue = todayRevenue,
                MonthRevenue = monthRevenue,
                RecentOrders = recentOrders,
                TopSellingItems = topItems,
                RevenueChart = revenueChart
            };

            return ApiResponseDto<AdminDashboardDto>.SuccessResponse(dashboard);
        }

        public async Task<ApiResponseDto<ManagerDashboardDto>> GetManagerDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var allOrders = _unitOfWork.Order.GetAll();

            var todayRevenue = await allOrders
                .Where(o => o.OrderDate.Date == today && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Total);

            var weekRevenue = await allOrders
                .Where(o => o.OrderDate.Date >= weekStart && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Total);

            var monthRevenue = await allOrders
                .Where(o => o.OrderDate >= monthStart && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Total);

            var todayOrders = await allOrders.CountAsync(o => o.OrderDate.Date == today);
            var pendingOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Pending);
            var preparingOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Preparing);

            var recentOrders = await allOrders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new RecentOrderSummaryDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    Total = o.Total,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            var topItems = await _unitOfWork.OrderItem.GetAll()
                .Include(oi => oi.MenuItem)
                .Where(oi => oi.Order.OrderDate.Date == today)
                .GroupBy(oi => new { oi.MenuItem.NameEn, oi.MenuItem.NameAr })
                .Select(g => new TopItemDto
                {
                    ItemNameEn = g.Key.NameEn,
                    ItemNameAr = g.Key.NameAr,
                    OrderCount = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(5)
                .ToListAsync();

            var dashboard = new ManagerDashboardDto
            {
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                TodayOrders = todayOrders,
                PendingOrders = pendingOrders,
                PreparingOrders = preparingOrders,
                RecentOrders = recentOrders,
                TopItems = topItems
            };

            return ApiResponseDto<ManagerDashboardDto>.SuccessResponse(dashboard);
        }

        public async Task<ApiResponseDto<StaffDashboardDto>> GetStaffDashboardAsync()
        {
            var allOrders = _unitOfWork.Order.GetAll();

            var pendingOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Pending);
            var preparingOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Preparing);
            var readyOrders = await allOrders.CountAsync(o => o.Status == OrderStatus.Ready);

            var allTables = _unitOfWork.Table.GetAll();
            var availableTables = await allTables.CountAsync(t => t.IsAvailable);
            var occupiedTables = await allTables.CountAsync(t => !t.IsAvailable);

            var activeOrders = await allOrders
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Preparing)
                .Include(o => o.Table)
                .OrderBy(o => o.OrderDate)
                .Take(20)
                .Select(o => new StaffOrderSummaryDto
                {
                    OrderId = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    TableNumber = o.Table != null ? o.Table.TableNumber : null,
                    ItemsCount = o.OrderItems.Count,
                    OrderDate = o.OrderDate
                })
                .ToListAsync();

            var dashboard = new StaffDashboardDto
            {
                PendingOrders = pendingOrders,
                PreparingOrders = preparingOrders,
                ReadyOrders = readyOrders,
                AvailableTables = availableTables,
                OccupiedTables = occupiedTables,
                ActiveOrders = activeOrders
            };

            return ApiResponseDto<StaffDashboardDto>.SuccessResponse(dashboard);
        }
    }
}

