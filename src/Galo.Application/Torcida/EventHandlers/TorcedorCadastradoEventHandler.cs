using Galo.Application.Common.Interfaces.IntegrationEvents;
using Galo.Application.Common.IntegrationEvents;
using Galo.Domain.Torcida.Events;
using MediatR;

namespace Galo.Application.Torcida.EventHandlers
{
    /// <summary>
    /// Evento de DOMÍNIO (TorcedorCadastrado) -> evento de INTEGRAÇÃO (EnviarEmail) no RabbitMQ.
    /// O envio de e-mail é um efeito colateral lento e falível: sai do fluxo principal via mensageria.
    /// </summary>
    public sealed class TorcedorCadastradoEventHandler : INotificationHandler<TorcedorCadastradoEvent>
    {
        private readonly IIntegrationEventPublisher _publisher;
        public TorcedorCadastradoEventHandler(IIntegrationEventPublisher publisher) => _publisher = publisher;

        public Task Handle(TorcedorCadastradoEvent notification, CancellationToken cancellationToken)
            => _publisher.PublishAsync(new EnviarEmailIntegrationEvent(
                notification.Email,
                "Bem-vindo à Massa Atleticana!",
                $"<h1>Olá, {notification.Nome}!</h1><p>Seu cadastro de torcedor foi realizado. Galo!</p>"), cancellationToken);
    }
}
