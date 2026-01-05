using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Infrastructure.Persistence.Mappings.Base;

namespace Vendas.Infrastructure.Persistence.Mappings
{
    internal sealed class CategoriaMap : EntityMap<Categoria>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("CATEGORIA");

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Descricao)
                .HasMaxLength(500);

            builder.Property(c => c.Ativa)
                .IsRequired();
        }
    }
}
