using FluentAssertions;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Elenco.Entities;
using Galo.Domain.Elenco.Enums;
using Galo.Domain.Elenco.Events;
using Galo.Domain.Elenco.ValueObjects;

namespace Galo.Domain.Tests.Elenco
{
    public class JogadorTests
    {
        private static Jogador CriarJogador(int idade = 25)
            => Jogador.Criar(
                NomeJogador.Create("Lucas Silva"), "Lucão", NumeroCamisa.Create(9), Posicao.Atacante,
                DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-idade).AddDays(-10)), "Brasil", ValorMercado.Create(5_000_000m));

        [Fact(DisplayName = "Deve criar jogador ativo e registrar o evento de cadastro")]
        public void Deve_Criar_Jogador_Ativo()
        {
            var jogador = CriarJogador();

            jogador.Status.Should().Be(StatusJogador.Ativo);
            jogador.Id.Should().NotBeEmpty();
            jogador.Contratos.Should().BeEmpty();
            jogador.DomainEvents.Should().ContainSingle(e => e is JogadorCadastradoEvent);
        }

        [Fact(DisplayName = "Não deve criar jogador com menos de 14 anos")]
        public void Nao_Deve_Criar_Jogador_Muito_Novo()
        {
            var act = () => CriarJogador(idade: 10);
            act.Should().Throw<DomainException>().WithMessage("*14 anos*");
        }

        [Fact(DisplayName = "Deve calcular a idade considerando o aniversário")]
        public void Deve_Calcular_Idade()
        {
            var jogador = Jogador.Criar(NomeJogador.Create("Gabriel Souza"), null, NumeroCamisa.Create(10), Posicao.Meia,
                new DateOnly(2000, 6, 15), "Brasil", ValorMercado.Create(0));

            jogador.CalcularIdade(new DateOnly(2026, 6, 14)).Should().Be(25);   // dia antes do aniversário
            jogador.CalcularIdade(new DateOnly(2026, 6, 15)).Should().Be(26);   // no aniversário
        }

        [Fact(DisplayName = "Atualizar valor de mercado deve gerar evento com o valor anterior e o novo")]
        public void Deve_Gerar_Evento_Ao_Atualizar_Valor()
        {
            var jogador = CriarJogador();
            jogador.ClearDomainEvents();

            jogador.AtualizarValorMercado(ValorMercado.Create(8_000_000m));

            jogador.ValorMercado.Valor.Should().Be(8_000_000m);
            jogador.DataAtualizacao.Should().NotBeNull();
            jogador.DomainEvents.OfType<ValorMercadoAtualizadoEvent>().Should().ContainSingle()
                .Which.Should().Match<ValorMercadoAtualizadoEvent>(e => e.ValorAnterior == 5_000_000m && e.ValorNovo == 8_000_000m);
        }

        [Fact(DisplayName = "Deve registrar contrato e localizar o contrato vigente")]
        public void Deve_Registrar_Contrato()
        {
            var jogador = CriarJogador();
            var inicio = new DateOnly(2025, 1, 1);

            var contratoId = jogador.RegistrarContrato(inicio, inicio.AddYears(2), 300_000m, 100_000_000m);

            jogador.Contratos.Should().ContainSingle(c => c.Id == contratoId);
            jogador.ContratoVigente(new DateOnly(2026, 1, 1)).Should().NotBeNull();
            jogador.ContratoVigente(new DateOnly(2028, 1, 1)).Should().BeNull();
        }

        [Fact(DisplayName = "Não deve registrar contratos com períodos sobrepostos")]
        public void Nao_Deve_Registrar_Contratos_Sobrepostos()
        {
            var jogador = CriarJogador();
            jogador.RegistrarContrato(new DateOnly(2025, 1, 1), new DateOnly(2027, 1, 1), 100_000m);

            var act = () => jogador.RegistrarContrato(new DateOnly(2026, 6, 1), new DateOnly(2028, 6, 1), 120_000m);

            act.Should().Throw<DomainException>().WithMessage("*sobrepõe*");
        }

        [Theory(DisplayName = "Não deve registrar contrato inválido")]
        [InlineData(2027, 2026, 100_000)]   // fim antes do início
        [InlineData(2026, 2028, 0)]         // salário zero
        [InlineData(2026, 2028, -10)]       // salário negativo
        public void Nao_Deve_Registrar_Contrato_Invalido(int anoInicio, int anoFim, decimal salario)
        {
            var jogador = CriarJogador();
            var act = () => jogador.RegistrarContrato(new DateOnly(anoInicio, 1, 1), new DateOnly(anoFim, 1, 1), salario);
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Lesionar e recuperar devem respeitar o estado do jogador")]
        public void Deve_Lesionar_E_Recuperar()
        {
            var jogador = CriarJogador();

            jogador.Lesionar("Entorse no tornozelo");
            jogador.Status.Should().Be(StatusJogador.Lesionado);

            var lesionarDeNovo = () => jogador.Lesionar("Outra lesão");
            lesionarDeNovo.Should().Throw<DomainException>();

            jogador.Recuperar();
            jogador.Status.Should().Be(StatusJogador.Ativo);

            var recuperarDeNovo = () => jogador.Recuperar();
            recuperarDeNovo.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Jogador transferido não pode receber novo contrato")]
        public void Transferido_Nao_Recebe_Contrato()
        {
            var jogador = CriarJogador();
            jogador.Transferir();

            var act = () => jogador.RegistrarContrato(new DateOnly(2026, 1, 1), new DateOnly(2027, 1, 1), 50_000m);
            act.Should().Throw<DomainException>();
        }
    }
}
