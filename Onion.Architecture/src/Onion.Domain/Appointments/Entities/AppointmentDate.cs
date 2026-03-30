using Onion.Domain.Common.Base;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Onion.Domain.Appointments.Entities
{
    public sealed class AppointmentDate : Entity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        internal static AppointmentDate Create(
            string name,
            DateTime startDate,
            DateTime endDate,
            string? description = null)
        => new AppointmentDate(name, startDate, endDate, description);

        private AppointmentDate(
            string name,
            DateTime startDate,
            DateTime endDate,
            string? description)
        {
            Validate(name, startDate, endDate);

            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }

        internal void Update(
            string name,
            DateTime startDate,
            DateTime endDate,
            string? description = null)
        {
            Validate(name, startDate, endDate);

            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }

        private void Validate(
            string name,
            DateTime startDate,
            DateTime endDate)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNull<DateTime>(startDate, nameof(startDate));
            Guard.AgainstNull<DateTime>(endDate, nameof(endDate));
            
            Guard.Against<DomainException>(startDate >= endDate, "Start date must be before end date.", nameof(startDate));
            Guard.Against<DomainException>(
                startDate.Date != endDate.Date,
                "The start and end time of a specific date must be on the same day.");

            Guard.Against<DomainException>(startDate < DateTime.UtcNow, "Start date cannot be in the past.", nameof(startDate));
        }
    }
}
