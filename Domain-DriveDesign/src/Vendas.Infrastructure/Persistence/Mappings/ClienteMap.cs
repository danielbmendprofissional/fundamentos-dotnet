using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Infrastructure.Persistence.Mappings.Base;

namespace Vendas.Infrastructure.Persistence.Mappings
{
    internal sealed class ClienteMap : EntityMap<Cliente>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            // Enums
            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.Sexo)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.EstadoCivil)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.EnderecoPrincipalId)
                .IsRequired();

            // NomeCompleto (ValueObject)
            builder.OwnsOne(c => c.Nome, nome =>
            {
                nome.Property(n => n.Nome)
                    .HasColumnName("Nome")
                    .IsRequired()
                    .HasMaxLength(150);

                nome.Property(n => n.Sobrenome)
                    .HasColumnName("Sobrenome")
                    .IsRequired()
                    .HasMaxLength(150);

                nome.Property(n => n.NomeCompletoFormatado)
                    .HasColumnName("NomeCompleto")
                    .IsRequired()
                    .HasMaxLength(300);
            });

            // CPF
            builder.OwnsOne(c => c.Cpf, cpf =>
            {
                cpf.Property(c => c.Numero)
                    .HasColumnName("Cpf")
                    .IsRequired()
                    .HasMaxLength(11);

                cpf.HasIndex(c => c.Numero).IsUnique();
            });

            // Email
            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Endereco)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(e => e.Endereco).IsUnique();
            });

            // Telefone
            builder.OwnsOne(c => c.Telefone, telefone =>
            {
                telefone.Property(t => t.Numero)
                    .HasColumnName("Telefone")
                    .IsRequired()
                    .HasMaxLength(11);
            });

            // Navegação privada (_enderecos)
            builder.HasMany("_enderecos")
                .WithOne()
                .HasForeignKey("ClienteId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata
                .FindNavigation("_enderecos")!
                .SetPropertyAccessMode(PropertyAccessMode.Field);


        }
    }
}
