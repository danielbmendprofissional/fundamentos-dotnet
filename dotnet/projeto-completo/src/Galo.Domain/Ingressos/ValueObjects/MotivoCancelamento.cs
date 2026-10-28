using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Ingressos.ValueObjects
{
    public sealed class MotivoCancelamento : ValueObject
    {
        public string Codigo { get; }
        public string Descricao { get; }

        // Conjunto fechado de motivos padronizados do domínio
        private static readonly Dictionary<string, string> _motivosPadrao = new()
        {
            { "TorcedorDesistiu", "Torcedor desistiu da compra" },
            { "ErroPagamento", "Erro no processamento do pagamento" },
            { "IngressosEsgotados", "Ingressos esgotados no setor" },
            { "PartidaCancelada", "A partida foi cancelada" },
            { "Outro", "Outro motivo não especificado" }
        };

        private MotivoCancelamento(string codigo)
        {
            Guard.AgainstNullOrWhiteSpace(codigo, nameof(codigo));
            Guard.Against<DomainException>(!_motivosPadrao.ContainsKey(codigo),
                $"Motivo de cancelamento '{codigo}' não é válido.");

            Codigo = codigo;
            Descricao = _motivosPadrao[codigo];
        }

        public static MotivoCancelamento TorcedorDesistiu() => new("TorcedorDesistiu");
        public static MotivoCancelamento ErroPagamento() => new("ErroPagamento");
        public static MotivoCancelamento IngressosEsgotados() => new("IngressosEsgotados");
        public static MotivoCancelamento PartidaCancelada() => new("PartidaCancelada");
        public static MotivoCancelamento Outro() => new("Outro");

        public static MotivoCancelamento De(string codigo) => new(codigo);

        public override string ToString() => Descricao;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Codigo;
            yield return Descricao;
        }
    }
}
