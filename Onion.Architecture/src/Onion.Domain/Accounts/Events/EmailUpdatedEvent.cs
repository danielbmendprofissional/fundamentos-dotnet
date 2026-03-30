using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Accounts.Events
{
    // see something like change the snapshots from partners ->
    // change the snapshots from participants -> 
    // change the snapshots from accounts, etc...
    public sealed record EmailUpdatedEvent(
        Guid Id, 
        string NewEmail) : DomainEvent;
}