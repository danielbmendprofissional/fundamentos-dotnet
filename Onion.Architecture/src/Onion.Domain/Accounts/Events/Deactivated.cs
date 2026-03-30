using System;
using Onion.Domain.Common.Base;

namespace Onion.Domain.Accounts.Events
{
    // see something like change the partner status to deactivated,
    // change the participant status to deactivated, etc...
    public sealed record DeactivatedEvent(Guid Id) : DomainEvent;
}