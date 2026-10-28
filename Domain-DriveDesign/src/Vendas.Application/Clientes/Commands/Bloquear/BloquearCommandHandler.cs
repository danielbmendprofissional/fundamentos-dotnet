using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.AtualizarPerfil;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;

namespace Vendas.Application.Clientes.Commands.Bloquear
{
    public sealed class BloquearCommandHandler : IRequestHandler<
        BloquearCommand>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BloquearCommandHandler(
            IClienteRepository clienteRepository,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(BloquearCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);
            if (cliente == null)
                throw new InvalidOperationException("Cliente não encontrado.");

            cliente.Bloquear();

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
