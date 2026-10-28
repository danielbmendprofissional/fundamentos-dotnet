using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Catalogo.Categorias.Persistence.QueryModels;
using Vendas.Infrastructure.Catalogo.Produtos.Persistence.QueryModels;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.Mappings
{
    public sealed class ProdutoResumoMap
        : IEntityTypeConfiguration<ProdutoResumoModel>
    {
        public void Configure(EntityTypeBuilder<ProdutoResumoModel> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome);
            builder.Property(p => p.Preco);
            builder.Property(p => p.Status);
            builder.Property(p => p.Descricao);
        }
    }
}
