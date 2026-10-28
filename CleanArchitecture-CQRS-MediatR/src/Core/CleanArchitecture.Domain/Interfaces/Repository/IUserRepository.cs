using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces.Repository.Base;

namespace CleanArchitecture.Domain.Interfaces.Repository
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email, CancellationToken cancellationToken);
    }
}
