//create full enum helper where store all enums like user roles, trip status, etc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace theTripChalleng.Helpers
{
    public static class EnumHelper
    {
        public enum UserRole
        {
            Admin = 1,
            User = 2
        }

        public enum RequestStatus
        {
            Pending = 0,
            Approved = 1,
            Rejected = 2
        }

        // Add more enums as needed
    }
}