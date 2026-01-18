using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Enums
{
    public enum Orientation
    {
        [Description("No informed")]
        NoInformed = 0,
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2,
        [Description("Other")]
        Other = 3,
    }
}
