using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vendas.Application.Commom.Interfaces.IntegrationEvents
{
    // Vive na camada de Application (pois é um contrato de saída)
    public interface IIntegrationEvent : INotification
    {
        public Guid Id { get; }
        public DateTime DateOccurred { get; }
    }
}
