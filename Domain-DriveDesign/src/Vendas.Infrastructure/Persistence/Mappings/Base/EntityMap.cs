using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Common.Base;

namespace Vendas.Infrastructure.Persistence.Mappings.Base
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

            builder.Property(e => e.DataCriacao)
                .IsRequired();

            builder.Property(e => e.DataAtualizacao);

            ConfigureMapping(builder);
        }

        protected abstract void ConfigureMapping(EntityTypeBuilder<TEntity> builder);
    }
}