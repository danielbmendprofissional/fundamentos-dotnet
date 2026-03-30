using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Partners.Enums;
using Onion.Domain.Partners.ValueObjects;
using Onion.Domain.Common.Exceptions;

namespace Onion.Domain.Partners.Entities
{
    public sealed class Partner : AggregateRoot
    {
        public FullName FullName { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; } 
        public PartnerType PartnerType { get; private set; }
        public string PartnerId { get; private set; }
        public string? Image { get; private set; }

        private Partner(
            FullName fullName,
            string name,
            Email email,
            PartnerType partnerType,
            string partnerId,
            string? image)
        {
            Guard.AgainstInvalidEnum<PartnerType>(partnerType, nameof(partnerType));
            Guard.AgainstNullOrWhiteSpace(partnerId, nameof(partnerId));
            Guard.AgainstNull(email, nameof(email));
            
            Validate(fullName, name);
            ValidatePartnerId(partnerType, partnerId, email);

            FullName = fullName;
            Name = name;
            Email = email;
            PartnerType = partnerType;
            PartnerId = partnerId;
            Image = image;
        }

        public static Partner Create(
            FullName fullName,
            string name,
            Email email,
            PartnerType partnerType,
            string partnerId,
            string? image)
        => new (fullName, name, email, partnerType, partnerId, image);

        public void Update(
            FullName fullName,
            string name,
            string? image)
        {
            Validate(fullName, name);

            FullName = fullName;
            Name = name;
            Image = image;

            SetDataAtualizacao();
        }

        private void Validate(
            FullName fullName,
            string name)
        {
            Guard.AgainstNull(fullName, nameof(fullName));
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        }

        private void ValidatePartnerId(PartnerType partnerType, string partnerId, Email email)
        {
            Guard.AgainstNullOrWhiteSpace(partnerId, nameof(partnerId));

            switch (partnerType)
            {
                case PartnerType.Calendar:
                    if (!Guid.TryParse(partnerId, out _))
                        throw new DomainException("PartnerId for Calendar must be a valid GUID (reference to a user).");
                    break;

                case PartnerType.Google:
                case PartnerType.Microsoft:
                    if (email == null)
                        throw new DomainException("Email is required for Google/Microsoft partners.");
                    if (!string.Equals(partnerId, email.Address, StringComparison.OrdinalIgnoreCase))
                        throw new DomainException("PartnerId must match the email for Google/Microsoft partners.");
                    break;

                default:
                    break;
            }
        }
    }
}