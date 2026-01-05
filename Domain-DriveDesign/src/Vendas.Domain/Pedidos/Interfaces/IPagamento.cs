using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Domain.Pedidos.Interfaces
{
    //Interface criada pois o ROOT precisa da lista public, porém a entidade é internal,
    //sendo assim ele não consegue criar algo com a visibilidade maior que a raiz, então
    //como é uma Read list cria-se a interface para que seja feito o CAST e possa sex exposto
    //fora.
    public interface IPagamento
    {
        Guid Id { get; }
        public MetodoPagamento MetodoPagamento { get; }
        StatusPagamento StatusPagamento { get; }
        decimal Valor { get; }
        public DateTime? DataPagamento { get; }
        public string? CodigoTransacao { get; }
    }

}
