using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.ThirdExample.Factory
{
    public interface IPermissionFactory
    {
        IAuthorizationRequirement CreatePolicy(string policyType);
    }
}
