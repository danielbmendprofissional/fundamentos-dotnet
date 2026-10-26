using System.Text.RegularExpressions;
using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Torcida.Entities
{
    /// <summary>Entidade filha do agregado Torcedor (construtor internal: só o Torcedor cria).</summary>
    public sealed class Endereco : Entity
    {
        public Guid TorcedorId { get; private set; }
        public string Cep { get; private set; } = string.Empty;
        public string Logradouro { get; private set; } = string.Empty;
        public string Numero { get; private set; } = string.Empty;
        public string Bairro { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string Estado { get; private set; } = string.Empty;
        public string Pais { get; private set; } = string.Empty;
        public string? Complemento { get; private set; }

        private Endereco() { } // EF Core

        internal Endereco(Guid torcedorId, string cep, string logradouro, string numero, string bairro,
            string cidade, string estado, string pais, string? complemento = null)
        {
            Guard.AgainstEmptyGuid(torcedorId, nameof(torcedorId));
            Validar(cep, logradouro, numero, bairro, cidade, estado, pais);

            TorcedorId = torcedorId;
            Cep = ApenasDigitos(cep);
            Logradouro = logradouro.Trim();
            Numero = numero.Trim();
            Bairro = bairro.Trim();
            Cidade = cidade.Trim();
            Estado = estado.Trim().ToUpperInvariant();
            Pais = pais.Trim();
            Complemento = complemento?.Trim();
        }

        private static string ApenasDigitos(string cep) => new(cep.Where(char.IsDigit).ToArray());

        private static void Validar(string cep, string logradouro, string numero, string bairro,
            string cidade, string estado, string pais)
        {
            Guard.AgainstNullOrWhiteSpace(cep, nameof(cep));
            Guard.Against<DomainException>(!Regex.IsMatch(ApenasDigitos(cep), @"^\d{8}$"), "CEP inválido.");
            Guard.AgainstNullOrWhiteSpace(logradouro, nameof(logradouro));
            Guard.Against<DomainException>(logradouro.Trim().Length < 3, "Logradouro muito curto.");
            Guard.AgainstNullOrWhiteSpace(numero, nameof(numero));
            Guard.AgainstNullOrWhiteSpace(bairro, nameof(bairro));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.AgainstNullOrWhiteSpace(estado, nameof(estado));
            Guard.AgainstNullOrWhiteSpace(pais, nameof(pais));
        }
    }
}
