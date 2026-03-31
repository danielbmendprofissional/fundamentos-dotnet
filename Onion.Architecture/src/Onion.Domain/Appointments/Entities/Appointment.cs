using Onion.Domain.Appointments.Enums;
using Onion.Domain.Appointments.ValueObjects;
using Onion.Domain.Common.Base;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Onion.Domain.Appointments.Entities
{
    public sealed class Appointment : AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Image { get; private set; } 
        public AppointmentType AppointmentType { get; private set; }
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public Address Address { get; private set; } 

        private readonly List<AppointmentDate> _dates = new();
        public IReadOnlyCollection<AppointmentDate> Dates => _dates.AsReadOnly();

        private readonly List<Participant> _participants = new();
        public IReadOnlyCollection<Participant> Participants => _participants.AsReadOnly();

        public static Appointment Create(
            Guid ownerId,
            string name,
            AppointmentType appointmentType,
            DateOnly startDate,
            DateOnly endDate,
            Address address,
            string? description = null,
            string? image = null)
        => new(ownerId, name, appointmentType, startDate, endDate, address, description, image);

        private Appointment(
            Guid ownerId,
            string name,
            AppointmentType appointmentType,
            DateOnly startDate,
            DateOnly endDate,
            Address address,
            string? description,
            string? image)
        {
            Guard.Against<DomainException>(ownerId == Guid.Empty, "Invalid owner id.", nameof(ownerId));
            Validate(name, appointmentType, startDate, endDate, address);

            OwnerId = ownerId;
            Name = name;
            Description = description;
            Image = image;
            AppointmentType = appointmentType;
            StartDate = startDate;
            EndDate = endDate;
            Address = address;
        }

        public void Update(
            string name,
            AppointmentType appointmentType,
            DateOnly startDate,
            DateOnly endDate,
            Address address,
            string? description = null,
            string? image = null)
        {
            Validate(name, appointmentType, startDate, endDate, address);

            Name = name;
            Description = description;
            Image = image;
            AppointmentType = appointmentType;
            StartDate = startDate;
            EndDate = endDate;
            Address = address;

            SetUpdateDate();
        }

        public void AddDate(
            string name,
            DateTime startDate,
            DateTime endDate,
            string? description = null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow), 
                "Cannot add dates to a past appointment.");
            
            Guard.Against<DomainException>(
                _dates.Any(d => d.StartDate.Date == startDate.Date), 
                $"A date for {startDate.ToShortDateString()} already exists.", nameof(startDate));
            
            Guard.Against<DomainException>(
                DateOnly.FromDateTime(startDate) < StartDate || DateOnly.FromDateTime(endDate) > EndDate,
                "The date must be within the appointment's start and end period.");

            var newDate = AppointmentDate.Create(name, endDate, startDate, description);
            _dates.Add(newDate);
            SetUpdateDate();
        }

        public void UpdateDate(
            Guid dateId,
            string name,
            string? description,
            DateTime startDate,
            DateTime endDate)
        {
            var date = _dates.FirstOrDefault(x => x.Id == dateId);

            Guard.Against<DomainException>(
                 date is null,
                "Specific date not found in this appointment.");

            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot update dates of a past appointment.");

            Guard.Against<DomainException>(
                 DateOnly.FromDateTime(startDate) < StartDate || DateOnly.FromDateTime(endDate) > EndDate,
                "The updated date must be within the appointment's period.");

            Guard.Against<DomainException>(
                _dates.Any(d => d.Id != dateId && d.StartDate.Date == startDate.Date),
                $"Another entry for {startDate.ToShortDateString()} already exists.");


            date!.Update(name, startDate, endDate, description);
            SetUpdateDate();
        }

        public void RemoveDate(Guid dateId)
        {
            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot remove dates from a past appointment.");

            var date = _dates.FirstOrDefault(x => x.Id == dateId);
            Guard.Against<DomainException>(
                date is null,
                "Specific date not found in this appointment.");
            
            _dates.Remove(date!);
            SetUpdateDate();
        }

        public void AddParticipant(string name, string email, Guid partnerId)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));

            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot add participants to a past appointment.");

            Guard.Against<DomainException>(
                _participants.Any(p => p.PartnerId == partnerId),
                $"This participant is already registered for this appointment.");

            var participant = Participant.Create(name, email, partnerId);

            _participants.Add(participant);
            SetUpdateDate();
        }

        public void UpdateParticipant(Guid participantId, string name, string email)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));

            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot update participants of a past appointment.");

            var participant = _participants.FirstOrDefault(x => x.Id == participantId);
            Guard.Against<DomainException>(
                participant is null,
                "Specific participant not found in this appointment.");

            participant!.Update(name, email);
            SetUpdateDate();
        }

        public void UpdateParticipantSituation(Guid participantId, ParticipantSituation situation)
        {
            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot update participants of a past appointment.");

            var participant = _participants.FirstOrDefault(x => x.Id == participantId);
            Guard.Against<DomainException>(
                participant is null,
                "Specific participant not found in this appointment.");

            participant!.UpdateSituation(situation);
            SetUpdateDate();
        }

        public void RemoveParticipant(Guid participantId)
        {
            Guard.Against<DomainException>(
                EndDate < DateOnly.FromDateTime(DateTime.UtcNow),
                "Cannot add participants to a past appointment.");

            var participant = _participants.FirstOrDefault(x => x.Id == participantId);
            Guard.Against<DomainException>(
                participant is null,
                "Specific participant not found in this appointment.");

            _participants.Remove(participant!);
            SetUpdateDate();

        }

        private void Validate(
            string name,
            AppointmentType appointmentType,
            DateOnly startDate,
            DateOnly endDate,
            Address address)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstInvalidEnum<AppointmentType>(appointmentType, nameof(appointmentType));
            Guard.Against<DomainException>(startDate > endDate, "Start date must be before end date.", nameof(startDate));
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            Guard.Against<DomainException>(startDate < today, "Start date cannot be in the past.", nameof(startDate));

            Guard.AgainstNull<Address>(address, nameof(address));
        }
    }
}
