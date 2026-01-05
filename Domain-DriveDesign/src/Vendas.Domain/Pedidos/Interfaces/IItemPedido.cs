using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Pedidos.Interfaces
{
    //Interface criada pois o ROOT precisa da lista public, porém a entidade é internal,
    //sendo assim ele não consegue criar algo com a visibilidade maior que a raiz, então
    //como é uma Read list cria-se a interface para que seja feito o CAST e possa sex exposto
    //fora.
    public interface IItemPedido
    {
        Guid Id { get; }
        Guid ProdutoId { get; }
        string NomeProduto { get; }
        decimal PrecoUnitario { get; }
        int Quantidade { get; }
        decimal DescontoAplicado { get; }
        decimal ValorTotal { get; }
    }

}
