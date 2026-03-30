using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Appointments.Enums
{
    public enum AppointmentType
    {
        [Description("Holliday")]
        Holliday = 1,
        [Description("DayOff")]
        DayOff = 2,
        [Description("Vacation")]
        Vacation = 3,
        [Description("Family")]
        Family = 4,
        [Description("Personal")]
        Personal = 5,
        [Description("Social")]
        Social = 6,
        [Description("Business")]
        Business = 7,
        [Description("Other")]
        Other = 8
    }
}
