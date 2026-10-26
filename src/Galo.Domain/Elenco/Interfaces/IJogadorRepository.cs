using Galo.Domain.Elenco.Entities;

namespace Galo.Domain.Elenco.Interfaces
{
    /// <summary>
    /// Repositório do Aggregate Root Jogador (lado de ESCRITA).
    /// As leituras ficam em IJogadorQueryRepository (CQRS) na camada Application.
    /// </summary>
    public interface IJogadorRepository
    {
        Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Jogador jogador, CancellationToken cancellationToken = default);
        void Atualizar(Jogador jogador);
    }
}
