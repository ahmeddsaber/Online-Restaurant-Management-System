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
        private readonly IImageService _imageService;

        public MenuCategoryService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<AdminCategoryDto?> CreateCategory(AdminCreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NameEn) && string.IsNullOrWhiteSpace(dto.NameAr))
                return null;

            var entity = new MenuCategory
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                DescriptionEn = dto.DescriptionEn,
                DescriptionAr = dto.DescriptionAr,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive
            };

            // ✅ Handle image upload safely
            if (dto.ImageFile != null)
            {
                entity.ImageUrl = await _imageService.UploadAsync(dto.ImageFile, "categories");
            }

            await _unitOfWork.MenuCategoryRepo.Create(entity);
            await _unitOfWork.SaveChangesAsync();

            return new AdminCategoryDto
            {
                Id = entity.Id,
                NameEn = entity.NameEn,
                NameAr = entity.NameAr,
                DescriptionEn = entity.DescriptionEn,
                DescriptionAr = entity.DescriptionAr,
                ImageUrl = entity.ImageUrl,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                MenuItemsCount = entity.MenuItems.Count,
                AvailableItemsCount = entity.MenuItems.Count(m => m.IsAvailable && !m.IsDeleted)
            };
        }

        public async Task<AdminCategoryDto?> UpdateCategory(AdminUpdateCategoryDto dto)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetById(dto.Id);
            if (category == null) return null;

            // ✅ SAFE PARTIAL UPDATE - Preserve critical fields
            category.NameEn = dto.NameEn;
            category.NameAr = dto.NameAr;
            category.DescriptionEn = dto.DescriptionEn;
            category.DescriptionAr = dto.DescriptionAr;
            category.DisplayOrder = dto.DisplayOrder;
            category.IsActive = dto.IsActive;

            // ✅ Only update image if new one is provided
            if (dto.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _imageService.DeleteAsync(category.ImageUrl);
                }
                // Upload new image
                category.ImageUrl = await _imageService.UploadAsync(dto.ImageFile, "categories");
            }
            // ✅ If no new image, existing ImageUrl is preserved

            _unitOfWork.MenuCategoryRepo.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return category.Adapt<AdminCategoryDto>();
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _unitOfWork.MenuCategoryRepo.GetById(id);
            if (category == null) return;

            // ✅ Delete associated image when deleting category
            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                await _imageService.DeleteAsync(category.ImageUrl);
            }

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

            if (category == null)
                return null;

            return new AdminCategoryDto
            {
                Id = category.Id,
                NameEn = category.NameEn,
                NameAr = category.NameAr,
                DescriptionEn = category.DescriptionEn,
                DescriptionAr = category.DescriptionAr,
                ImageUrl = category.ImageUrl,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
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

