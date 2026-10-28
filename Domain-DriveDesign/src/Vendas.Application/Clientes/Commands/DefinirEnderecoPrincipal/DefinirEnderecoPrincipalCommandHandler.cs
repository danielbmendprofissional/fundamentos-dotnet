using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.RemoverEndereco;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Interfaces;

namespace Vendas.Application.Clientes.Commands.DefinirEnderecoPrincipal
{
    public class DefinirEnderecoPrincipalCommandHandler : IRequestHandler<
        DefinirEnderecoPrincipalCommand
        >
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DefinirEnderecoPrincipalCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DefinirEnderecoPrincipalCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            cliente.DefinirEnderecoPrincipal(request.EnderecoId);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
