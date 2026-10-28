using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.ThirdExample
{
    public interface IAuthorizationRequirement
    {
        string GetName();
        bool IsAuthorized(string userRole);
    }
}
