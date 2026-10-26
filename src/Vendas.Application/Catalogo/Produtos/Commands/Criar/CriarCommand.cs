using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Application.Catalogo.Produtos.Commands.Criar
{
    public sealed record CriarCommand(
            string Nome, 
            string Codigo, 
            decimal Preco,
            Guid CategoriaId, 
            int EstoqueInicial,
            string? Descricao
        ) : IRequest;
}
