using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Catalogo.Categorias.Persistence.QueryModels;

namespace Vendas.Infrastructure.Catalogo.Categorias.Persistence.Mappings
{
    internal sealed class CategoriaDetalheMap
        : IEntityTypeConfiguration<CategoriaDetalheModel>
    {
        public void Configure(EntityTypeBuilder<CategoriaDetalheModel> builder)
        {
            builder.ToTable("CATEGORIA");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome);
            builder.Property(p => p.Descricao);
            builder.Property(p => p.Ativo);
        }
    }
}
