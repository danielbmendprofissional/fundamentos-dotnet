using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Partners.Enums;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.ValueObjects;

namespace Onion.Domain.Partners.Entities
{
    public sealed class Partner : AggregateRoot
    {
        public FullName FullName { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; } 
        public PartnerType PartnerType { get; private set; }
        public string TypeId { get; private set; }
        public string? Image { get; private set; }

        private Partner(
            FullName fullName,
            string name,
            Email email,
            PartnerType partnerType,
            string typeId,
            string? image)
        {
            Guard.AgainstInvalidEnum<PartnerType>(partnerType, nameof(partnerType));
            Guard.AgainstNullOrWhiteSpace(typeId, nameof(typeId));
            Guard.AgainstNull(email, nameof(email));
            
            Validate(fullName, name);
            ValidateTypeId(partnerType, typeId, email);

            FullName = fullName;
            Name = name;
            Email = email;
            PartnerType = partnerType;
            TypeId = typeId;
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

            SetUpdateDate();
        }

        private void Validate(
            FullName fullName,
            string name)
        {
            Guard.AgainstNull(fullName, nameof(fullName));
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        }

        private void ValidateTypeId(PartnerType partnerType, string typeId, Email email)
        {
            Guard.AgainstNullOrWhiteSpace(typeId, nameof(typeId));

            switch (partnerType)
            {
                case PartnerType.Calendar:
                    if (!Guid.TryParse(typeId, out _))
                        throw new DomainException("TypeId for Calendar must be a valid GUID (reference to a user).");
                    break;

                case PartnerType.Google:
                case PartnerType.Microsoft:
                    if (email == null)
                        throw new DomainException("Email is required for Google/Microsoft partners.");
                    if (!string.Equals(typeId, email.Address, StringComparison.OrdinalIgnoreCase))
                        throw new DomainException("TypeId must match the email for Google/Microsoft partners.");
                    break;

                default:
                    break;
            }
        }
    }
}