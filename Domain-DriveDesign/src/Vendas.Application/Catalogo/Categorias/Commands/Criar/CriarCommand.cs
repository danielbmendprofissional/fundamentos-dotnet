using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Catalogo.Categorias.Commands.Criar
{
    public sealed record CriarCommand(
            string Nome,
            string Descricao
        ) : IRequest;
}
