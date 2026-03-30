using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Appointments.Events
{
    // notify users of the appointment about the date
    public sealed record DateAddedEvent(
        Guid Id, 
        Guid DateId,
        DateTime StartAt, 
        DateTime EndAt, 
        string? Name = null) : DomainEvent;
}