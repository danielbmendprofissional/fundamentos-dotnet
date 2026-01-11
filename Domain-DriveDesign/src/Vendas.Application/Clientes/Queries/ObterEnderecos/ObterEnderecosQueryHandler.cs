using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Interfaces;
using Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal;

namespace Vendas.Application.Clientes.Queries.ObterEnderecos
{
    public sealed class ObterEnderecosQueryHandler 
        : IRequestHandler<
            ObterEnderecosQuery, 
            List<EnderecoItemReponseDTO>>
    {
        private readonly IClienteQueryRepository _repository;

        public ObterEnderecosQueryHandler(
            IClienteQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EnderecoItemReponseDTO>> Handle(ObterEnderecosQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _repository.ObterListaEnderecos(
                request.ClienteId,
                cancellationToken);

            if (retorno == null)
                throw new ArgumentNullException("Endereço principal de cliente não encontrado.");

            return retorno.ToList();
        }
    }
}
