using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Accounts.Events
{
    // see something like change the snapshots from partners ->
    // change the snapshots from participants -> 
    // change the snapshots from accounts, etc...
    public sealed record ProfileUpdatedEvent(
        Guid Id, 
        string? NewDisplayName = null, 
        string? NewPhotoUrl = null) : DomainEvent;
}