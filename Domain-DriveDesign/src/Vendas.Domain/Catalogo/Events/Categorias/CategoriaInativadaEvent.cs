using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalogo.Events.Categorias
{
    public sealed record CategoriaInativadaEvent(Guid CategoriaId) : DomainEvent;
}
