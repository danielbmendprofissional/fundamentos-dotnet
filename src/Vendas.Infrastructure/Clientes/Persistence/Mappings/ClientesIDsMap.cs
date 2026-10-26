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
    internal class ClientesIDsMap
        : IEntityTypeConfiguration<ClienteIDsModel>
    {
        public void Configure(EntityTypeBuilder<ClienteIDsModel> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(p => p.Id);

            builder.Property(e => e.EnderecoPrincipalId);
        }
    }
}
