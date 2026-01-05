using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Clientes.Events
{
    public sealed record EnderecoPrincipalAlteradoEvent(
        Guid ClienteId,
        Guid NovoEnderecoId) : DomainEvent;
}
