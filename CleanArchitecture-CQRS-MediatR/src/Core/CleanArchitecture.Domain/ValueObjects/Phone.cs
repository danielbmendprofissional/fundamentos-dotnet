using CleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.ValueObjects
{
    public sealed class Phone : ValueObject
    {
        public string Number { get; }

        private Phone(string number)
        {
            Guard.AgainstNullOrWhiteSpace(number, nameof(number));

            var digits = new string(number.Where(char.IsDigit).ToArray());

            Guard.Against<DomainException>(
                digits.Length is < 10 or > 11,
                "phone number needs to contain 10 or 11 digits");

            Number = digits;
        }

        public static Phone Create(string number)
            => new Phone(number);

        public override string ToString()
        {
            if (Number.Length == 11)
                return Convert.ToInt64(Number).ToString(@"\(00\) 00000\-0000");

            return Convert.ToInt64(Number).ToString(@"\(00\) 0000\-0000");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
        }
    }
}
