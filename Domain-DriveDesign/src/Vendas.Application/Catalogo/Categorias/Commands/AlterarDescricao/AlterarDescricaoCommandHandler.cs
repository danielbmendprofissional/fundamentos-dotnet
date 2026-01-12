using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Catalogo.Interfaces.Categorias;

namespace Vendas.Application.Catalogo.Categorias.Commands.AlterarDescricao
{
    internal class AlterarDescricaoCommandHandler : IRequestHandler<
        AlterarDescricaoCommand>
    {
        private readonly ICategoriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarDescricaoCommandHandler(
            ICategoriaRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarDescricaoCommand request, CancellationToken cancellationToken)
        {
            var categoria = await _repository.ObterPorIdAsync(request.Id, cancellationToken);
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            categoria.AlterarNome(request.NovaDescricao);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
