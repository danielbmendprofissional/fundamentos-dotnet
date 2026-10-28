using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Competicoes.Entities
{
    public sealed class Estadio : AggregateRoot
    {
        public string Nome { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public int Capacidade { get; private set; }

        private Estadio() { } // EF Core

        private Estadio(string nome, string cidade, int capacidade)
        {
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Guard.AgainstNullOrWhiteSpace(cidade, nameof(cidade));
            Guard.Against<DomainException>(capacidade <= 0, "A capacidade deve ser maior que zero.");

            Nome = nome.Trim();
            Cidade = cidade.Trim();
            Capacidade = capacidade;
        }

        public static Estadio Criar(string nome, string cidade, int capacidade) => new(nome, cidade, capacidade);
    }
}
