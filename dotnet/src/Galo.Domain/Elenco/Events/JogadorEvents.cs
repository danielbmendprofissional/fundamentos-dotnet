using Galo.Domain.Common.Base;

namespace Galo.Domain.Elenco.Events
{
    public sealed record JogadorCadastradoEvent(Guid JogadorId, string Nome) : DomainEvent;

    public sealed record ValorMercadoAtualizadoEvent(Guid JogadorId, decimal ValorAnterior, decimal ValorNovo) : DomainEvent;

    public sealed record ContratoRegistradoEvent(Guid JogadorId, Guid ContratoId, DateOnly Inicio, DateOnly Fim) : DomainEvent;

    public sealed record JogadorLesionadoEvent(Guid JogadorId, string Descricao) : DomainEvent;

    public sealed record JogadorRecuperadoEvent(Guid JogadorId) : DomainEvent;
}
