using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Appointments.Events
{
    // see something like change the snapshots from appointments ->
    public sealed record UpdatedEvent(
        Guid Id) : DomainEvent;
}