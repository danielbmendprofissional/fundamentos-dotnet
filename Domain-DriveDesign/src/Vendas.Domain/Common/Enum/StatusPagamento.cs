using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Common.Enum
{
    public enum StatusPagamento
    {
        Pendente = 1,
        Aprovado = 2,
        Recusado = 3,
        Estornado = 4,
        Cancelado = 5
    }
}
