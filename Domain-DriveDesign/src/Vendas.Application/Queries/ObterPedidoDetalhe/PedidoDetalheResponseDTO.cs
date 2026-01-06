using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Queries.ObterPedidoDetalhe
{
    public sealed record PedidoDetalheResponseDTO(
        Guid Id,
        Guid ClienteId,
        string NumeroPedido,
        decimal ValorTotal,
        string Status,
        DateTime DataCriacao,
        string Cep,
        string Cidade,
        string Estado);
}
