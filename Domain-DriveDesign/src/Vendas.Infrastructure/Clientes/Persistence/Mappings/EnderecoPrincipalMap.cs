using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Infrastructure.Clientes.Persistence.QueryModels;

namespace Vendas.Infrastructure.Clientes.Persistence.Mappings
{
    internal sealed class EnderecoPrincipalMap
        : IEntityTypeConfiguration<EnderecoPrincipalModel>
    {
        public void Configure(EntityTypeBuilder<EnderecoPrincipalModel> builder) 
        {
            builder.ToTable("Enderecos");

            builder.HasKey(p => p.Id);

            builder.Property(e => e.ClienteId);
            builder.Property(e => e.Cep);
            builder.Property(e => e.Logradouro);
            builder.Property(e => e.Numero);
            builder.Property(e => e.Bairro);
            builder.Property(e => e.Cidade);
            builder.Property(e => e.Estado);
            builder.Property(e => e.Pais);
            builder.Property(e => e.Complemento);
        }
    }
}
