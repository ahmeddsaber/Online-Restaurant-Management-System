using Mapster;
using Restaurant.Application.DTOS.MenuItem;
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
            TypeAdapterConfig<MenuItem, MenuItemDto>.NewConfig()
           .Map(dest => dest.NameEn, src => src.NameEn)
            .Map(dest => dest.NameAr, src => src.NameAr)
            .TwoWays();

        }
    }
}
