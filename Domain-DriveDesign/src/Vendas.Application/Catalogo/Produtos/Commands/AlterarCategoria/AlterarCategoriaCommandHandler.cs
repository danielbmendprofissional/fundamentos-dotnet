using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Produtos.Commands.AlterarNome;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Application.Catalogo.Produtos.Commands.AlterarCategoria
{
    public sealed class AlterarCategoriaCommandHandler : IRequestHandler<
        AlterarCategoriaCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarCategoriaCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarCategoriaCommand request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            produto.AlterarCategoria(request.CategoriaId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
