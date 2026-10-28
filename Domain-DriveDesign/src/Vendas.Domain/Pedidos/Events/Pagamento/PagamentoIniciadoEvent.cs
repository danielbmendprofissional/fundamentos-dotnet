using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Pedidos.Events.Pagamento
{
    public sealed record PagamentoIniciadoEvent(Guid PagamentoId,
                                         decimal Valor,
                                         string? CodigoTransacao) : DomainEvent;
}
