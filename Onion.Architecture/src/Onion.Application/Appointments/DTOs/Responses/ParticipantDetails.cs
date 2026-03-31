using Onion.Domain.Appointments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record ParticipantDetails(
        Guid Id, 
        string Name,
        string Email,
        string Situation);
}
