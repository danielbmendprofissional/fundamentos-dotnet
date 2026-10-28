using Galo.Application.Common.Models;
using Galo.Application.Torcida.Queries;
using Galo.Domain.Torcida.Enums;

namespace Galo.Application.Torcida.Interfaces
{
    public interface ITorcedorQueryRepository
    {
        Task<PagedResult<TorcedorResumoDto>> ListarAsync(PlanoSocio? plano, int pagina, int tamanho, CancellationToken cancellationToken);
        Task<TorcedorDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken);
    }
}
