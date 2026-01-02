using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Common.Enum
{
    public enum MetodoPagamento
    {
        CartaoCredito = 1,
        CartaoDebito = 2,
        Pix = 3,
        Boleto = 4,
        TransferenciaBancaria = 5
    }
}
