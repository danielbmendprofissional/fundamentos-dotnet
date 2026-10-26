using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Galo.Infrastructure.Persistence.Mappings
{
    internal sealed class TorcedorMap : EntityMap<Torcedor>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Torcedor> builder)
        {
            builder.ToTable("Torcedores");

            builder.Property(t => t.Nome)
                .HasConversion(v => v.NomeCompletoFormatado, s => NomeCompleto.Create(s))
                .HasMaxLength(150).IsRequired();

            builder.Property(t => t.Cpf)
                .HasConversion(v => v.Numero, s => Cpf.Create(s))
                .HasMaxLength(11).IsRequired();

            builder.Property(t => t.Email)
                .HasConversion(v => v.Endereco, s => Email.Create(s))
                .HasMaxLength(200).IsRequired();

            // Telefone é opcional: o EF não chama o conversor quando o valor é null.
            builder.Property(t => t.Telefone)
                .HasConversion(v => v!.Numero, s => Telefone.Create(s))
                .HasMaxLength(11);

            builder.Property(t => t.DataNascimento).IsRequired();
            builder.Property(t => t.Plano).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
            builder.Property(t => t.EnderecoPrincipalId);

            // Unicidade garantida pelo BANCO (a checagem no handler é só para dar mensagem amigável).
            builder.HasIndex(t => t.Cpf).IsUnique();
            builder.HasIndex(t => t.Email).IsUnique();
            builder.HasIndex(t => t.Plano);

            builder.HasMany(t => t.Enderecos).WithOne().HasForeignKey(e => e.TorcedorId).OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(t => t.Enderecos).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Ignore(t => t.DomainEvents);
        }
    }

    internal sealed class EnderecoMap : EntityMap<Endereco>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Endereco> builder)
        {
            builder.ToTable("Enderecos");
            builder.Property(e => e.Cep).HasMaxLength(8).IsRequired();
            builder.Property(e => e.Logradouro).HasMaxLength(150).IsRequired();
            builder.Property(e => e.Numero).HasMaxLength(20).IsRequired();
            builder.Property(e => e.Bairro).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Cidade).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Estado).HasMaxLength(2).IsRequired();
            builder.Property(e => e.Pais).HasMaxLength(60).IsRequired();
            builder.Property(e => e.Complemento).HasMaxLength(100);

            builder.HasIndex(e => e.TorcedorId);
        }
    }
}
