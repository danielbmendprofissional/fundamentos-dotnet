using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Events.Categorias;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Catalogo.Entities
{
    public class CategoriaTests
    {
        [Fact]
        public void CriarCategoria_DeveCriarAtivaComNomeValido()
        {
            // Arrange
            var nome = "Eletrônicos";

            // Act
            var categoria = Categoria.Criar(nome);

            // Assert
            categoria.Nome.Should().Be(nome);
            categoria.Ativa.Should().BeTrue();
            categoria.DataCriacao.Should().NotBe(default);
            categoria.Descricao.Should().BeNull();
            categoria.DomainEvents.Should().BeEmpty(); // nenhum evento é disparado no construtor
        }

        [Fact]
        public void CriarCategoria_ComNomeInvalido_DeveLancarDomainException()
        {
            // Arrange
            Action act = () => Categoria.Criar("ab");

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("Nome deve ter ao menos 3 caracteres.");
        }

        [Fact]
        public void CriarCategoria_ComNomeVazio_DeveLancarDomainException()
        {
            // Arrange
            Action act = () => Categoria.Criar("");

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("nome cannot be null or empty");
        }

        [Fact]
        public void AlterarNome_DeveAtualizarNomeEDataAtualizacao()
        {
            var categoria = Categoria.Criar("Acessórios");

            categoria.AlterarNome("Periféricos");

            categoria.Nome.Should().Be("Periféricos");
            categoria.DataAtualizacao.Should().NotBeNull();
        }

        [Fact]
        public void AlterarNome_ComNomeInvalido_DeveLancarDomainException()
        {
            var categoria = Categoria.Criar("Acessórios");

            Action act = () => categoria.AlterarNome("ab");

            act.Should()
                .Throw<DomainException>()
                .WithMessage("Nome deve ter ao menos 3 caracteres.");
        }

        [Fact]
        public void AlterarDescricao_DeveAtualizarDescricaoEDataAtualizacao()
        {
            var categoria = Categoria.Criar("Informática");

            categoria.AlterarDescricao("Tudo de tecnologia");

            categoria.Descricao.Should().Be("Tudo de tecnologia");
            categoria.DataAtualizacao.Should().NotBeNull();
        }

        [Fact]
        public void Ativar_DeveGerarEventoCategoriaAtivada()
        {
            // Arrange
            var categoria = Categoria.Criar("Jogos");
            categoria.Inativar();
            categoria.ClearDomainEvents(); // limpar eventos anteriores

            // Act
            categoria.Ativar();
            var events = categoria.DomainEvents;

            // Assert
            events.Should().ContainSingle()
                .Which.Should().BeOfType<CategoriaAtivadaEvent>();

            categoria.Ativa.Should().BeTrue();
        }

        [Fact]
        public void Ativar_QuandoJaAtiva_DeveLancarDomainException()
        {
            var categoria = Categoria.Criar("Roupas");

            Action act = () => categoria.Ativar();

            act.Should()
                .Throw<DomainException>()
                .WithMessage("Categoria já está ativa.");
        }

        [Fact]
        public void Inativar_DeveGerarEventoCategoriaInativada()
        {
            var categoria = Categoria.Criar("Livros");

            categoria.Inativar();
            var events = categoria.DomainEvents;

            events.Should().ContainSingle()
                .Which.Should().BeOfType<CategoriaInativadaEvent>();

            categoria.Ativa.Should().BeFalse();
        }

        [Fact]
        public void Inativar_QuandoJaInativa_DeveLancarDomainException()
        {
            var categoria = Categoria.Criar("Eletrodomésticos");
            categoria.Inativar();

            Action act = () => categoria.Inativar();

            act.Should()
                .Throw<DomainException>()
                .WithMessage("Categoria já está inativa.");
        }

        [Fact]
        public void DomainEvents_DeveSerPossivelLimparEventos()
        {
            var categoria = Categoria.Criar("Brinquedos");
            categoria.Inativar();

            categoria.DomainEvents.Should().HaveCount(1);

            categoria.ClearDomainEvents();

            categoria.DomainEvents.Should().BeEmpty();
        }

    }
}
