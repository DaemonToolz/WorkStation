using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkstationServices.Data;

namespace WorkstationServices.Security
{
    internal static class UserAccessModel
    {
        private static readonly ServicesManagementEntities UserEF = new ServicesManagementEntities();

        public static bool UserValid(String username, String password)
        {
            return UserEF.Users.Any(obj => obj.username.Equals(username) && obj.password.Equals(password));
        }

        public static bool UserExists(String username)
        {
            return UserEF.Users.Any(obj => obj.username.Equals(username) );
        }

    }
}