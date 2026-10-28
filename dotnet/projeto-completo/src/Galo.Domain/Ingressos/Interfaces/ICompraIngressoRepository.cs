using Galo.Domain.Ingressos.Entities;

namespace Galo.Domain.Ingressos.Interfaces
{
    public interface ICompraIngressoRepository
    {
        Task<CompraIngresso?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AdicionarAsync(CompraIngresso compra, CancellationToken cancellationToken = default);
        void Atualizar(CompraIngresso compra);
    }
}
