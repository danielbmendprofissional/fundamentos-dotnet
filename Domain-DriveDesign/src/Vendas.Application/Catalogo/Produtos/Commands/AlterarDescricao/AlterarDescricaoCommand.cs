using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.Commands.AlterarDescricao
{
    public sealed record AlterarDescricaoCommand(
            Guid Id,
            string NovaDescricao
        ) : IRequest;
}
