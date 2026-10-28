using Galo.Domain.Competicoes.Entities;

namespace Galo.Domain.Competicoes.Interfaces
{
    public interface IClubeRepository
    {
        Task<Clube?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Clube clube, CancellationToken cancellationToken = default);
    }

    public interface IEstadioRepository
    {
        Task<Estadio?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Estadio estadio, CancellationToken cancellationToken = default);
    }

    public interface ICampeonatoRepository
    {
        Task<Campeonato?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Campeonato campeonato, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Carrega a Partida COMPLETA (gols + setores): é o agregado inteiro que muda dentro de uma transação.
    /// </summary>
    public interface IPartidaRepository
    {
        Task<Partida?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Partida partida, CancellationToken cancellationToken = default);
        void Atualizar(Partida partida);
    }
}
