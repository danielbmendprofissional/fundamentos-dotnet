using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Commands.DefinirCodigoTransacao
{
    public sealed record DefinirCodigoTransacaoCommand(
        Guid PedidoId,
        Guid PagamentoId,
        string CodigoTransacao
    ) : IRequest;

}
