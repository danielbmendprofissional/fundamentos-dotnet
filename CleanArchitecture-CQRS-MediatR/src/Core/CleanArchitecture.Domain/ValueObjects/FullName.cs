using CleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.ValueObjects
{
    public sealed class FullName : ValueObject
    {
        public string Name { get; }
        public string LastName { get; }
        public string NormalizedName { get; }

        private FullName(string fullName)
        {
            Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));

            var partes = fullName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Guard.Against<DomainException>(partes.Length < 2,
                "A complete name needs to be a name and lastname.");

            LastName = partes.Last();
            Name = string.Join(" ", partes.Take(partes.Length - 1));

            NormalizedName = string.Join(" ", partes);
        }

        public static FullName Create(string fullName)
            => new FullName(fullName);

        public string ResumName => $"{Name.Split(' ').First()} {LastName}";
        public override string ToString() => NormalizedName;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return NormalizedName.ToLowerInvariant();
        }

    }
}
