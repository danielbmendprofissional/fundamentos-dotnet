using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Application.Commands.Pedidos.IniciarPagamento
{
    public sealed record IniciarPagamentoCommand(
        Guid PedidoId,
        long MetodoPagamento
        ) : IRequest<IniciarPagamentoCommandResultDTO>;
}
