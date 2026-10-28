using Galo.Application.Common.Models;
using Galo.Application.Ingressos.Queries;

namespace Galo.Application.Ingressos.Interfaces
{
    public interface ICompraQueryRepository
    {
        Task<CompraDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<CompraResumoDto>> ListarDoTorcedorAsync(Guid torcedorId, int pagina, int tamanho, CancellationToken cancellationToken);
    }
}
