using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Competicoes.Enums;

namespace Galo.Domain.Competicoes.Entities
{
    public sealed class Campeonato : AggregateRoot
    {
        public string Nome { get; private set; } = string.Empty;
        public int Temporada { get; private set; }
        public TipoCampeonato Tipo { get; private set; }
        public StatusCampeonato Status { get; private set; }

        private Campeonato() { } // EF Core

        private Campeonato(string nome, int temporada, TipoCampeonato tipo)
        {
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Guard.AgainstInvalidEnum(tipo, nameof(tipo));
            Guard.Against<DomainException>(temporada < 1908 || temporada > 2100, "Temporada inválida.");

            Nome = nome.Trim();
            Temporada = temporada;
            Tipo = tipo;
            Status = StatusCampeonato.EmAndamento;
        }

        public static Campeonato Criar(string nome, int temporada, TipoCampeonato tipo) => new(nome, temporada, tipo);

        public void Encerrar()
        {
            Guard.Against<DomainException>(Status == StatusCampeonato.Encerrado, "O campeonato já foi encerrado.");
            Status = StatusCampeonato.Encerrado;
            SetDataAtualizacao();
        }
    }
}
