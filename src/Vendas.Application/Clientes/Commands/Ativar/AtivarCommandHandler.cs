using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.Ativar;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Interfaces;

namespace Vendas.Application.Clientes.Commands.Ativar
{
    public sealed class AtivarCommandHandler : IRequestHandler<
        AtivarCommand>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AtivarCommandHandler(
            IClienteRepository clienteRepository,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AtivarCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);
            if (cliente == null)
                throw new InvalidOperationException("Cliente não encontrado.");

            cliente.Ativar();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
