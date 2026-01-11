using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Pedidos.Persistence.QueryModels
{
    /*Model de leitura não faz parte das regras de negócio, não pode ficar no domínio*/
    public sealed class PedidoResumoModel
    {
        public Guid Id { get; }
        public string NumeroPedido { get; } = string.Empty;
        public Guid ClienteId { get; }
        public decimal ValorTotal { get; }
        public string Status { get; } = string.Empty;
        public DateTime DataCriacao { get; }
    }
}
