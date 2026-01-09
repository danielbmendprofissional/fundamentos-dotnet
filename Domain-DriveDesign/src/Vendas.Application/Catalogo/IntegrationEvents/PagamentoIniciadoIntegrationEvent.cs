using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Common.Base;

namespace Vendas.Application.Catalogo.IntegrationEvents
{
    public sealed record PagamentoIniciadoIntegrationEvent(
                                            Guid PagamentoId,
                                            decimal Valor,
                                            string? CodigoTransacao);
}
