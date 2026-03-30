using Onion.Domain.Partners.Entities;
using Onion.Domain.Common.Specifications;

namespace Onion.Domain.Partners.Interfaces
{
    public interface IPartnerRepository
    {
        Task<Partner?> GetByIdAsync(Guid Id,
            CancellationToken cancellationToken = default);

        Task AddAsync(Partner partner,
            CancellationToken cancellationToken = default);

        Task<PagedResult<Partner>> ListAsync(ISpecification<Partner> specification,
            CancellationToken cancellationToken = default);
    }
}
