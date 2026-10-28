using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Torcida.ValueObjects
{
    public sealed class NomeCompleto : ValueObject
    {
        public string Nome { get; }
        public string Sobrenome { get; }
        public string NomeCompletoFormatado { get; }

        private NomeCompleto(string nomeCompleto)
        {
            Guard.AgainstNullOrWhiteSpace(nomeCompleto, nameof(nomeCompleto));

            var partes = nomeCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Guard.Against<DomainException>(partes.Length < 2,
                "O nome completo deve ter pelo menos um nome e um sobrenome.");

            Sobrenome = partes.Last();
            Nome = string.Join(' ', partes.Take(partes.Length - 1));
            NomeCompletoFormatado = string.Join(' ', partes);
        }

        public static NomeCompleto Create(string nomeCompleto) => new(nomeCompleto);

        public string NomeResumido => $"{Nome.Split(' ').First()} {Sobrenome}";

        public override string ToString() => NomeCompletoFormatado;

        protected override IEnumerable<object> GetEqualityComponents() { yield return NomeCompletoFormatado.ToLowerInvariant(); }
    }
}
