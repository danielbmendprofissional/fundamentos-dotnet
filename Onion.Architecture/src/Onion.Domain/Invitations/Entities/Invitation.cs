using Onion.Domain.Common.Base;
using Onion.Domain.Common.Validation;
using Onion.Domain.Common.Exceptions;
using Onion.Domain.Invitations.Enums;
using System;

namespace Onion.Domain.Invitations.Entities
{
    public sealed class Invitation : AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public InvitationType Type { get; private set; }
        public Guid? TargetId { get; private set; }
        public InvitationStatus Status { get; private set; }
        public string RecipientEmail { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime? RespondedAt { get; private set; }
        public string? Note { get; private set; }

        public static Invitation CreateForPartner(Guid ownerId, string recipientEmail, Guid partnerId, DateTime? expiresAt = null, string? note = null)
        => new Invitation(ownerId, InvitationType.ToPartner, recipientEmail, partnerId, expiresAt, note);

        public static Invitation CreateForAppointment(Guid ownerId, string recipientEmail, Guid appointmentId, DateTime? expiresAt = null, string? note = null)
            => new Invitation(ownerId, InvitationType.ToAppointment, recipientEmail, appointmentId, expiresAt, note);

        private Invitation(Guid ownerId, InvitationType type, string recipientEmail, Guid? targetId, DateTime? expiresAt, string? note)
        {
            Guard.AgainstNullOrWhiteSpace(recipientEmail, nameof(recipientEmail));
            Guard.Against<DomainException>(ownerId == Guid.Empty, "Invalid owner id.", nameof(ownerId));

            if (type == InvitationType.ToPartner)
                Guard.Against<DomainException>(targetId == null || targetId == Guid.Empty, "Partner invitation requires a Partner id.");
            if (type == InvitationType.ToAppointment)
                Guard.Against<DomainException>(targetId == null || targetId == Guid.Empty, "Appointment invitation requires an Appointment id.");

            OwnerId = ownerId;
            Type = type;
            TargetId = targetId;
            RecipientEmail = recipientEmail;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            Note = note;
            Status = InvitationStatus.Pending;
        }

        public void Accept()
        {
            Guard.Against<DomainException>(Status != InvitationStatus.Pending, "Invitation is not pending.");
            Guard.Against<DomainException>(ExpiresAt.HasValue && ExpiresAt < DateTime.UtcNow, "Invitation is expired.");

            Status = InvitationStatus.Accepted;
            RespondedAt = DateTime.UtcNow;
            SetUpdateDate();
        }

        public void Reject(string? reason = null)
        {
            Guard.Against<DomainException>(Status != InvitationStatus.Pending, "Invitation is not pending.");

            Status = InvitationStatus.Rejected;
            Note = reason ?? Note;
            RespondedAt = DateTime.UtcNow;
            SetUpdateDate();
        }

        public void Cancel(string? reason = null)
        {
            Guard.Against<DomainException>(Status != InvitationStatus.Pending, "Only pending invitations can be cancelled.");

            Status = InvitationStatus.Cancelled;
            Note = reason ?? Note;
            RespondedAt = DateTime.UtcNow;
            SetUpdateDate();
        }

        public void Expire()
        {
            if (Status == InvitationStatus.Pending && ExpiresAt.HasValue && ExpiresAt < DateTime.UtcNow)
            {
                Status = InvitationStatus.Expired;
                SetUpdateDate();
            }
        }
    }
}