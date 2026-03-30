using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Accounts.Events
{
    // see something like change the partner status to active,
    // change the participant status to active, etc...
    public sealed record ActivatedEvent(Guid Id) : DomainEvent;
}