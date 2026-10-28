using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Domain.Catalogo.Events.Produtos
{
    public sealed record class EstoqueAjustadoEvent(
                                    Guid ProdutoId, 
                                    int Quantidade, 
                                    string Motivo) : DomainEvent;
}
