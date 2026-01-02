using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Common.Enum
{
    public enum StatusPedido
    {
        Pendente = 1,
        PagamentoConfirmado = 2,
        EmSeparacao = 3,
        Enviado = 4,
        Entregue = 5,
        Cancelado = 6
    }
}
