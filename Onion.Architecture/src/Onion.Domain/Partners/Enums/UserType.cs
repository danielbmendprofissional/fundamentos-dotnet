using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onion.Domain.Partners.Enums
{
    public enum PartnerType
    {
        [Description("Calendar")]
        Calendar = 1,
        [Description("Google")]
        Google = 2,
        [Description("Microsoft")]
        Microsoft = 3,
    }
}
