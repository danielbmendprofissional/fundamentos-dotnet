using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Interfaces.Repository.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<T> Get(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken);
    }
}
