using Mapster;
using Restaurant.Application.Contract;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Application.DTOS.Customer;
using Restaurant.Application.DTOS.Manager;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Application.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminCategoryDto?> CreateCategory(AdminCreateCategoryDto dto)
        {
            var entity = dto.Adapt<MenuCategory>();

            if (string.IsNullOrWhiteSpace(entity.NameEn) && string.IsNullOrWhiteSpace(entity.NameAr))
                return null;

            await _unitOfWork.MenuCategoryRepo.Create(entity);
            await _unitOfWork.SaveChangesAsync();

            // ارجع DTO بعد الحفظ
            return entity.Adapt<AdminCategoryDto>();
        }



        public async Task<AdminCategoryDto?> UpdateCategory(AdminUpdateCategoryDto dto)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetById(dto.Id);
            if (category == null) return null;

            dto.Adapt(category);
            _unitOfWork.MenuCategoryRepo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Adapt<AdminCategoryDto>();
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetById(id);
            if (category == null) return;

            category.IsDeleted = true;
            _unitOfWork.MenuCategoryRepo.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<AdminCategoryDto>> GetAllCategories()
        {
            var categories = await _unitOfWork.MenuCategoryRepo.GetAllCategoriesAsync();
            return categories.Adapt<IEnumerable<AdminCategoryDto>>();
        }

        public async Task<IEnumerable<AdminCategoryDto>> GetActiveCategoriesAsync()
        {
            var categories = await _unitOfWork.MenuCategoryRepo.GetActiveCategoriesAsync();
            return categories.Adapt<IEnumerable<AdminCategoryDto>>();
        }

        public async Task<IEnumerable<AdminCategoryDto>> GetDeletedCategoriesAsync()
        {
            var categories = await _unitOfWork.MenuCategoryRepo.GetDeletedCategoriesAsync();
            return categories.Adapt<IEnumerable<AdminCategoryDto>>();
        }

        public async Task<AdminCategoryDto?> GetCategoryById(int id)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetById(id);
            return category?.Adapt<AdminCategoryDto>();
        }

        public async Task<AdminCategoryDto?> GetCategoryByIdWithItemsAsync(int id)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetCategoryByIdWithItemsAsync(id);
            return category?.Adapt<AdminCategoryDto>();
        }

        public async Task<IEnumerable<CategorySalesDto>> GetCategorySalesAsync()
        {
            return await _unitOfWork.MenuCategoryRepo.GetCategorySalesAsync();
        }

        public async Task<IEnumerable<AdminCategoryDto?>> SearchAdminCategory(string? name)
        {
            var categories = await _unitOfWork.MenuCategoryRepo.SearchAsync(name);
            return categories.Adapt<IEnumerable<AdminCategoryDto>>();
        }

        public async Task<IEnumerable<CustomerMenuCategoryDto>> GetActiveCategoriesforCustomerAsync()
        {
            var categories = await _unitOfWork.MenuCategoryRepo.GetActiveCategoriesAsync();

            var result = categories.Select(c => new CustomerMenuCategoryDto
            {
                Id = c.Id,
                NameEn = c.NameEn ?? string.Empty,
                NameAr = c.NameAr ?? string.Empty,
                DescriptionEn = c.DescriptionEn,
                DescriptionAr = c.DescriptionAr,
                ImageUrl = c.ImageUrl,
                DisplayOrder = c.DisplayOrder,
                MenuItems = c.MenuItems
                    .Where(mi => !mi.IsDeleted && mi.IsAvailable) 
                    .Select(mi => new CustomerMenuItemDto
                    {
                        Id = mi.Id,
                        NameEn = mi.NameEn ?? string.Empty,
                        NameAr = mi.NameAr ?? string.Empty,
                        DescriptionEn = mi.DescriptionEn,
                        DescriptionAr = mi.DescriptionAr,
                        Price = mi.Price,
                        IsAvailable = mi.IsAvailable,
                        PreparationTime = mi.PreparationTime,
                        ImageUrl = mi.ImageUrl,
                        CanOrder = mi.IsAvailable && !mi.IsDeleted 
                    }).ToList()
            });

            return result;
        }

    }
}
