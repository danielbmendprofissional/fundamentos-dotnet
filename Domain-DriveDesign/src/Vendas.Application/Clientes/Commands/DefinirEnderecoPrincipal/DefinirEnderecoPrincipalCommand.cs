using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Commands.DefinirEnderecoPrincipal
{
    public sealed record DefinirEnderecoPrincipalCommand(
          Guid ClienteId,
          Guid EnderecoId
        ): IRequest;
}
