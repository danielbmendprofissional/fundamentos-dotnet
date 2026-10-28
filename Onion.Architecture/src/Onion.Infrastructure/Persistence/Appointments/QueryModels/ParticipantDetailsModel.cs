using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.QueryModels
{
    public sealed class ParticipantDetailsModel
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public int Situation { get; init; }
    }
}
