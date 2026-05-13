using Mapster;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Extensions
{
    public static class SafeMappingExtensions
    {
        public static TEntity SafeAdapt<TDto, TEntity>(
            this TDto source,
            TEntity destination)
            where TEntity : BaseEntity
        {
            if (source == null)
                return destination;

            // 🔥 اعمل config صح
            var config = new TypeAdapterConfig();

            config.ForType<TDto, TEntity>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted)
                .Ignore(dest => dest.DeletedAt)
                .IgnoreNullValues(true);

            return source.Adapt(destination, config);
        }
    }
}