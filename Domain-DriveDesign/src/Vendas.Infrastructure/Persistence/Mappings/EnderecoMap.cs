using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Clientes.Entities;
using Vendas.Infrastructure.Persistence.Mappings.Base;


namespace Vendas.Infrastructure.Persistence.Mappings
{
    internal sealed class EnderecoMap : EntityMap<Endereco>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("Enderecos");

            builder.Property(e => e.Cep)
                .IsRequired()
                .HasMaxLength(8);

            builder.Property(e => e.Logradouro)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Numero)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(e => e.Pais)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Complemento)
                .HasMaxLength(200);
        }
    }
}
