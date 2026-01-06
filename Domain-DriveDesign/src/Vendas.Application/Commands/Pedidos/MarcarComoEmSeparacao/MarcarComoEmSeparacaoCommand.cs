using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.MarcarComoEmSeparacao
{
    public sealed record MarcarComoEmSeparacaoCommand(Guid PedidoId) : IRequest;
}
