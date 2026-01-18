using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces.Repository;
using CleanArchitecture.Infrastructure.Persistence.Context;
using CleanArchitecture.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories
{
    /*
        Implementation of IUserRepository
    */
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email.Adress == email, cancellationToken);
        }
    }
}
