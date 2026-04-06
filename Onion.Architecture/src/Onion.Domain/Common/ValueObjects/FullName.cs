using Onion.Domain.Common.Base;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Common.ValueObjects
{
    public sealed class FullName : ValueObject
    {
        public string Name { get; }
        public string LastName { get; }
        public string FormatedFullName { get; }

        private FullName(string fullName)
        {
            Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));

            var parts = fullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Guard.Against<DomainException>(parts.Length < 2,
                "Full name needs to have a first and a last name.");

            LastName = parts.Last();
            Name = string.Join(" ", parts.Take(parts.Length - 1));

            FormatedFullName = string.Join(" ", parts);
        }

        public static FullName Create(string fullName)
            => new (fullName);

        public string ResumName => $"{Name.Split(' ').First()} {LastName}";
        public override string ToString() => FormatedFullName;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FormatedFullName.ToLowerInvariant();
        }

    }
}
