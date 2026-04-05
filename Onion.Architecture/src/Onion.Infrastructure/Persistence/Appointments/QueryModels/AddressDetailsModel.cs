using Onion.Domain.Appointments.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Infrastructure.Persistence.Appointments.QueryModels
{
    public class AddressDetailsModel
    {
        public Guid AppointmentId { get; set; }
        public string Street { get; init; }
        public string Number { get; init; }
        public string? Complement { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }
    }
}
