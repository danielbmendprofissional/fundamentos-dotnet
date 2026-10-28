using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Onion.Domain.Common.Base;

namespace Onion.Infrastructure.Persistence.Common.Base
{
    internal abstract class EntityMap<TEntity>
            : IEntityTypeConfiguration<TEntity>
            where TEntity : Entity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever();

            builder.Property(e => e.CreateDate)
                .IsRequired();

            builder.Property(e => e.UpdateDate);

            ConfigureMapping(builder);

            builder.HasIndex(e => e.CreateDate);
            builder.HasIndex(e => e.UpdateDate);
            builder.Property<byte[]>("RowVersion").IsRowVersion();
        }

        protected abstract void ConfigureMapping(EntityTypeBuilder<TEntity> builder);
    }
}
