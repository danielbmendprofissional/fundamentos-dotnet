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
    internal class ProdutoDetalhesMap
        : IEntityTypeConfiguration<ProdutoDetalhesModel>
    {
        public void Configure(EntityTypeBuilder<ProdutoDetalhesModel> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome);
            builder.Property(p => p.Codigo);
            builder.Property(p => p.Preco);
            builder.Property(p => p.Descricao);
            builder.Property(p => p.CategoriaId);
            builder.Property(p => p.Status);
            builder.Property(p => p.Estoque);

            builder.HasMany(p => p.Imagens)
               .WithOne()
               .HasForeignKey("ProdutoId");
        }
    }
}