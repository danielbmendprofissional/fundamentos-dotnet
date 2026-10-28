using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.Mappings
{
    internal sealed class ImagemProdutoMap : IEntityTypeConfiguration<ImagemProdutoModel>
    {
        public void Configure(EntityTypeBuilder<ImagemProdutoModel> builder)
        {
            // Apontamos para a mesma tabela física que o Command criou
            builder.ToTable("ProdutoImagens");

            // No Command usamos PK Composta (ProdutoId, Ordem)
            // Precisamos repetir aqui para o EF saber como rastrear (mesmo em NoTracking)
            builder.HasKey(i => new { i.ProdutoId, i.Ordem });

            builder.Property(i => i.Url)
                .HasColumnName("Url")
                .IsRequired();

            builder.Property(i => i.Ordem)
                .HasColumnName("Ordem")
                .IsRequired();

            builder.Property(i => i.ProdutoId)
                .HasColumnName("ProdutoId")
                .IsRequired();
        }
    }
}
