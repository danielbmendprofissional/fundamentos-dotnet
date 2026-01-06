using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Vendas.Domain.Pedidos.Events.Pagamento;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vendas.Application.EventHandlers.Pedidos
{
    public sealed class PagamentoConfirmadoEventHandler
        : INotificationHandler<PagamentoConfirmadoEvent>
    {
        private readonly IMediator _mediator;

        public PagamentoConfirmadoEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task Handle(
            PagamentoConfirmadoEvent notification,
            CancellationToken cancellationToken)
        {
            // EXEMPLOS DO QUE FAZER AQUI:

            // 1. Enviar email
            // await _emailService.EnviarPagamentoConfirmado(...);

            // 2. Chamar outro Aggregado de outro bounded context
            // lançando outro event (Integration Event)

            // 3. Chamar Outro Aggregado no mesmo contexto (SEM ALTERAR O AGGREGATE ATUAL)
            // chamando a factory direto e realizando o negócio.

            // 4. Serviços de log

            // 5. Api externa passando pela ACL

            /*
            Domain Event Handler pode ficar na Application quando:

                ✔ Reage a um Domain Event
                ✔ Atua sobre OUTRO aggregate
                ✔ Coordena persistência
                ✔ Dispara Integration Events
                ✔ NÃO continua o fluxo do aggregate que gerou o evento

             Domain Event Handler NÃO deve:

                ❌ Chamar outro command do mesmo contexto
                ❌ Continuar o mesmo caso de uso
                ❌ Alterar o aggregate que disparou o evento
                ❌ Virar “workflow engine”
            */
        }
    }
}
