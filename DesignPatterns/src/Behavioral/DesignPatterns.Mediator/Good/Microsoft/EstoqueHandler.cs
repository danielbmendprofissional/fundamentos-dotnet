using DesignPatterns.Mediator.Good.Microsoft.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.Microsoft
{
    public class EstoqueHandler : INotificationHandler<PedidoPagoEvent>
    {
        public Task Handle(PedidoPagoEvent notification, CancellationToken ct)
        {
            Console.WriteLine($"[Estoque] Baixando {notification.Produto} do inventário.");
            return Task.CompletedTask;
        }
    }
}
