using Galo.Application.Common.Models;
using Galo.Application.Competicoes.Queries;
using Galo.Domain.Competicoes.Enums;

namespace Galo.Application.Competicoes.Interfaces
{
    public interface IPartidaQueryRepository
    {
        Task<PagedResult<PartidaResumoDto>> ListarAsync(
            Guid? campeonatoId, Guid? clubeId, StatusPartida? status, int pagina, int tamanho, CancellationToken cancellationToken);

        Task<PartidaDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<ClassificacaoItemDto>> ObterClassificacaoAsync(Guid campeonatoId, CancellationToken cancellationToken);
    }

    /// <summary>Consultas de cadastros simples (combos/listas): poucas linhas, mudam raramente => ótimas para cache.</summary>
    public interface ICatalogoQueryRepository
    {
        Task<IReadOnlyList<ClubeDto>> ListarClubesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<EstadioDto>> ListarEstadiosAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<CampeonatoDto>> ListarCampeonatosAsync(CancellationToken cancellationToken);
    }
}
