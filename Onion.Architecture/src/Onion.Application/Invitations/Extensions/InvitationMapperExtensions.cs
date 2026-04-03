using Onion.Application.Invitations.DTOs.Requests;
using Onion.Domain.Invitations.Entities;
using Onion.Domain.Invitations.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Application.Invitations.Extensions
{
    public static class InvitationMapperExtensions
    {
        public static Invitation ToEntity(this CreateInvitationRequest request)
        {
            if ((InvitationType)request.Type == InvitationType.ToPartner)
            {
                return Invitation.CreateForPartner(
                    request.AccountId,
                    request.RecipientEmail,
                    request.TargetId,
                    request.ExpiresAt,
                    request.Note);
            }

            return Invitation.CreateForAppointment(
                request.AccountId,
                request.RecipientEmail,
                request.TargetId,
                request.ExpiresAt,
                request.Note);
        }
    }
}
