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

namespace Vendas.Application.Clientes.Commands.AlterarEndereco
{
    public class AlterarEnderecoCommandHandler : IRequestHandler<
        AlterarEnderecoCommand
        >
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AlterarEnderecoCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AlterarEnderecoCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            cliente.AlterarEndereco(
                request.EndereocId,
                request.Endereco.Cep,
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Estado,
                request.Endereco.Cidade,
                request.Endereco.Pais,
                request.Endereco.Complemento);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
