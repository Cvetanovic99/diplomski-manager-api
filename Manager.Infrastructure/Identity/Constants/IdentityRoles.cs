using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Identity.Constants
{
    public static class IdentityRoles
    {
        public static Dictionary<string, string> Roles;

        static IdentityRoles()
        {
           Roles = new Dictionary<string, string>();
           Roles.Add("User", "User");
           Roles.Add("Admin", "Admin");
        }
    }
}
