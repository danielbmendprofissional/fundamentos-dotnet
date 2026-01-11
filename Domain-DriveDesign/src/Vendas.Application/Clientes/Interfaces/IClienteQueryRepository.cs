using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal;
using Vendas.Application.Clientes.Queries.ObterEnderecos;

namespace Vendas.Application.Clientes.Interfaces
{
    public interface IClienteQueryRepository
    {
        Task<EnderecoPrincipalReponseDTO?> ObterEnderecoPrincipal(
            Guid clinteId,
            CancellationToken cancellationToken);

        Task<IEnumerable<EnderecoItemReponseDTO>> ObterListaEnderecos(
            Guid clienteId,
            CancellationToken cancellationToken);
    }
}
