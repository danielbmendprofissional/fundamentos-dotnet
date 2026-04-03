// ...existing code...
using System;
using Onion.Domain.Common.Base;
using Onion.Domain.Invitations.Enums;

namespace Onion.Domain.Invitations.Events
{
    // after acept the invite this event will be raised,
    // notify the user that they have been added to the appointment, etc...
    // see something like add participant to the appointment, etc...

    public sealed record AcceptedEvent(
        Guid Id, 
        Guid AccountId, 
        InvitationType Type, 
        Guid? TargetId, 
        string RecipientEmail, 
        Guid? ResponderPartnerId = null) : DomainEvent;
}