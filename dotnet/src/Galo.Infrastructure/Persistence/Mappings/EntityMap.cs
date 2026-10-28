using Galo.Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Galo.Infrastructure.Persistence.Mappings
{
    /// <summary>
    /// Mapeamento base de TODA entidade do domínio (Fluent API, sem atributos no domínio = domínio livre de EF).
    /// </summary>
    internal abstract class EntityMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            // Id é gerado PELO DOMÍNIO (Guid.NewGuid no construtor), nunca pelo banco.
            // Isso é o que faz o EF tratar um filho novo (já com Id preenchido) como INSERT e não como UPDATE.
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.DataCriacao).IsRequired();
            builder.Property(e => e.DataAtualizacao);

            ConfigureMapping(builder);
        }

        protected abstract void ConfigureMapping(EntityTypeBuilder<TEntity> builder);
    }
}
