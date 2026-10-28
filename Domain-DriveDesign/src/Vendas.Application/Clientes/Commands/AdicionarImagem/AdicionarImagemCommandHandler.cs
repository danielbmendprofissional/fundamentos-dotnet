using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Produtos.Commands.AlterarDescricao;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Interfaces.Produtos;
using Vendas.Domain.Catalogo.ValueObjects.Produtos;

namespace Vendas.Application.Clientes.Commands.AdicionarImagem
{
    public sealed class AdicionarImagemCommandHandler : IRequestHandler<
        AdicionarImagemCommand>
    {
        private readonly IProdutoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarImagemCommandHandler(
            IProdutoRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AdicionarImagemCommand request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            var imagem = ImagemProduto.Create(request.Url, request.Ordem);

            produto.AdicionarImagem(imagem);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
