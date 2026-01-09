using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Pedidos.Commands.DTOs;

namespace Vendas.Application.Pedidos.Commands.AtualizaEnderecoEntrega
{
    public sealed record AtualizarEnderecoEntregaCommand(
    Guid PedidoId,
    EnderecoDTO Endereco
    ) : IRequest;
}
