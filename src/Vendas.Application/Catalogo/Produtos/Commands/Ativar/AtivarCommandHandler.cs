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

namespace Vendas.Application.Catalogo.Produtos.Commands.Ativar
{
    public sealed class AtivarCommandHandler : IRequestHandler<
        AtivarCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AtivarCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AtivarCommand request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));
            produto.Ativar();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}