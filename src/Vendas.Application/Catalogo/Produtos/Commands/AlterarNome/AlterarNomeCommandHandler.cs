using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Application.Catalogo.Produtos.Commands.AlterarNome
{
    public sealed class AlterarNomeCommandHandler : IRequestHandler<
        AlterarNomeCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarNomeCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarNomeCommand request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if(produto == null)
                throw new ArgumentNullException(nameof(produto));

            var nome = NomeProduto.Create(request.NovoNome);

            produto.AlterarNome(nome);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
