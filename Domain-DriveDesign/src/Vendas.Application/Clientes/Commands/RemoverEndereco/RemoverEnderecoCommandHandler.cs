using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.AdicionarEndereco;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;

namespace Vendas.Application.Clientes.Commands.RemoverEndereco
{
    public class RemoverEnderecoCommandHandler : IRequestHandler<
        RemoverEnderecoCommand
        >
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoverEnderecoCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(RemoverEnderecoCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            cliente.RemoverEndereco(request.EnderecoId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
