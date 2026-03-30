using Onion.Domain.Appointments.Enums;
using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Common.Exceptions;

namespace Onion.Domain.Appointments.Entities
{
    public sealed class Participant : Entity
    {
        public string Name { get; private set;  }
        public string Email { get; private set; }
        public ParticipantSituation Situation { get; private set; }
        public Guid PartnerId { get; private init; }

        internal static Participant Create(
            string name, 
            string email, 
            Guid partnerId)
        => new(name, email, partnerId);

        private Participant(
            string name, 
            string email, 
            Guid partnerId)
        {
            Validate(name, email, partnerId);

            Situation = ParticipantSituation.Invited;
            Name = name;
            Email = email;
            PartnerId = partnerId;
        }

        private void Validate(
            string name,
            string email,
            Guid partnerId)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            Guard.AgainstEmptyGuid(partnerId, nameof(partnerId));
        }

        public void Update(string name, string email)
        {
            Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            Guard.AgainstNullOrWhiteSpace(email, nameof(email));
            Name = name;
            Email = email;
        }

        public void UpdateSituation(ParticipantSituation situation)
        {
            Guard.AgainstInvalidEnum<ParticipantSituation>(situation, nameof(situation));

            Guard.Against<DomainException>(
                Situation == ParticipantSituation.Rejected && situation == ParticipantSituation.Accepted,
                "Cannot change situation from rejected to accepted.");

            Guard.Against<DomainException>(
                Situation == ParticipantSituation.Accepted && situation == ParticipantSituation.Rejected,
                "Cannot change situation from accepted to rejected.");


            Situation = situation;
        }
    }
}