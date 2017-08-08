using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Workstation.Model;
using WorkstationMessaging.Model;
using WorkstationServices.Data;

namespace WorkstationServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class Session : ISession{

        private static HashSet<UsersModel> OnlineUsers;

        private ServicesManagementEntities entities;
        static Session(){
            OnlineUsers = new HashSet<UsersModel>();
        }

        public UsersModel LogIn(string Username, string Token){
            entities = new ServicesManagementEntities();
            var CurrentUser = entities.Users.Single(user => user.username.Equals(Username));
            UsersModel CurrentUserModel = new UsersModel()
            {
                id = CurrentUser.id,
                email = CurrentUser.email,
                username = Username,
                team_id = CurrentUser.team_id
            };

            OnlineUsers.Add(CurrentUserModel);
            return CurrentUserModel;
        }

        public void LogOut(UsersModel user)
        {
            throw new NotImplementedException();
        }

        public IList<ProjectModel> GetAllProjects()
        {
            throw new NotImplementedException();
        }

        public IList<UsersModel> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public IList<DepartmentModel> GetAllDepartments()
        {
            throw new NotImplementedException();
        }

        public IList<TeamModel> GetAllTeams()
        {
            throw new NotImplementedException();
        }
    }
}
