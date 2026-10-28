using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Queries.ObterEnderecoPrincipal
{
    public sealed record ObterEnderecoPrincipalQuery(Guid ClienteId) 
        : IRequest<EnderecoPrincipalReponseDTO>;
}
