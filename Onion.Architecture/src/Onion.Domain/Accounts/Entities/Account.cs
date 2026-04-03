
using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Common.Exceptions;

namespace Onion.Domain.Accounts.Entities
{
    public sealed class Account : AggregateRoot
    {
        public string Email { get; private set; } 
        public string Name { get; private set; }
        public string? PhotoUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }


        public static Account Create(
            Guid id, 
            string email, 
            string name, 
            string? photoUrl = null)
        => new(id, email, name, photoUrl);

        private Account(Guid id, string email, string name, string? photoUrl = null) : base(id)
        {
            Guard.Against<DomainException>(id == Guid.Empty, "Invalid id.", nameof(id));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));

            Id = id;
            Email = email;
            Name = name;
            PhotoUrl = photoUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string name, string? photoUrl)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));

            Name = name;
            PhotoUrl = photoUrl;
            SetUpdateDate();
        }

        public void UpdateEmailSnapshot(string email)
        {
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            
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