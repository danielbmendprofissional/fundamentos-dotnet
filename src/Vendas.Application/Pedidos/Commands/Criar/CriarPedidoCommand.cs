using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Pedidos.Commands.DTOs;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Pedidos.Commands.Criar
{
    public sealed record CriarPedidoCommand(
        Guid ClienteId,
        EnderecoDTO Endereco
    ) : IRequest;


}
