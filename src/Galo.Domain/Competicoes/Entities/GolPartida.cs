using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;
using Galo.Domain.Competicoes.Enums;

namespace Galo.Domain.Competicoes.Entities
{
    /// <summary>Entidade filha de Partida. ClubeId = clube BENEFICIADO pelo gol.</summary>
    public sealed class GolPartida : Entity
    {
        public Guid PartidaId { get; private set; }
        public Guid ClubeId { get; private set; }
        public Guid? JogadorId { get; private set; }
        public int Minuto { get; private set; }
        public TipoGol Tipo { get; private set; }

        private GolPartida() { } // EF Core

        internal GolPartida(Guid partidaId, Guid clubeId, Guid? jogadorId, int minuto, TipoGol tipo)
        {
            Guard.AgainstEmptyGuid(partidaId, nameof(partidaId));
            Guard.AgainstEmptyGuid(clubeId, nameof(clubeId));
            Guard.AgainstInvalidEnum(tipo, nameof(tipo));
            Guard.Against<DomainException>(minuto < 1 || minuto > 130, "O minuto do gol deve estar entre 1 e 130.");

            PartidaId = partidaId;
            ClubeId = clubeId;
            JogadorId = jogadorId;
            Minuto = minuto;
            Tipo = tipo;
        }
    }
}
