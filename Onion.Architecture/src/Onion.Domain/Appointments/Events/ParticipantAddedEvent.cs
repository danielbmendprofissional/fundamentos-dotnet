using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Appointments.Events
{
    // after acept the invite this event will be raised,
    // notify the user that they have been added to the appointment, etc...
    public sealed record ParticipantAddedEvent(
        Guid Id, 
        Guid ParticipantId, 
        string ParticipantEmail, 
        Guid? PartnerId = null) : DomainEvent;
}