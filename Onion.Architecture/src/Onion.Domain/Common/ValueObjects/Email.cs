using System;
using System.Net.Mail;
using Onion.Domain.Common.Validation;
using Onion.Domain.Common.Exceptions;

namespace Onion.Domain.Common.ValueObjects
{
    public sealed class Email
    {
        public string Address { get; private set; }

        private Email(string address)
        {
            Guard.AgainstNullOrWhiteSpace(address, nameof(address));
            try
            {
                var _ = new MailAddress(address);
            }
            catch (FormatException)
            {
                throw new DomainException("Invalid email format.");
            }

            Address = address;
        }

        public static Email Create(string address)
            => new (address);

        public override string ToString() => Address;
    }
}