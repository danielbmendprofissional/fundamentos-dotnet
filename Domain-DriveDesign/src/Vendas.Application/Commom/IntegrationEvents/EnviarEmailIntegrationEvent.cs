using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Base;

namespace Vendas.Application.Commom.IntegrationEvents
{
    public record EnviarEmailIntegrationEvent(
        string Destinatario,
        string Assunto,
        string CorpoHtml) : IntegrationEvent;
}
