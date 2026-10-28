using FluentAssertions;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.Enums;
using Galo.Domain.Torcida.Events;
using Galo.Domain.Torcida.ValueObjects;

namespace Galo.Domain.Tests.Torcida
{
    public class TorcedorTests
    {
        private static Torcedor Cadastrar()
            => Torcedor.Cadastrar(NomeCompleto.Create("Maria Atleticana"), Cpf.Create("529.982.247-25"),
                Email.Create("maria@galo.com"), Telefone.Create("31 99999-0000"), new DateOnly(1990, 5, 20));

        private static Guid AddEndereco(Torcedor t, string numero = "100", bool principal = false)
            => t.AdicionarEndereco("30130-000", "Avenida Afonso Pena", numero, "Centro", "Belo Horizonte", "mg", principal: principal);

        [Fact(DisplayName = "Cadastro cria torcedor ativo, sem plano, e evento de boas-vindas")]
        public void Deve_Cadastrar()
        {
            var t = Cadastrar();

            t.Status.Should().Be(StatusTorcedor.Ativo);
            t.Plano.Should().Be(PlanoSocio.Nenhum);
            t.DomainEvents.OfType<TorcedorCadastradoEvent>().Should().ContainSingle()
                .Which.Email.Should().Be("maria@galo.com");
        }

        [Fact(DisplayName = "Alterar plano gera evento e não aceita o mesmo plano")]
        public void Alterar_Plano()
        {
            var t = Cadastrar();
            t.ClearDomainEvents();

            t.AlterarPlano(PlanoSocio.Ouro);

            t.Plano.Should().Be(PlanoSocio.Ouro);
            t.DomainEvents.OfType<PlanoSocioAlteradoEvent>().Should().ContainSingle()
                .Which.Should().Match<PlanoSocioAlteradoEvent>(e => e.PlanoAnterior == PlanoSocio.Nenhum && e.PlanoNovo == PlanoSocio.Ouro);

            var act = () => t.AlterarPlano(PlanoSocio.Ouro);
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Torcedor bloqueado não altera plano")]
        public void Bloqueado_Nao_Altera_Plano()
        {
            var t = Cadastrar();
            t.Bloquear("Fraude");

            var act = () => t.AlterarPlano(PlanoSocio.Prata);
            act.Should().Throw<DomainException>().WithMessage("*bloqueado*");
        }

        [Fact(DisplayName = "O primeiro endereço vira o principal automaticamente")]
        public void Primeiro_Endereco_E_Principal()
        {
            var t = Cadastrar();
            var primeiro = AddEndereco(t);
            AddEndereco(t, "200");

            t.EnderecoPrincipalId.Should().Be(primeiro);
            t.Enderecos.Should().HaveCount(2);
            t.Enderecos.First().Estado.Should().Be("MG");   // normalizado em maiúsculas
        }

        [Fact(DisplayName = "Pode trocar o endereço principal, mas não remover o principal")]
        public void Troca_E_Remocao_De_Endereco_Principal()
        {
            var t = Cadastrar();
            var a = AddEndereco(t);
            var b = AddEndereco(t, "200");

            var remover = () => t.RemoverEndereco(a);
            remover.Should().Throw<DomainException>().WithMessage("*principal*");

            t.DefinirEnderecoPrincipal(b);
            t.EnderecoPrincipalId.Should().Be(b);

            t.RemoverEndereco(a);
            t.Enderecos.Should().ContainSingle();
        }

        [Fact(DisplayName = "Endereço de outro torcedor não pode ser definido como principal")]
        public void Endereco_Alheio()
        {
            var t = Cadastrar();
            var act = () => t.DefinirEnderecoPrincipal(Guid.NewGuid());
            act.Should().Throw<DomainException>();
        }

        [Theory(DisplayName = "CEP inválido deve ser recusado")]
        [InlineData("123")]
        [InlineData("abcdefgh")]
        public void Cep_Invalido(string cep)
        {
            var t = Cadastrar();
            var act = () => t.AdicionarEndereco(cep, "Rua A", "1", "Centro", "BH", "MG");
            act.Should().Throw<DomainException>();
        }
    }
}
