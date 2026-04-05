namespace Onion.API.Appointments.Models
{
    public sealed record ParticipantViewModel(
        string Name,
        string Email,
        Guid PartnerId);
}
