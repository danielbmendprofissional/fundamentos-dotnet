using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Interfaces.Categorias;

namespace Vendas.Application.Catalogo.Categorias.Commands.Inativar
{
    internal class InativarCommandHandler : IRequestHandler<
        InativarCommand>
    {
        private readonly ICategoriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InativarCommandHandler(
            ICategoriaRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(InativarCommand request, CancellationToken cancellationToken)
        {
            var categoria = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            categoria.Inativar();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
