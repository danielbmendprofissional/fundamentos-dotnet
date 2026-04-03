using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Appointments.DTOs.Requests
{
    public sealed record AddressRequest(
        string Street,
        string Number,
        string? Complement,
        string City,
        string State,
        string PostalCode,
        string Country);
}
