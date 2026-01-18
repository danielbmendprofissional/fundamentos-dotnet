using CleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Adress { get; }

        private static readonly Regex _regex = new(
            @"^[\w\.-]+@[\w\.-]+\.\w{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Email(string adress)
        {
            Guard.AgainstNullOrWhiteSpace(adress, nameof(adress));
            Guard.Against<DomainException>(!_regex.IsMatch(adress), "invalid e-mail");

            Adress = adress.Trim().ToLowerInvariant();
        }

        public static Email Create(string adress)
            => new Email(adress);

        public override string ToString() => Adress;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Adress;
        }
    }
}
