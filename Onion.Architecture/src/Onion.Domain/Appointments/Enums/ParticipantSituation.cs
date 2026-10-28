using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Appointments.Enums
{
    public enum ParticipantSituation
    {
        [Description("Invited")]
        Invited = 0,
        [Description("Rejected")]
        Rejected = 1,
        [Description("Accepted")]
        Accepted = 2,
        [Description("Desativated")]
        Desativated = 3
    }
}
