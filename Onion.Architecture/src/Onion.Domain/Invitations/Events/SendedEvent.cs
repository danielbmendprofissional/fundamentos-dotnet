// ...existing code...
using System;
using Onion.Domain.Common.Base;
using Onion.Domain.Invitations.Enums;

namespace Onion.Domain.Invitations.Events
{
    // notify the user that they have received an invitation, etc...

    public sealed record SendedEvent(
        Guid InvitationId, 
        InvitationType Type, 
        Guid? TargetId, 
        string RecipientEmail, 
        string? Reason = null, 
        Guid? ResponderPartnerId = null) : DomainEvent;
}