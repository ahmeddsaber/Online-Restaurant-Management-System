using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.MenuItem;
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

        // دالة async
        public async Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync()
        {
            var items = _unitOfWork.MenuItem.GetAll(); // افترض إن GetAll() بيرجع IQueryable<MenuItem>

            var dtos = items.Select(i => new MenuItemDto
            {
               
                NameEn = i.NameEn,
                NameAr = i.NameAr,
                DescriptionEn = i.DescriptionEn,
                DescriptionAr = i.DescriptionAr,
                Price = i.Price,
                IsAvailable = i.IsAvailable,
                PreparationTime = i.PreparationTime,
                ImageUrl = i.ImageUrl,
                CategoryId = i.CategoryId,
                CategoryNameEn = i.Category.NameEn,
                CategoryNameAr = i.Category.NameAr
            });

            return await Task.FromResult(dtos); // لو DbContext مش async
            // لو DbContext من EF Core:
            // return await dtos.ToListAsync();
        }
    }
}
