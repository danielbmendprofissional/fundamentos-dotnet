using MediatR;
using Vendas.Application.Commom.Interfaces.Persistence;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Interfaces;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Application.Clientes.Commands.Criar
{
    public sealed class CriarCommandHandler : IRequestHandler<
        CriarCommand>
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCommandHandler(
            IClienteRepository clienteRepository,
            IUnitOfWork unitOfWork)
        {
            _clienteRepository = clienteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CriarCommand request, CancellationToken cancellationToken)
        {
            var endereco = DadosEndereco.Criar(
            request.EnderecoPrincipal.Cep,
            request.EnderecoPrincipal.Logradouro,
            request.EnderecoPrincipal.Numero,
            request.EnderecoPrincipal.Bairro,
            request.EnderecoPrincipal.Estado,
            request.EnderecoPrincipal.Cidade,
            request.EnderecoPrincipal.Pais,
            request.EnderecoPrincipal.Complemento
            );

            var cpf = Cpf.Create(request.Cpf);
            var email = Email.Create(request.Email);
            var nomeCompleto = NomeCompleto.Create(request.Nome);
            var telefone = Telefone.Create(request.Telefone);

            var cliente = Cliente.Criar(
                    nomeCompleto,
                    cpf,
                    email,
                    telefone,
                    endereco,
                    (Sexo)request.SexoId,
                    (EstadoCivil)request.EstadoCivilId
                );

            await _clienteRepository.AdicionarAsync(cliente, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
