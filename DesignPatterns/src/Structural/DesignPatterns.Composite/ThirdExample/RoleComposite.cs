using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.ThirdExample
{
    public class RoleComposite : IAuthorizationRequirement
    {
        private readonly string _roleName;
        private readonly List<IAuthorizationRequirement> _requirements = new();

        public RoleComposite(string name) => _roleName = name;

        public void Add(IAuthorizationRequirement requirement) => _requirements.Add(requirement);

        public string GetName() => _roleName;

        // The core of Composite: check if ALL nested permissions are satisfied
        public bool IsAuthorized(string userRole)
        {
            return _requirements.All(r => r.IsAuthorized(userRole));
        }
    }
}
