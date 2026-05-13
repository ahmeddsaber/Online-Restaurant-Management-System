using Mapster;
using Restaurant.Application.DTOS.Admin;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Application.Mapper
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            // ✅ Complete MenuCategory -> AdminCategoryDto mapping
            TypeAdapterConfig<MenuCategory, AdminCategoryDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.NameEn, src => src.NameEn)
                .Map(dest => dest.NameAr, src => src.NameAr)
                .Map(dest => dest.DescriptionEn, src => src.DescriptionEn)
                .Map(dest => dest.DescriptionAr, src => src.DescriptionAr)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl)
                .Map(dest => dest.DisplayOrder, src => src.DisplayOrder)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.MenuItemsCount, src => src.MenuItems.Count)
                .Map(dest => dest.AvailableItemsCount,
                     src => src.MenuItems.Count(m => m.IsAvailable && !m.IsDeleted));

            TypeAdapterConfig<MenuItem, MenuItem>
                .NewConfig()
                .Map(dest => dest.NameEn, src => src.NameEn)
                .Map(dest => dest.NameAr, src => src.NameAr)
                .TwoWays();
        }
    }
}

