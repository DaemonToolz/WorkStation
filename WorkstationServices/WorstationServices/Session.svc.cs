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

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession)]
    public class Session : ISession{

        private static HashSet<UsersModel> OnlineUsers;

        private ServicesManagementEntities entities;
        static Session(){
            OnlineUsers = new HashSet<UsersModel>();
        }

        public UsersModel LogIn(string Username, string Token){
            entities = new ServicesManagementEntities();
            var CurrentUser = entities.Users.Single(user => user.username.Equals(Username));
            UsersModel CurrentUserModel = new UsersModel(){
                id = CurrentUser.id,
                email = CurrentUser.email,
                username = Username,
                team_id = CurrentUser.team_id
            };

            OnlineUsers.Add(CurrentUserModel);
            return CurrentUserModel;
        }

        public void LogOut(UsersModel user){
            OnlineUsers.Remove(user);
        }
        

        public IList<ProjectModel> GetAllProjects()
        {
            List<ProjectModel> projects = new List<ProjectModel>();

            foreach (var project in entities.Project)
                projects.Add(new ProjectModel(){
                    id = project.id,
                    name = project.name,
                    root = project.root
                });

            return projects;
        }

        public IList<UsersModel> GetAllUsers(){
            List<UsersModel> users = new List<UsersModel>();

            foreach (var user in entities.Users)
                users.Add(new UsersModel()
                {
                     id = user.id,
                     email = user.email,
                     username = user.username,
                     team_id = user.team_id
                });

            return users;
        }

        public IList<DepartmentModel> GetAllDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            foreach (var dept in entities.Department)
                departments.Add(new DepartmentModel()
                {
                    id = dept.id,
                     name = dept.name
                });

            return departments;
        }

        public IList<TeamModel> GetAllTeams()
        {
            List<TeamModel> teams = new List<TeamModel>();

            foreach (var team in entities.Team)
                teams.Add(new TeamModel()
                {
                    id = team.id,
                    department_id = team.department_id,
                    project_id = team.project_id,
                    name = team.name
                });

            return teams;
        }

        public TeamModel GetTeamPerUser(int userid){
            try
            {
                var team = entities.Team.First(teamObj => teamObj.Users.Any(userObj => userObj.id == userid));
                return new TeamModel()
                {
                    id = team.id,
                    name = team.name,
                    department_id = team.department_id,
                    project_id = team.project_id
                };
            } catch { return null; }
        }

        public TeamModel GetTeamPerId(int id)
        {
            try
            {
                var team = entities.Team.First(teamObj => teamObj.id == id);
                return new TeamModel()
                {
                    id = team.id,
                    name = team.name,
                    department_id = team.department_id,
                    project_id = team.project_id
                };
            }
            catch { return null; }
        }
    }


}
