using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Competicoes.ValueObjects
{
    /// <summary>VO com mais de um valor: mapeado como OwnsOne (2 colunas na tabela Partidas).</summary>
    public sealed class Placar : ValueObject
    {
        public int GolsMandante { get; }
        public int GolsVisitante { get; }

        // Os nomes dos parâmetros casam com as propriedades: o EF Core usa este construtor para materializar.
        private Placar(int golsMandante, int golsVisitante)
        {
            Guard.Against<DomainException>(golsMandante < 0 || golsVisitante < 0, "O placar não pode ser negativo.");
            GolsMandante = golsMandante;
            GolsVisitante = golsVisitante;
        }

        public static Placar Zerado() => new(0, 0);
        public static Placar Create(int golsMandante, int golsVisitante) => new(golsMandante, golsVisitante);

        public Placar GolMandante() => new(GolsMandante + 1, GolsVisitante);
        public Placar GolVisitante() => new(GolsMandante, GolsVisitante + 1);

        public override string ToString() => $"{GolsMandante} x {GolsVisitante}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return GolsMandante;
            yield return GolsVisitante;
        }
    }
}
