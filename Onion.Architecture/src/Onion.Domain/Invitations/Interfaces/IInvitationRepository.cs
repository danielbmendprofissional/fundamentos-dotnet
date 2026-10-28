    using Onion.Domain.Invitations.Entities;
    using Onion.Domain.Common.Specifications;
    
    public interface IInvitationRepository
    {
        Task<Invitation?> GetByIdAsync(Guid Id,
            CancellationToken cancellationToken = default);

        Task AddAsync(Invitation invitation,
            CancellationToken cancellationToken = default);
    }
