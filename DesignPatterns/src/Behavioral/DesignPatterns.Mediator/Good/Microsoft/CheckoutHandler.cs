using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Mediator.Good.Microsoft
{
    // OBRIGATORIEDADE: Manipulador específico. A "bomba de ifs" foi explodida em classes menores.
    public class CheckoutHandler : IRequestHandler<CheckoutCommand, bool>
    {
        private readonly IMediator _mediator;
        public CheckoutHandler(IMediator mediator) => _mediator = mediator;

        public async Task<bool> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[Microsoft] Processando {request.Produto} via MediatR...");

            // O Handler pode disparar outros eventos (Notifications) para Estoque e Financeiro.
            await _mediator.Publish(new PedidoPagoNotification(request.Produto));

            return true;
        }
    }
}
