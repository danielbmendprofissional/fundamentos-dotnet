using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commands.Pedidos.AtualizaEnderecoEntrega
{
    public sealed record AtualizarEnderecoEntregaCommand(
    Guid PedidoId,
    string Cep,
    string Logradouro,
    string Bairro,
    string Cidade,
    string Estado,
    string Pais,
    string? Complemento
) : IRequest;
}
