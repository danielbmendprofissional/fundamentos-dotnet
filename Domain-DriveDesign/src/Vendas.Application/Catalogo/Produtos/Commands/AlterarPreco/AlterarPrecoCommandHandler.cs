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

namespace Vendas.Application.Catalogo.Produtos.Commands.AlterarPreco
{
    public sealed class AlterarPrecoCommandHandler : IRequestHandler<
        AlterarPrecoCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarPrecoCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarPrecoCommand request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            var nome = PrecoProduto.Create(request.NovoPreco);

            produto.AlterarPreco(nome);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
