using Galo.Domain.Common.Base;
using Galo.Domain.Common.Exceptions;
using Galo.Domain.Common.Validations;

namespace Galo.Domain.Competicoes.Entities
{
    /// <summary>
    /// Setor do estádio para UMA partida (ex.: Norte, Superior Sul). Guarda o ESTOQUE de ingressos.
    /// É o ponto de COMPETIÇÃO do sistema: vários torcedores disputam o mesmo "Disponiveis".
    /// A concorrência otimista (xmin do PostgreSQL) é configurada no mapeamento desta entidade.
    /// </summary>
    public sealed class SetorPartida : Entity
    {
        public Guid PartidaId { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public int Capacidade { get; private set; }
        public int Disponiveis { get; private set; }
        public decimal Preco { get; private set; }

        private SetorPartida() { } // EF Core

        internal SetorPartida(Guid partidaId, string nome, int capacidade, decimal preco)
        {
            Guard.AgainstEmptyGuid(partidaId, nameof(partidaId));
            Guard.AgainstNullOrWhiteSpace(nome, nameof(nome));
            Guard.Against<DomainException>(capacidade <= 0, "A capacidade do setor deve ser maior que zero.");
            Guard.Against<DomainException>(preco <= 0, "O preço do setor deve ser maior que zero.");

            PartidaId = partidaId;
            Nome = nome.Trim();
            Capacidade = capacidade;
            Disponiveis = capacidade;
            Preco = preco;
        }

        internal void Reservar(int quantidade)
        {
            Guard.Against<DomainException>(quantidade <= 0, "A quantidade deve ser maior que zero.");
            Guard.Against<DomainException>(quantidade > Disponiveis,
                $"Ingressos insuficientes no setor {Nome}: restam {Disponiveis}, solicitados {quantidade}.");

            Disponiveis -= quantidade;
            SetDataAtualizacao();
        }

        internal void Liberar(int quantidade)
        {
            Guard.Against<DomainException>(quantidade <= 0, "A quantidade deve ser maior que zero.");
            Guard.Against<DomainException>(Disponiveis + quantidade > Capacidade,
                "A liberação excede a capacidade do setor.");

            Disponiveis += quantidade;
            SetDataAtualizacao();
        }
    }
}
