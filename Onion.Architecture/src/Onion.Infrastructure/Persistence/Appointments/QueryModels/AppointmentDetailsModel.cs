namespace Onion.Infrastructure.Persistence.Appointments.QueryModels
{
    public sealed class AppointmentDetailsModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string? Image { get; init; }
        public int AppointmentType { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }

        public string Street { get; init; } = null!;
        public string Number { get; init; } = null!;

        public List<AppointmentDateSummaryModel> Dates { get; init; } = new();
        public List<ParticipantSummaryModel> Participants { get; init; } = new();
    }
}
