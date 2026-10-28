namespace CleanArchitecture.Domain.Interfaces.Repository.Base
{
    public interface IUnitOfWork
    {
        Task Commit(CancellationToken cancellationToken);
    }
}
