using FluentAssertions;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Competicoes.Entities;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Competicoes.Events;

namespace Galo.Domain.Tests.Competicoes
{
    public class PartidaTests
    {
        private static readonly Guid Campeonato = Guid.NewGuid();
        private static readonly Guid Galo = Guid.NewGuid();
        private static readonly Guid Rival = Guid.NewGuid();
        private static readonly Guid Estadio = Guid.NewGuid();

        private static Partida Agendar()
            => Partida.Agendar(Campeonato, Galo, Rival, Estadio, DateTime.UtcNow.AddDays(7), rodada: 5);

        private static Partida EmAndamento()
        {
            var p = Agendar();
            p.Iniciar();
            p.ClearDomainEvents();
            return p;
        }

        [Fact(DisplayName = "Deve agendar partida com placar zerado e evento")]
        public void Deve_Agendar()
        {
            var p = Agendar();

            p.Status.Should().Be(StatusPartida.Agendada);
            p.Placar.ToString().Should().Be("0 x 0");
            p.DomainEvents.Should().ContainSingle(e => e is PartidaAgendadaEvent);
        }

        [Fact(DisplayName = "Mandante e visitante devem ser diferentes")]
        public void Mandante_Diferente_De_Visitante()
        {
            var act = () => Partida.Agendar(Campeonato, Galo, Galo, Estadio, DateTime.UtcNow.AddDays(1), 1);
            act.Should().Throw<DomainException>().WithMessage("*diferentes*");
        }

        [Fact(DisplayName = "Gol atualiza o placar do clube beneficiado e gera evento")]
        public void Gol_Atualiza_Placar()
        {
            var p = EmAndamento();

            p.RegistrarGol(Galo, Guid.NewGuid(), 12);
            p.RegistrarGol(Rival, null, 40);
            p.RegistrarGol(Galo, Guid.NewGuid(), 77, TipoGol.Penalti);

            p.Placar.GolsMandante.Should().Be(2);
            p.Placar.GolsVisitante.Should().Be(1);
            p.Gols.Should().HaveCount(3);
            p.DomainEvents.OfType<GolMarcadoEvent>().Should().HaveCount(3);
            p.DomainEvents.OfType<GolMarcadoEvent>().Last().Should().Match<GolMarcadoEvent>(e => e.PlacarMandante == 2 && e.PlacarVisitante == 1);
        }

        [Fact(DisplayName = "Gol só pode ser registrado com a partida em andamento")]
        public void Gol_Exige_Partida_Em_Andamento()
        {
            var p = Agendar();
            var act = () => p.RegistrarGol(Galo, null, 10);
            act.Should().Throw<DomainException>().WithMessage("*em andamento*");
        }

        [Fact(DisplayName = "Gol de clube que não participa da partida deve ser recusado")]
        public void Gol_De_Clube_Estranho()
        {
            var p = EmAndamento();
            var act = () => p.RegistrarGol(Guid.NewGuid(), null, 10);
            act.Should().Throw<DomainException>().WithMessage("*não participa*");
        }

        [Fact(DisplayName = "Encerrar gera evento com o placar final e bloqueia novos gols")]
        public void Encerrar_Partida()
        {
            var p = EmAndamento();
            p.RegistrarGol(Rival, null, 90);
            p.ClearDomainEvents();

            p.Encerrar();

            p.Status.Should().Be(StatusPartida.Encerrada);
            p.DomainEvents.OfType<PartidaEncerradaEvent>().Should().ContainSingle()
                .Which.Should().Match<PartidaEncerradaEvent>(e => e.GolsMandante == 0 && e.GolsVisitante == 1);

            var act = () => p.RegistrarGol(Galo, null, 91);
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Partida encerrada não pode ser cancelada")]
        public void Encerrada_Nao_Cancela()
        {
            var p = EmAndamento();
            p.Encerrar();
            var act = () => p.Cancelar("Chuva");
            act.Should().Throw<DomainException>();
        }

        // ---------------- ingressos (estoque dos setores) ----------------

        [Fact(DisplayName = "Setores: nome único (sem diferenciar maiúsculas)")]
        public void Setor_Nome_Unico()
        {
            var p = Agendar();
            p.AdicionarSetor("Norte", 100, 50m);

            var act = () => p.AdicionarSetor("NORTE", 10, 10m);
            act.Should().Throw<DomainException>().WithMessage("*já existe*");
        }

        [Fact(DisplayName = "Reservar baixa o estoque e liberar devolve")]
        public void Reservar_E_Liberar()
        {
            var p = Agendar();
            p.AdicionarSetor("Norte", 10, 50m);

            p.ReservarIngressos("norte", 4);
            p.ObterSetor("Norte").Disponiveis.Should().Be(6);
            p.IngressosDisponiveis.Should().Be(6);

            p.LiberarIngressos("Norte", 3);
            p.ObterSetor("Norte").Disponiveis.Should().Be(9);
        }

        [Fact(DisplayName = "Não deve reservar mais ingressos do que o disponível")]
        public void Nao_Reserva_Alem_Do_Estoque()
        {
            var p = Agendar();
            p.AdicionarSetor("Camarote", 2, 600m);

            var act = () => p.ReservarIngressos("Camarote", 3);

            act.Should().Throw<DomainException>().WithMessage("*insuficientes*");
            p.ObterSetor("Camarote").Disponiveis.Should().Be(2);   // estado preservado
        }

        [Fact(DisplayName = "Não deve liberar além da capacidade do setor")]
        public void Nao_Libera_Alem_Da_Capacidade()
        {
            var p = Agendar();
            p.AdicionarSetor("Sul", 5, 40m);

            var act = () => p.LiberarIngressos("Sul", 1);
            act.Should().Throw<DomainException>().WithMessage("*capacidade*");
        }

        [Fact(DisplayName = "Só se reserva ingresso de partida agendada")]
        public void Reserva_Exige_Partida_Agendada()
        {
            var p = Agendar();
            p.AdicionarSetor("Norte", 10, 50m);
            p.Iniciar();

            var act = () => p.ReservarIngressos("Norte", 1);
            act.Should().Throw<DomainException>().WithMessage("*agendadas*");
        }

        [Fact(DisplayName = "Setor inexistente deve falhar com mensagem clara")]
        public void Setor_Inexistente()
        {
            var p = Agendar();
            var act = () => p.ReservarIngressos("Fantasma", 1);
            act.Should().Throw<DomainException>().WithMessage("*não encontrado*");
        }
    }
}
