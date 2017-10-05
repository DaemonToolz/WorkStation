using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Workstation.Model;
using WorkstationMessaging.Model;
using WorkstationServices.Data;

namespace WorkstationServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Mobile" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Mobile.svc or Mobile.svc.cs at the Solution Explorer and start debugging.
    public class Mobile : IMobile
    {
        private ServicesManagementEntities entities;
     
        public UsersModel LogIn(string Username, string Token)
        {
            entities = new ServicesManagementEntities();
            var CurrentUser = entities.Users.Single(user => user.username.Equals(Username));
            UsersModel CurrentUserModel = new UsersModel()
            {
                id = CurrentUser.id,
                email = CurrentUser.email,
                username = Username,
                team_id = CurrentUser.team_id,
                rank = CurrentUser.Rank1.name,
                rights = CurrentUser.Rank1.rights,
                profilepic = CurrentUser.profilepic
            };

          
            return CurrentUserModel;
        }

    }
}
