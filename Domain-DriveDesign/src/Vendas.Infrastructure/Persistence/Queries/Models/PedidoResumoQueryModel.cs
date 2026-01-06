using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Infrastructure.Persistence.Query.Models
{
    /*Model de leitura não faz parte das regras de negócio, não pode ficar no domínio*/
    public sealed class PedidoResumoQueryModel
    {
        public Guid Id { get; set; }
        public string NumeroPedido { get; set; } = string.Empty;
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }
}
