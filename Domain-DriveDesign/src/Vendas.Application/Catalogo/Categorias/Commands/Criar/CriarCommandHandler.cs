using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Produtos.Commands.AlterarDescricao;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Interfaces.Categorias;
using Vendas.Domain.Catalogo.Interfaces.Produtos;

namespace Vendas.Application.Catalogo.Categorias.Commands.Criar
{
    public sealed class CriarCommandHandler : IRequestHandler<
        CriarCommand>
    {
        private readonly ICategoriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCommandHandler(
            ICategoriaRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CriarCommand request, CancellationToken cancellationToken)
        {
            var categoria = Categoria.Criar(request.Nome, request.Descricao);
            await _repository.AdicionarAsync(categoria);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
