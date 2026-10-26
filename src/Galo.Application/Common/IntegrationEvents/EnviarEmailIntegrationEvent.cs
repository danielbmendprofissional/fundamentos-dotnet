using Galo.Application.Common.Base;

namespace Galo.Application.Common.IntegrationEvents
{
    public sealed record EnviarEmailIntegrationEvent(string Destinatario, string Assunto, string CorpoHtml) : IntegrationEvent;
}
