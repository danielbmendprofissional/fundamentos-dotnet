using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Events.Pagamento;

namespace Vendas.Application.EventHandlers.Pedidos
{
    public sealed class PagamentoConfirmadoEventHandler
        : INotificationHandler<PagamentoConfirmadoEvent>
    {
        public async Task Handle(
            PagamentoConfirmadoEvent notification,
            CancellationToken cancellationToken)
        {
            // EXEMPLOS DO QUE FAZER AQUI:

            // 1. Enviar email
            // await _emailService.EnviarPagamentoConfirmado(...);

            // 2. Chamar outro Aggregado de outro bounded context
            // lançando outro event

            // 3. Chamar Outro Aggregado no mesmo contexto (SEM ALTERAR O AGGREGATE ATUAL)
            // chamando a factory direto e realizando o negócio.

            // 4. Serviços de log

            // 5. Api externa passando pela ACL
        }
    }
}
