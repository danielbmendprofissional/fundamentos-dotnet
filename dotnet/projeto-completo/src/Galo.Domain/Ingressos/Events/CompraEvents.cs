using Galo.Domain.Common.Base;
using Galo.Domain.Ingressos.ValueObjects;

namespace Galo.Domain.Ingressos.Events
{
    /// <summary>Foto do item no momento do evento (setor + quantidade), para não depender do agregado depois.</summary>
    public sealed record ItemCompraSnapshot(string Setor, int Quantidade);

    public sealed record PagamentoIniciadoEvent(
        Guid CompraId, Guid PagamentoId, Guid PartidaId, decimal Valor,
        IReadOnlyCollection<ItemCompraSnapshot> Itens) : DomainEvent;

    public sealed record PagamentoConfirmadoEvent(
        Guid CompraId, Guid PagamentoId, Guid TorcedorId, string CodigoTransacao, decimal Valor) : DomainEvent;

    public sealed record PagamentoRecusadoEvent(Guid CompraId, Guid PagamentoId, string CodigoTransacao) : DomainEvent;

    public sealed record IngressosEmitidosEvent(
        Guid CompraId, Guid TorcedorId, Guid PartidaId, IReadOnlyCollection<string> CodigosIngressos) : DomainEvent;

    public sealed record CompraCanceladaEvent(
        Guid CompraId, Guid TorcedorId, Guid PartidaId, MotivoCancelamento Motivo,
        bool ReservaConfirmada, IReadOnlyCollection<ItemCompraSnapshot> Itens) : DomainEvent;
}
