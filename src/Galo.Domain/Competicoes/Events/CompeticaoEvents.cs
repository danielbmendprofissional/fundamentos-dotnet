using Galo.Domain.Common.Base;
using Galo.Domain.Competicoes.Enums;

namespace Galo.Domain.Competicoes.Events
{
    public sealed record PartidaAgendadaEvent(Guid PartidaId, Guid MandanteId, Guid VisitanteId, DateTime DataHoraUtc) : DomainEvent;

    public sealed record PartidaIniciadaEvent(Guid PartidaId) : DomainEvent;

    public sealed record GolMarcadoEvent(
        Guid PartidaId, Guid ClubeId, Guid? JogadorId, int Minuto, TipoGol Tipo,
        int PlacarMandante, int PlacarVisitante) : DomainEvent;

    public sealed record PartidaEncerradaEvent(
        Guid PartidaId, Guid CampeonatoId, Guid MandanteId, Guid VisitanteId,
        int GolsMandante, int GolsVisitante) : DomainEvent;

    public sealed record PartidaCanceladaEvent(Guid PartidaId, string Motivo) : DomainEvent;
}
