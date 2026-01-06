using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.Pedidos.CriarPedido
{
    public sealed record CriarPedidoCommand(
        Guid ClienteId,
        string Cep,
        string Logradouro,
        string Bairro,
        string Cidade,
        string Estado,
        string Pais,
        string? Complemento
    ) : IRequest;


}
