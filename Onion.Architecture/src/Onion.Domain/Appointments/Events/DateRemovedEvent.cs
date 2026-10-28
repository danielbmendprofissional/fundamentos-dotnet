using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Appointments.Events
{
    // notify users of the appointment about the date
    public sealed record DateRemovedEvent(
        Guid Id, 
        Guid DateId) : DomainEvent;
}