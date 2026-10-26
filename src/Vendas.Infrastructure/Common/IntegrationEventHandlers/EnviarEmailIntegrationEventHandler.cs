using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.IntegrationEvents;

namespace Vendas.Infrastructure.Common.IntegrationEventHandlers
{
    public sealed class EnviarEmailIntegrationEventHandler
        : INotificationHandler<EnviarEmailIntegrationEvent>
    {
        //private readonly IEmailService _emailService; -- Sua ACL de e-mail
        private readonly ILogger<EnviarEmailIntegrationEventHandler> _logger;

        public EnviarEmailIntegrationEventHandler(
            //IEmailService emailService,
            ILogger<EnviarEmailIntegrationEventHandler> logger)
        {
            //_emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(
            EnviarEmailIntegrationEvent notification,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Enviando e-mail para {Destinatario} com ID de evento {EventId}",
                notification.Destinatario, notification.Id);

            try
            {
                // A chamada real para o servidor de e-mail (SMTP, SendGrid, etc)
            }
            catch (Exception ex)
            {
                // Importante: Como e-mail é infraestrutura, ele pode falhar.
                // Aqui você loga o erro, mas não deixa travar o fluxo principal do sistema.
                _logger.LogError(ex, "Falha ao enviar e-mail para {Destinatario}", notification.Destinatario);
            }
        }
    }
}
