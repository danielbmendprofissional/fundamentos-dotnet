using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.Commands.Ativar
{
    public sealed record AtivarCommand(
            Guid Id
        ) : IRequest;
}
