using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Competicoes.Entities
{
    /// <summary>
    /// Clube (o Galo e os adversários). EhOGalo marca o "nosso" clube para facilitar
    /// consultas do tipo "jogos do Galo".
    /// </summary>
    public sealed class Clube : AggregateRoot
    {
        public string Nome { get; private set; } = string.Empty;
        public string Sigla { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string Uf { get; private set; } = string.Empty;
        public int AnoFundacao { get; private set; }
        public bool EhOGalo { get; private set; }

        private Clube() { } // EF Core

        private Clube(string nome, string sigla, string cidade, string uf, int anoFundacao, bool ehOGalo)
        {
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Guard.AgainstNullOrWhiteSpace(sigla, nameof(sigla));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.AgainstNullOrWhiteSpace(uf, nameof(uf));
            Guard.Against<DomainException>(sigla.Trim().Length != 3, "A sigla deve ter 3 letras.");
            Guard.Against<DomainException>(uf.Trim().Length != 2, "A UF deve ter 2 letras.");
            Guard.Against<DomainException>(anoFundacao < 1850 || anoFundacao > DateTime.UtcNow.Year, "Ano de fundação inválido.");

            Nome = nome.Trim();
            Sigla = sigla.Trim().ToUpperInvariant();
            Cidade = cidade.Trim();
            Uf = uf.Trim().ToUpperInvariant();
            AnoFundacao = anoFundacao;
            EhOGalo = ehOGalo;
        }

        public static Clube Criar(string nome, string sigla, string cidade, string uf, int anoFundacao, bool ehOGalo = false)
            => new(nome, sigla, cidade, uf, anoFundacao, ehOGalo);
    }
}
