using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Clientes.Queries.ObterEnderecos
{
    public sealed record ObterEnderecosQuery(Guid ClienteId)
        : IRequest<List<EnderecoItemReponseDTO>>;
}
