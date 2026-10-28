using Onion.Domain.Appointments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Responses
{
    public sealed record AddressDetails(
        Guid Id,
        string Street,
        string Number,
        string Complement,
        string City,
        string State,
        string Country);
}
