using Onion.Domain.Appointments.Entities;
using Onion.Domain.Common.Specifications;

namespace Onion.Domain.Appointments.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid Id,
            CancellationToken cancellationToken = default);

        Task AddAsync(Appointment appointment,
            CancellationToken cancellationToken = default);

        Task<PagedResult<Appointment>> ListAsync(ISpecification<Appointment> specification,
            CancellationToken cancellationToken = default);
    }
}
