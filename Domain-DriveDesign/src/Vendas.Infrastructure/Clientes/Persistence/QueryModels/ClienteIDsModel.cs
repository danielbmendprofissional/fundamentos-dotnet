using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Clientes.Persistence.QueryModels
{
    public sealed record ClienteIDsModel(Guid Id, Guid EnderecoPrincipalId);
}
