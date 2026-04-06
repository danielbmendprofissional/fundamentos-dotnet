
using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.ValueObjects;

namespace Onion.Domain.Accounts.Entities
{
    public sealed class Account : AggregateRoot
    {
        public Email Email { get; private set; } 
        public FullName FullName { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }


        public static Account Create(
            Guid id, 
            Email email, 
            FullName fullName, 
            string? photoUrl = null)
        => new(id, email, fullName, photoUrl);

        private Account(Guid id, Email email, FullName fullName, string? photoUrl = null) : base(id)
        {
            Guard.Against<DomainException>(id == Guid.Empty, "Invalid id.", nameof(id));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(fullName, nameof(fullName));

            Id = id;
            Email = email;
            FullName = fullName;
            PhotoUrl = photoUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(FullName fullName, string? photoUrl)
        {
            Guard.AgainstNull(fullName, nameof(fullName));

            FullName = fullName;
            PhotoUrl = photoUrl;
            SetUpdateDate();
        }

        public void UpdateEmailSnapshot(Email email)
        {
            Guard.AgainstNull(email, nameof(email));

            Email = email;
            SetUpdateDate();
        }

        public void Deactivate()
        {
            IsActive = false;
            SetUpdateDate();
        }

        public void Activate()
        {
            IsActive = true;
            SetUpdateDate();
        }
    }
}