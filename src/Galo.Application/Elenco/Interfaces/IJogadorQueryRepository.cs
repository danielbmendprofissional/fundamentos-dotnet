using Galo.Application.Common.Models;
using Galo.Application.Elenco.Queries;
using Galo.Domain.Elenco.Enums;

namespace Galo.Application.Elenco.Interfaces
{
    /// <summary>
    /// Lado de LEITURA (CQRS): devolve DTOs prontos, sem passar pelo agregado.
    /// A implementação usa o GaloQueryDbContext (AsNoTracking + projeção direto para o DTO).
    /// </summary>
    public interface IJogadorQueryRepository
    {
        Task<PagedResult<JogadorResumoDto>> ListarAsync(
            Posicao? posicao, StatusJogador? status, int pagina, int tamanho, CancellationToken cancellationToken);

        Task<JogadorDetalheDto?> ObterDetalheAsync(Guid id, CancellationToken cancellationToken);
    }
}
