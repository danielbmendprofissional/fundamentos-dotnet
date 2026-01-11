using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Interfaces;

namespace Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal
{
    public sealed class ObterEnderecoPrincipalQueryHandler
        :IRequestHandler<
            ObterEnderecoPrincipalQuery,
            EnderecoPrincipalReponseDTO>
    {
        private readonly IClienteQueryRepository _repository;

        public ObterEnderecoPrincipalQueryHandler(
            IClienteQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<EnderecoPrincipalReponseDTO> Handle(ObterEnderecoPrincipalQuery request, CancellationToken cancellationToken)
        {
            var retorno = await _repository.ObterEnderecoPrincipal(
                request.ClienteId, 
                cancellationToken);

            if (retorno == null)
                throw new ArgumentNullException("Endereço principal de cliente não encontrado.");

            return retorno;
        }
    }
}
