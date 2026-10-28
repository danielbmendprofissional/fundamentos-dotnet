using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record ParticipantRequest(
        string Name,
        string Email,
        Guid PartnerId);
}
