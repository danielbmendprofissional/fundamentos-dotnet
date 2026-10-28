using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Produtos.Commands.AlterarCategoria
{
    public sealed record AlterarCategoriaCommand(
            Guid Id,
            Guid CategoriaId
        ) : IRequest;
}
