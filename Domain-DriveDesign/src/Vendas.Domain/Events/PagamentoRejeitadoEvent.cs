using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Events
{
    //Record é usado porque eventos de domínio são fatos que JÁ OCORRERAM devem ser imutaveis.
    public record PagamentoRejeitadoEvent(Guid PagamentoId,
                                         Guid PedidoId,
                                         decimal Valor,
                                         DateTime DataPagamento,
                                         string? CodigoTransacao) : DomainEvent;
}
