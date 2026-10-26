using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Competicoes.Enums;
using Galo.Domain.Competicoes.Events;
using Galo.Domain.Competicoes.ValueObjects;

namespace Galo.Domain.Competicoes.Entities
{
    /// <summary>
    /// Aggregate Root: Partida + seus Gols + os Setores (estoque de ingressos).
    /// Referencia outros agregados (Campeonato, Clube, Estadio) APENAS por Id - nunca por navegação.
    /// </summary>
    public sealed class Partida : AggregateRoot
    {
        public Guid CampeonatoId { get; private set; }
        public Guid MandanteId { get; private set; }
        public Guid VisitanteId { get; private set; }
        public Guid EstadioId { get; private set; }
        public DateTime DataHoraUtc { get; private set; }
        public int Rodada { get; private set; }
        public StatusPartida Status { get; private set; }
        public Placar Placar { get; private set; } = null!;

        private readonly List<GolPartida> _gols = new();
        public IReadOnlyCollection<GolPartida> Gols => _gols.AsReadOnly();

        private readonly List<SetorPartida> _setores = new();
        public IReadOnlyCollection<SetorPartida> Setores => _setores.AsReadOnly();

        private Partida() { } // EF Core

        private Partida(Guid campeonatoId, Guid mandanteId, Guid visitanteId, Guid estadioId, DateTime dataHoraUtc, int rodada)
        {
            Guard.AgainstEmptyGuid(campeonatoId, nameof(campeonatoId));
            Guard.AgainstEmptyGuid(mandanteId, nameof(mandanteId));
            Guard.AgainstEmptyGuid(visitanteId, nameof(visitanteId));
            Guard.AgainstEmptyGuid(estadioId, nameof(estadioId));
            Guard.Against<DomainException>(mandanteId == visitanteId, "Mandante e visitante devem ser clubes diferentes.");
            Guard.Against<DomainException>(rodada <= 0, "A rodada deve ser maior que zero.");

            CampeonatoId = campeonatoId;
            MandanteId = mandanteId;
            VisitanteId = visitanteId;
            EstadioId = estadioId;
            DataHoraUtc = DateTime.SpecifyKind(dataHoraUtc, DateTimeKind.Utc);
            Rodada = rodada;
            Status = StatusPartida.Agendada;
            Placar = Placar.Zerado();

            AddDomainEvent(new PartidaAgendadaEvent(Id, mandanteId, visitanteId, DataHoraUtc));
        }

        public static Partida Agendar(Guid campeonatoId, Guid mandanteId, Guid visitanteId, Guid estadioId,
            DateTime dataHoraUtc, int rodada)
            => new(campeonatoId, mandanteId, visitanteId, estadioId, dataHoraUtc, rodada);

        public int IngressosDisponiveis => _setores.Sum(s => s.Disponiveis);

        // ---------- Ingressos (estoque) ----------

        public Guid AdicionarSetor(string nome, int capacidade, decimal preco)
        {
            Guard.Against<DomainException>(Status != StatusPartida.Agendada,
                "Setores só podem ser adicionados enquanto a partida está agendada.");
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Guard.Against<DomainException>(
                _setores.Any(s => s.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase)),
                "Já existe um setor com este nome.");

            var setor = new SetorPartida(Id, nome, capacidade, preco);
            _setores.Add(setor);
            SetDataAtualizacao();
            return setor.Id;
        }

        public SetorPartida ObterSetor(string nome)
        {
            var setor = _setores.FirstOrDefault(s => s.Nome.Equals(nome?.Trim(), StringComparison.OrdinalIgnoreCase));
            Guard.Against<DomainException>(setor is null, $"Setor '{nome}' não encontrado nesta partida.");
            return setor!;
        }

        public void ReservarIngressos(string nomeSetor, int quantidade)
        {
            Guard.Against<DomainException>(Status != StatusPartida.Agendada,
                "Só é possível reservar ingressos de partidas agendadas.");

            ObterSetor(nomeSetor).Reservar(quantidade);
            SetDataAtualizacao();
        }

        public void LiberarIngressos(string nomeSetor, int quantidade)
        {
            ObterSetor(nomeSetor).Liberar(quantidade);
            SetDataAtualizacao();
        }

        // ---------- Ciclo da partida ----------

        public void Iniciar()
        {
            Guard.Against<DomainException>(Status != StatusPartida.Agendada, "A partida não está agendada.");

            Status = StatusPartida.EmAndamento;
            SetDataAtualizacao();
            AddDomainEvent(new PartidaIniciadaEvent(Id));
        }

        /// <param name="clubeBeneficiadoId">Clube que ganha o gol (no gol contra, é o adversário do autor).</param>
        public void RegistrarGol(Guid clubeBeneficiadoId, Guid? jogadorId, int minuto, TipoGol tipo = TipoGol.Normal)
        {
            Guard.Against<DomainException>(Status != StatusPartida.EmAndamento,
                "Gols só podem ser registrados com a partida em andamento.");
            Guard.Against<DomainException>(clubeBeneficiadoId != MandanteId && clubeBeneficiadoId != VisitanteId,
                "O clube do gol não participa desta partida.");

            _gols.Add(new GolPartida(Id, clubeBeneficiadoId, jogadorId, minuto, tipo));
            Placar = clubeBeneficiadoId == MandanteId ? Placar.GolMandante() : Placar.GolVisitante();
            SetDataAtualizacao();

            AddDomainEvent(new GolMarcadoEvent(Id, clubeBeneficiadoId, jogadorId, minuto, tipo,
                Placar.GolsMandante, Placar.GolsVisitante));
        }

        public void Encerrar()
        {
            Guard.Against<DomainException>(Status != StatusPartida.EmAndamento, "A partida não está em andamento.");

            Status = StatusPartida.Encerrada;
            SetDataAtualizacao();
            AddDomainEvent(new PartidaEncerradaEvent(Id, CampeonatoId, MandanteId, VisitanteId,
                Placar.GolsMandante, Placar.GolsVisitante));
        }

        public void Cancelar(string motivo)
        {
            Guard.AgainstNullOrWhiteSpace(motivo, nameof(motivo));
            Guard.Against<DomainException>(Status == StatusPartida.Encerrada, "Partidas encerradas não podem ser canceladas.");
            Guard.Against<DomainException>(Status == StatusPartida.Cancelada, "A partida já está cancelada.");

            Status = StatusPartida.Cancelada;
            SetDataAtualizacao();
            AddDomainEvent(new PartidaCanceladaEvent(Id, motivo.Trim()));
        }
    }
}
