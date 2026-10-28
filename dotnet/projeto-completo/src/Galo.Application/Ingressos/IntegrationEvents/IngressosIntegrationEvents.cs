using Galo.Application.Common.Base;

namespace Galo.Application.Ingressos.IntegrationEvents
{
    public sealed record ItemReservaDto(string Setor, int Quantidade);

    /// <summary>
    /// Pedido (assíncrono) para a Partida baixar o estoque dos setores. Vai para o RabbitMQ e VOLTA para a
    /// própria API pelo consumidor. Em um sistema maior, quem consome seria outro serviço (estoque/ticketing).
    /// </summary>
    public sealed record ReservarIngressosIntegrationEvent(
        Guid CompraId, Guid PartidaId, IReadOnlyList<ItemReservaDto> Itens) : IntegrationEvent;

    /// <summary>Compensação: devolve ao estoque os ingressos de uma compra cancelada.</summary>
    public sealed record LiberarIngressosIntegrationEvent(
        Guid CompraId, Guid PartidaId, IReadOnlyList<ItemReservaDto> Itens) : IntegrationEvent;
}
