using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns.Composite.ThirdExample.Factory
{
    public class PermissionFactory : IPermissionFactory
    {
        // In a real scenario, you might inject a DbContext or repository to fetch policy data
        // private readonly MyDbContext _context;
        // public PermissionFactory(MyDbContext context) => _context = context;

        public IAuthorizationRequirement CreatePolicy(string policyType)
        {
            // Simulating fetching policy data from a database
            return policyType switch
            {
                "BasicUser" => new PermissionLeaf("READ_ONLY"),

                "Manager" => BuildManagerPolicy(),

                "SuperAdmin" => BuildAdminPolicy(),

                _ => throw new ArgumentException("Invalid Policy Type")
            };
        }

        private IAuthorizationRequirement BuildManagerPolicy()
        {
            var group = new RoleComposite("Manager_Group");
            group.Add(new PermissionLeaf("READ_PRIVILEGE"));
            group.Add(new PermissionLeaf("WRITE_PRIVILEGE"));
            return group;
        }

        private IAuthorizationRequirement BuildAdminPolicy()
        {
            var adminGroup = new RoleComposite("Admin_Root");

            //The composite can contain other composites
            adminGroup.Add(BuildManagerPolicy());
            adminGroup.Add(new PermissionLeaf("DELETE_PRIVILEGE"));
            adminGroup.Add(new PermissionLeaf("DATABASE_ACCESS"));

            return adminGroup;
        }
    }
}
