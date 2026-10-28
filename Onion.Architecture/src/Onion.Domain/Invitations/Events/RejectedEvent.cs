// ...existing code...
using System;
using Onion.Domain.Common.Base;
using Onion.Domain.Invitations.Enums;

namespace Onion.Domain.Invitations.Events
{
    // notify the appointment owner that the invite has been rejected, etc...
    // change the participant status to rejected, etc...

    public sealed record RejectedEvent(
        Guid Id, 
        Guid AccountId, 
        InvitationType Type, 
        Guid? TargetId, 
        string RecipientEmail, 
        string? Reason = null, 
        Guid? ResponderPartnerId = null) : DomainEvent;
}