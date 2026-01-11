using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;

namespace Vendas.Application.Clientes.Commands.AdicionarEndereco
{
    public sealed class AdicionarEnderecoCommandHandler: IRequestHandler<
        AdicionarEnderecoCommand
        >
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarEnderecoCommandHandler(IClienteRepository clienteRepository, IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AdicionarEnderecoCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var endereco = DadosEndereco.Criar(
                request.Endereco.Cep,
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Estado,
                request.Endereco.Cidade,
                request.Endereco.Pais,
                request.Endereco.Complemento
                );

            cliente.AdicionarEndereco(endereco);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
