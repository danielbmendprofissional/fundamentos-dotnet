using Galo.Domain.Torcida.Entities;
using Galo.Domain.Torcida.ValueObjects;

namespace Galo.Domain.Torcida.Interfaces
{
    public interface ITorcedorRepository
    {
        Task<Torcedor?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistePorCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);
        Task<bool> ExistePorEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Torcedor torcedor, CancellationToken cancellationToken = default);
        void Atualizar(Torcedor torcedor);
    }
}
