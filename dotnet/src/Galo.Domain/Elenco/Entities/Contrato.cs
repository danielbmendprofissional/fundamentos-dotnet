using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Elenco.Entities
{
    /// <summary>
    /// Entidade FILHA do agregado Jogador: só existe dentro dele, não tem repositório
    /// e o construtor é internal (só o Jogador cria).
    /// </summary>
    public sealed class Contrato : Entity
    {
        public Guid JogadorId { get; private set; }
        public DateOnly Inicio { get; private set; }
        public DateOnly Fim { get; private set; }
        public decimal SalarioMensal { get; private set; }
        public decimal? ClausulaRescisoria { get; private set; }

        private Contrato() { } // EF Core

        internal Contrato(Guid jogadorId, DateOnly inicio, DateOnly fim, decimal salarioMensal, decimal? clausulaRescisoria)
        {
            Guard.AgainstEmptyGuid(jogadorId, nameof(jogadorId));
            Guard.Against<DomainException>(fim <= inicio, "O fim do contrato deve ser posterior ao início.");
            Guard.Against<DomainException>(salarioMensal <= 0, "O salário mensal deve ser maior que zero.");
            Guard.Against<DomainException>(clausulaRescisoria < 0, "A cláusula rescisória não pode ser negativa.");

            JogadorId = jogadorId;
            Inicio = inicio;
            Fim = fim;
            SalarioMensal = salarioMensal;
            ClausulaRescisoria = clausulaRescisoria;
        }

        public bool EstaVigente(DateOnly hoje) => hoje >= Inicio && hoje <= Fim;

        internal bool SobrepoeA(DateOnly inicio, DateOnly fim) => inicio <= Fim && fim >= Inicio;
    }
}
