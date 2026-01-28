using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.ThirdExample
{
    public class PermissionLeaf : IAuthorizationRequirement
    {
        private readonly string _permissionName;

        public PermissionLeaf(string name) => _permissionName = name;

        public string GetName() => _permissionName;

        // Logic for a single permission check
        public bool IsAuthorized(string userRole) => userRole == "Admin" || userRole == "Manager";
    }
}
