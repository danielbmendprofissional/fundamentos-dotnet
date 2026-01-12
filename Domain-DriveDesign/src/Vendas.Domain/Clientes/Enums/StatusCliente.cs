using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Domain.Clientes.Enums
{
    public enum StatusCliente
    {
        [Description("Ativo")]
        Ativo = 1,
        [Description("Bloqueado")]
        Bloqueado = 2
    }
}
