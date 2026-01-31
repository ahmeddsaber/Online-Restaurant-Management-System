using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.Exceptions;
using Restaurant.Application.Helpers;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ===================== Mapping =====================
        private static AdminMenuItemDto MapToAdmin(MenuItem m) => new()
        {
            Id = m.Id,
            NameEn = m.NameEn,
            NameAr = m.NameAr,
            DescriptionEn = m.DescriptionEn,
            DescriptionAr = m.DescriptionAr,
            Price = m.Price,
            IsAvailable = m.IsAvailable,
            PreparationTime = m.PreparationTime,
            DailyOrderCount = m.DailyOrderCount,
            ImageUrl = m.ImageUrl,
            CategoryId = m.CategoryId,
            CategoryNameEn = m.Category?.NameEn ?? "",
            CategoryNameAr = m.Category?.NameAr ?? "",
            CreatedAt = m.CreatedAt
        };

        private static CustomerMenuItemDto MapToCustomer(MenuItem m) => new()
        {
            Id = m.Id,
            NameEn = m.NameEn,
            NameAr = m.NameAr,
            DescriptionEn = m.DescriptionEn,
            DescriptionAr = m.DescriptionAr,
            Price = m.Price,
            IsAvailable = m.IsAvailable,
            PreparationTime = m.PreparationTime,
            ImageUrl = m.ImageUrl,
            CanOrder = m.CanOrder()
        };

        // ===================== Queries =====================

        public async Task<IEnumerable<AdminMenuItemDto>> GetAllMenuItemsAsync()
            => (await _unitOfWork.MenuItem.GetAllItems()).Select(MapToAdmin);

        public async Task<IEnumerable<AdminMenuItemDto>> GetAllItems()
            => await GetAllMenuItemsAsync();

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            Guard.AgainstInvalidRange(minPrice, maxPrice);
            return (await _unitOfWork.MenuItem.GetItemsByPriceRange(minPrice, maxPrice))
                .Select(MapToCustomer);
        }

        public async Task<IEnumerable<AdminMenuItemDto>> SearchItemsByNameforAdmin(string name)
        {
            Guard.AgainstNullOrEmpty(name);
            return (await _unitOfWork.MenuItem.SearchItemsByName(name.Trim()))
                .Select(MapToAdmin);
        }

        public async Task<IEnumerable<CustomerMenuItemDto>> SearchItemsByNameforCustomer(string name)
        {
            Guard.AgainstNullOrEmpty(name);
            return (await _unitOfWork.MenuItem.SearchItemsByName(name.Trim()))
                .Select(MapToCustomer);
        }

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsWithPreparationTimeLessThan(int minutes)
        {
            Guard.AgainstInvalidMinutes(minutes);
            return (await _unitOfWork.MenuItem.GetItemsWithPreparationTimeLessThan(minutes))
                .Select(MapToCustomer);
        }

        public async Task<IEnumerable<AdminMenuItemDto>> GetItemsWithCategory()
            => (await _unitOfWork.MenuItem.GetItemsWithCategory()).Select(MapToAdmin);

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsWithCategoryForCustomer()
            => (await _unitOfWork.MenuItem.GetItemsWithCategory()).Select(MapToCustomer);

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsSortedByPrice(bool ascending)
            => (await _unitOfWork.MenuItem.GetItemsSortedByPrice(ascending))
                .Select(MapToCustomer);

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsSortedByPreparationTime(bool ascending)
            => (await _unitOfWork.MenuItem.GetItemsSortedByPreparationTime(ascending))
                .Select(MapToCustomer);

        public async Task<IEnumerable<AdminMenuItemDto>> GetItemsAddedInLastNDays(int days)
        {
            Guard.AgainstInvalidDays(days);
            return (await _unitOfWork.MenuItem.GetItemsAddedInLastNDays(days))
                .Select(MapToAdmin);
        }

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsAddedInLastNDaysForCustomer(int days)
        {
            Guard.AgainstInvalidDays(days);
            return (await _unitOfWork.MenuItem.GetItemsAddedInLastNDays(days))
                .Select(MapToCustomer);
        }

        public async Task<IEnumerable<AdminMenuItemDto>> GetAvailableItems()
            => (await _unitOfWork.MenuItem.GetAvailableItems()).Select(MapToAdmin);

        public async Task<IEnumerable<CustomerMenuItemDto>> GetAvailableItemsForCustmer()
            => (await _unitOfWork.MenuItem.GetAvailableItems()).Select(MapToCustomer);

        public async Task<IEnumerable<AdminMenuItemDto>> GetNotAvailableItems()
            => (await _unitOfWork.MenuItem.GetNotAvailableItems()).Select(MapToAdmin);

        public async Task<IEnumerable<CustomerMenuItemDto>> GetItemsforCustomer()
            => (await _unitOfWork.MenuItem.GetItemsforCustomer()).Select(MapToCustomer);

        public async Task<TopItemDto> TopSellingItemDto()
        {
            var item = await _unitOfWork.MenuItem.TopSellingItemDto();
            if (item == null) return null;

            return new TopItemDto
            {
                ItemNameEn = item.NameEn,
                ItemNameAr = item.NameAr,
                OrderCount = item.DailyOrderCount
            };
        }

        // ===================== Get By Id =====================

        public async Task<AdminMenuItemDto> GetMenuItemByIdAsync(int id)
        {
            Guard.AgainstInvalidId(id);

            var item = await _unitOfWork.MenuItem.GetById(id);
            if (item == null || item.IsDeleted)
                throw new NotFoundException("Menu item not found");

            return MapToAdmin(item);
        }

        public async Task<CustomerMenuItemDto> GetMenuItemByIdAsyncForCustomer(int id)
        {
            Guard.AgainstInvalidId(id);

            var item = await _unitOfWork.MenuItem.GetById(id);
            if (item == null || item.IsDeleted || !item.IsAvailable)
                throw new NotFoundException("Menu item not available");

            return MapToCustomer(item);
        }

        // ===================== Commands =====================

        public async Task<AdminCreateMenuItemDto> CreateItemMenu(AdminCreateMenuItemDto dto)
        {
            Guard.AgainstNull(dto);
            Guard.AgainstInvalidPrice(dto.Price);
            Guard.AgainstInvalidMinutes(dto.PreparationTime);

            var entity = new MenuItem
            {
                NameEn = dto.NameEn.Trim(),
                NameAr = dto.NameAr.Trim(),
                DescriptionEn = dto.DescriptionEn,
                DescriptionAr = dto.DescriptionAr,
                Price = dto.Price,
                PreparationTime = dto.PreparationTime,
                CategoryId = dto.CategoryId,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.IsAvailable
            };

            await _unitOfWork.MenuItem.Create(entity);
            await _unitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task<AdminUpdateMenuItemDto> UpdateMenuItem(AdminUpdateMenuItemDto dto)
        {
            Guard.AgainstNull(dto);
            Guard.AgainstInvalidId(dto.Id);

            var item = await _unitOfWork.MenuItem.GetById(dto.Id);
            if (item == null || item.IsDeleted)
                throw new NotFoundException("Menu item not found");

            item.NameEn = dto.NameEn.Trim();
            item.NameAr = dto.NameAr.Trim();
            item.DescriptionEn = dto.DescriptionEn;
            item.DescriptionAr = dto.DescriptionAr;
            item.Price = dto.Price;
            item.PreparationTime = dto.PreparationTime;
            item.CategoryId = dto.CategoryId;
            item.ImageUrl = dto.ImageUrl;
            item.IsAvailable = dto.IsAvailable;

            _unitOfWork.MenuItem.Update(item);
            await _unitOfWork.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteMenuItem(int id)
        {
            Guard.AgainstInvalidId(id);

            var item = await _unitOfWork.MenuItem.GetById(id);
            if (item == null || item.IsDeleted)
                throw new NotFoundException("Menu item not found");

            item.IsDeleted = true;
            item.IsAvailable = false;

            _unitOfWork.MenuItem.Update(item);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
