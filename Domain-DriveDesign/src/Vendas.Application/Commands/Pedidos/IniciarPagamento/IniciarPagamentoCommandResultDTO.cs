using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.IniciarPagamento
{
    public sealed record IniciarPagamentoCommandResultDTO(Guid NovoPagamentoId);
}
