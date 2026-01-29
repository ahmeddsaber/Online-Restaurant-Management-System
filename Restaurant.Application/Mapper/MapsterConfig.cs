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
            TypeAdapterConfig<MenuCategory, AdminCategoryDto>.NewConfig()
    .Map(dest => dest.MenuItemsCount, src => src.MenuItems.Count)
    .Map(dest => dest.AvailableItemsCount,
         src => src.MenuItems.Count(m => m.IsAvailable && !m.IsDeleted));

            TypeAdapterConfig<MenuItem, MenuItem>.NewConfig()
           .Map(dest => dest.NameEn, src => src.NameEn)
            .Map(dest => dest.NameAr, src => src.NameAr)
            .TwoWays();

        }
    }
}
