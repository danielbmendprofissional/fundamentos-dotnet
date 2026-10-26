using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Enums.Produtos;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;

namespace Vendas.Application.Catalogo.Produtos.Commands.Criar
{
    public sealed class CriarCommandHandler : IRequestHandler<
        CriarCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CriarCommand request, CancellationToken cancellationToken)
        {
            var nome = NomeProduto.Create(request.Nome);
            var codigo = CodigoProduto.Create(request.Codigo);
            var preco = PrecoProduto.Create(request.Preco);

            var produto = Produto.Criar(
                    nome,
                    codigo,
                    preco,
                    request.CategoriaId,
                    request.EstoqueInicial,
                    request.Descricao
                );

            await _repository.AdicionarAsync(produto, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
