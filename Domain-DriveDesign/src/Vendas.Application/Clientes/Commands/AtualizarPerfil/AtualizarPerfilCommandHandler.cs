using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Commands.Criar;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Application.Clientes.Commands.AtualizarPerfil
{
    public sealed class AtualizarPerfilCommandHandler : IRequestHandler<
        AtualizarPerfilCommand>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AtualizarPerfilCommandHandler(
            IClienteRepository clienteRepository,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(AtualizarPerfilCommand request, CancellationToken cancellationToken)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);
            if(cliente == null)
                throw new InvalidOperationException("Cliente não encontrado.");

            var email = Email.Create(request.Email);
            var nomeCompleto = NomeCompleto.Create(request.Nome);
            var telefone = Telefone.Create(request.Telefone);

            cliente.AtualizarPerfil(
                nomeCompleto,
                email,
                telefone,
                (Sexo)request.SexoId,
                (EstadoCivil)request.EstadoCivilId
                );

            _clienteRepository.Atualizar(cliente);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
