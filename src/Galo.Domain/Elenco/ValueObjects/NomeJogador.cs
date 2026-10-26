using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Elenco.ValueObjects
{
    public sealed class NomeJogador : ValueObject
    {
        public string Valor { get; }

        private NomeJogador(string valor)
        {
            Guard.AgainstNullOrWhiteSpace(valor, nameof(valor));
            valor = valor.Trim();
            Guard.Against<DomainException>(valor.Length is < 2 or > 100,
                "O nome do jogador deve ter entre 2 e 100 caracteres.");
            Valor = valor;
        }

        public static NomeJogador Create(string valor) => new(valor);

        public override string ToString() => Valor;
        protected override IEnumerable<object> GetEqualityComponents() { yield return Valor.ToLowerInvariant(); }
    }
}
