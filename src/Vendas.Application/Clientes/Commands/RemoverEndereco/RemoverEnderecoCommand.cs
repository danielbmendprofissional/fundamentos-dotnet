using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Commands.RemoverEndereco
{
    public sealed record RemoverEnderecoCommand(
        Guid ClienteId,
        Guid EnderecoId
    ) : IRequest;
}
