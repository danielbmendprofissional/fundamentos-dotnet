using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Appointments.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Street { get; init; }
        public string Number { get; init; }
        public string? Complement { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public PostalCode PostalCode { get; init; }
        public string Country { get; init; }

        private Address(PostalCode postalCode, string street, string number,
        string city, string state, string country, string? complement)
        {
            Guard.AgainstNull(postalCode, nameof(postalCode));
            Guard.AgainstNullOrWhiteSpace(street, nameof(street));
            Guard.AgainstNullOrWhiteSpace(number, nameof(number));
            Guard.AgainstNullOrWhiteSpace(city, nameof(city));
            Guard.AgainstNullOrWhiteSpace(state, nameof(state));
            Guard.AgainstNullOrWhiteSpace(country, nameof(country));

            Street = street;
            PostalCode = postalCode;
            Number = number;
            Complement = complement;
            City = city;
            State = state;
            Country = country;
        }

        public static Address Create(string postalCode, string street, string number,
        string city, string state, string country, string? complement = null)
        {
            return new Address(ValueObjects.PostalCode.Create(postalCode), street, number, city, state, country, complement);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return Number;
            yield return Complement;
            yield return City;
            yield return State;
            yield return PostalCode;
            yield return Country;
        }
    }
}
