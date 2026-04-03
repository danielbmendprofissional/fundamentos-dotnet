using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.QueryModels
{
    public sealed class AppointmentSummaryModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public int AppointmentType { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
    }
}
