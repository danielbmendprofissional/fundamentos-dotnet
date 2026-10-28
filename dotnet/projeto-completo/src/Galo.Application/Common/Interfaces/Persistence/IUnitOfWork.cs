namespace Galo.Application.Common.Interfaces.Persistence
{
    /// <summary>
    /// Unit of Work: UMA transação por comando. Os repositórios só "preparam" as mudanças; o CommitAsync grava
    /// tudo junto (e dispara os eventos de domínio depois de gravar).
    /// </summary>
    public interface IUnitOfWork
    {
        /// <exception cref="Common.Exceptions.ConcurrencyConflictException">Outro processo alterou a mesma linha (concorrência otimista).</exception>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>Descarta o que está rastreado em memória (necessário antes de recarregar e tentar de novo após conflito).</summary>
        void DiscardChanges();
    }
}
