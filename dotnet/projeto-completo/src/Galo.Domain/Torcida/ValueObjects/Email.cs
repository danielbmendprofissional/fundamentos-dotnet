using System.Text.RegularExpressions;
using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Torcida.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Endereco { get; }

        private static readonly Regex _regex = new(
            @"^[\w\.\-+]+@[\w\.\-]+\.\w{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Email(string endereco)
        {
            Guard.AgainstNullOrWhiteSpace(endereco, nameof(endereco));
            endereco = endereco.Trim();
            Guard.Against<DomainException>(!_regex.IsMatch(endereco), "Email inválido.");

            Endereco = endereco.ToLowerInvariant();
        }

        public static Email Create(string endereco) => new(endereco);

        public override string ToString() => Endereco;

        protected override IEnumerable<object> GetEqualityComponents() { yield return Endereco; }
    }
}
