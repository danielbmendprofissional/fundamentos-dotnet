using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.Microsoft.Events
{
    // OBRIGATORIEDADE: INotification permite que múltiplos componentes reajam sem se conhecerem.
    public record PedidoPagoEvent(string Produto) : INotification;
}
