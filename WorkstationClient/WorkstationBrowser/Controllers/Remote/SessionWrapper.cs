using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers.Remote{
    [Serializable]
    [XmlInclude(typeof(SessionClient))]

    [XmlInclude(typeof(UsersModel))]
    public class SessionWrapper {

        public SessionClient WorkstationSession { get; private set; }
        public LogInModel OriginalInput { get; private set; }
        public String SavedUsername { get; private set; }
        public UsersModel CurrentUser { get; private set; }
        public String ConnectionToken { get; private set; }

        public SessionWrapper(String Username, String Password, String Token, LogInModel model, bool Autolog = false) {
            WorkstationSession = new SessionClient();
            WorkstationSession.ClientCredentials.UserName.UserName = Username;
            WorkstationSession.ClientCredentials.UserName.Password = Password;
            ConnectionToken = Token;
            SavedUsername = Username;
            OriginalInput = model;

            //if (Autolog)
            //    CurrentUser = WorkstationSession.LogIn(SavedUsername, ConnectionToken);
        }

        public bool LogIn(){
            try
            {
                CurrentUser = WorkstationSession.LogIn(SavedUsername, ConnectionToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LogOut(){
            try
            {
                WorkstationSession.LogOut(CurrentUser);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<ProjectModel> GetAllProjects(){
            return WorkstationSession.GetAllProjects();
        }

        public IEnumerable<UsersModel> GetAllUsers()
        {
            return WorkstationSession.GetAllUsers();
        }

        public IEnumerable<TeamModel> GetAllTeams()
        {
            return WorkstationSession.GetAllTeams();
        }

        public IEnumerable<DepartmentModel> GetAllDepartments()
        {
            return WorkstationSession.GetAllDepartments();
        }
       
    }
}