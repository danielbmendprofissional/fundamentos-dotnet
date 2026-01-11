using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Commands.Bloquear
{
    public sealed record BloquearCommand(Guid Id) : IRequest;
}
