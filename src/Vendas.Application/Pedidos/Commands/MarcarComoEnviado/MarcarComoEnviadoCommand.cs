using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Pedidos.Commands.MarcarComoEnviado
{
    public sealed record MarcarComoEnviadoCommand(Guid PedidoId) : IRequest;
}
