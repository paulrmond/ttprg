using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BCS.Models
{
    public class UserAccessControl
    {
        string module;
        string role;
        string service = "not water";
        public UserAccessControl(string module,string role)
        {
            this.module = module;
            this.role = role;
        }

        public UserAccessControl(string module, string role,string service)
        {
            this.module = module;
            this.role = role;
        }

        private bool isRoleOk()
        {
            bool roleOk = false;
            if(role.ToLower() == "system administrator" || role.ToLower() == "finance officer" || role.ToLower() == "billing officer" || role.ToLower() == "meter reader")
            {
                roleOk = true;
            }

            return roleOk;
        }

        public bool userCanAccess()
        {
            bool canAccess = false;
            if(module.ToLower() == "data entry" && isRoleOk())
            {
                canAccess = true;

                if(role.ToLower() == "meter reader" && service != "water")
                    canAccess = false;

            }
            else if(module.ToLower() == "billing" && isRoleOk())
            {
                canAccess = true;
            }
            else if (module.ToLower() == "maintenance" && isRoleOk())
            {
                canAccess = true;
            }
            else if (module.ToLower() == "reports" && isRoleOk())
            {
                canAccess = true;
            }

            return canAccess;
        }
    }
}