using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.Microsoft
{
    // OBRIGATORIEDADE: Representa o comando de Checkout.
    public record CheckoutCommand(string Produto, decimal Valor) : IRequest<bool>;
}
