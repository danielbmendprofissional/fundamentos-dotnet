using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Appointments.Events
{
    // notify the user that they have been removed from the appointment, etc...

    public sealed record ParticipantRemovedEvent(
        Guid AppointmentId, 
        Guid ParticipantId, 
        string ParticipantEmail) : DomainEvent;
}