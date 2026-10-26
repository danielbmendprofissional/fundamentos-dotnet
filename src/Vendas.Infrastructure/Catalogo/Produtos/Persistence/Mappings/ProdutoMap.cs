using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Infrastructure.Common.Persistence.Mappings;

namespace Vendas.Infrastructure.Catalogo.Produtos.Persistence.Mappings
{
    internal sealed class ProdutoMap : EntityMap<Produto>
    {
        protected override void ConfigureMapping(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            // Foreign Key
            builder.Property(p => p.CategoriaId)
                .IsRequired();

            builder.HasIndex(p => p.CategoriaId);

            builder.Property(p => p.Descricao)
                .HasMaxLength(500);

            builder.Property(p => p.Estoque)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            // NomeProduto (ValueObject)
            builder.OwnsOne(p => p.Nome, nome =>
            {
                nome.Property(n => n.Valor)
                    .HasColumnName("Nome")
                    .IsRequired()
                    .HasMaxLength(150);
            });

            // CodigoProduto (ValueObject)
            builder.OwnsOne(p => p.Codigo, codigo =>
            {
                codigo.Property(c => c.Valor)
                    .HasColumnName("Codigo")
                    .IsRequired()
                    .HasMaxLength(50);

                builder.HasIndex("Codigo").IsUnique();
            });

            // PrecoProduto (ValueObject)
            builder.OwnsOne(p => p.Preco, preco =>
            {
                preco.Property(p => p.Valor)
                    .HasColumnName("Preco")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
            });

            builder.OwnsMany(p => p.Imagens, imagens =>
            {
                imagens.ToTable("ProdutoImagens");

                // Deixando a FK explícita na tabela filha
                imagens.WithOwner().HasForeignKey("ProdutoId");

                // Definindo a PK da tabela para performance (Clustered Index)
                imagens.HasKey("ProdutoId", "Ordem");

                imagens.Property(i => i.Url)
                    .HasColumnName("Url") // Nome explícito
                    .IsRequired()
                    .HasMaxLength(500);

                imagens.Property(i => i.Ordem)
                    .HasColumnName("Ordem")
                    .IsRequired();

                var navigation = builder.Metadata.FindNavigation(nameof(Produto.Imagens));
                navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}
