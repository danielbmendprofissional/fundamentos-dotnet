// ...existing code...
using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Partners.Events
{
    // change all the snapshots related to the partner, etc...

    public sealed record UpdatedEvent(
        Guid PartnerId, 
        Guid AccountId, 
        string? NewName = null, 
        string? NewEmail = null) : DomainEvent;
}