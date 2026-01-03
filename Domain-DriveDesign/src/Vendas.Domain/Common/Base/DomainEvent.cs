using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vendas.Domain.Interfaces.Events;

namespace Vendas.Domain.Common.Base
{
    public abstract record class DomainEvent : IDomainEvent
    {
        public DateTime DateOccurred { get; } = DateTime.UtcNow;
    }
}
