using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Catalogo.Produtos.Commands.AlterarDescricao;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Interfaces.Categorias;
using Vendas.Domain.Catalogo.Interfaces.Produtos;

namespace Vendas.Application.Catalogo.Categorias.Commands.AlterarNome
{
    internal class AlterarNomeCommandHandler : IRequestHandler<
        AlterarNomeCommand>
    {
        private readonly ICategoriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarNomeCommandHandler(
            ICategoriaRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarNomeCommand request, CancellationToken cancellationToken)
        {
            var categoria = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            categoria.AlterarNome(request.NovoNome);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
