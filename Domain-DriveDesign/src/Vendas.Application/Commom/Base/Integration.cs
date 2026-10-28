using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Application.Commom.Interfaces.IntegrationEvents;

namespace Vendas.Application.Commom.Base
{
    // Vive na camada de Application (pois é um contrato de saída)
    public abstract record class IntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime DateOccurred { get; init; } = DateTime.UtcNow;
    }
}
