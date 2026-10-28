using Galo.Application.Common.Interfaces.IntegrationEvents;
using Galo.Application.Common.IntegrationEvents;
using Galo.Application.Ingressos.IntegrationEvents;
using Galo.Domain.Ingressos.Events;
using Galo.Domain.Torcida.Interfaces;
using MediatR;

namespace Galo.Application.Ingressos.EventHandlers
{
    // Evento de DOMÍNIO (síncrono, in-process, após o SaveChanges) -> evento de INTEGRAÇÃO (assíncrono, RabbitMQ).

    /// <summary>Pagamento iniciado => pede à Partida (via fila) para reservar os ingressos.</summary>
    public sealed class PagamentoIniciadoEventHandler : INotificationHandler<PagamentoIniciadoEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;
        public PagamentoIniciadoEventHandler(IIntegrationEventPublisher publisher) => _publisher = publisher;

        public Task Handle(PagamentoIniciadoEvent notification, CancellationToken cancellationToken)
            => _publisher.PublishAsync(new ReservarIngressosIntegrationEvent(
                notification.CompraId,
                notification.PartidaId,
                notification.Itens.Select(i => new ItemReservaDto(i.Setor, i.Quantidade)).ToList()), cancellationToken);
    }

    /// <summary>Compra cancelada => devolve ao estoque, mas SÓ se a reserva chegou a ser feita.</summary>
    public sealed class CompraCanceladaEventHandler : INotificationHandler<CompraCanceladaEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;
        public CompraCanceladaEventHandler(IIntegrationEventPublisher publisher) => _publisher = publisher;

        public Task Handle(CompraCanceladaEvent notification, CancellationToken cancellationToken)
        {
            if (!notification.ReservaConfirmada) return Task.CompletedTask;

            return _publisher.PublishAsync(new LiberarIngressosIntegrationEvent(
                notification.CompraId,
                notification.PartidaId,
                notification.Itens.Select(i => new ItemReservaDto(i.Setor, i.Quantidade)).ToList()), cancellationToken);
        }
    }

    public sealed class IngressosEmitidosEventHandler : INotificationHandler<IngressosEmitidosEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;
        private readonly ITorcedorRepository _torcedores;

        public IngressosEmitidosEventHandler(IIntegrationEventPublisher publisher, ITorcedorRepository torcedores)
        {
            _publisher = publisher;
            _torcedores = torcedores;
        }

        public async Task Handle(IngressosEmitidosEvent notification, CancellationToken cancellationToken)
        {
            var torcedor = await _torcedores.ObterPorIdAsync(notification.TorcedorId, cancellationToken);
            if (torcedor is null) return;

            var lista = string.Join("", notification.CodigosIngressos.Select(c => $"<li>{c}</li>"));
            await _publisher.PublishAsync(new EnviarEmailIntegrationEvent(
                torcedor.Email.Endereco,
                "Seus ingressos para o Galo",
                $"<p>Seus ingressos foram emitidos:</p><ul>{lista}</ul>"), cancellationToken);
        }
    }
}
