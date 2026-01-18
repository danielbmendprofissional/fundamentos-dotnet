using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.ValueObjects;
using System;
using System.Xml.Linq;

namespace CleanArchitecture.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public FullName Name { get; private set; }
        public Email Email { get; private set; }
        public Phone Phone { get; private set; }
        public Orientation Orientation { get; private set; }

        private User(
            FullName fullName,
            Email email,
            Phone phone,
            Orientation orientation)
        {
            Validate(fullName, email, phone);

            Name = fullName;
            Email = email;
            Phone = phone;
            Orientation = orientation;
        }

        public static User Create(
            FullName name, Email email, Phone phone,
            Orientation orientation = Orientation.NoInformed)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(phone, nameof(phone));
            Guard.AgainstInvalidEnum<Orientation>(orientation, nameof(orientation));

            return new User(
                name,
                email,
                phone,
                orientation);
        }

        private static void Validate(
            FullName name,
            Email email,
            Phone phone)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(email, nameof(email));
            Guard.AgainstNull(phone, nameof(phone));
        }
    }
}
