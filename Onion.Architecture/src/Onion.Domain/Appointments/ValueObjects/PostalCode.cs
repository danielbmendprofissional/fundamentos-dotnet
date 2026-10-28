using Onion.Domain.Common.Base;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Onion.Domain.Appointments.ValueObjects
{
    public sealed class PostalCode : ValueObject
    {
        public string Value { get; init; }

        private PostalCode(string value)
        {
            Guard.AgainstNullOrWhiteSpace(value, nameof(value));

            var normalizado = value.Replace("-", "").Trim();

            if (!Regex.IsMatch(normalizado, @"^\d{8}$"))
                throw new DomainException("Invalid Postal Code. must contains 8 digits.");

            Value = normalizado;
        }

        public static PostalCode Create(string value)
            => new(value);

        public override string ToString()
            => $"{Value[..5]}-{Value[5..]}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
