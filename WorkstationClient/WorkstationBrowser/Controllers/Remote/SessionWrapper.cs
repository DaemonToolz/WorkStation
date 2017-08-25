using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Serialization;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using WorkstationBrowser.Controllers.Generic;
using WorkstationBrowser.Models;
using WorkstationBrowser.SessionReference;
using WorkstationBrowser.Controllers.SignalR;

namespace WorkstationBrowser.Controllers.Remote{
    [Serializable]
    [XmlInclude(typeof(SessionClient))]

    [XmlInclude(typeof(UsersModel))]
    public class SessionWrapper : ISessionCallback {
       
        public SessionClient WorkstationSession { get; private set; }
        public LogInModel OriginalInput { get; private set; }
        public String SavedUsername { get; private set; }
        public UsersModel CurrentUser { get; private set; }
        public String ConnectionToken { get; private set; }
        public bool NotificationPooler { get; set; }
        public NotificationModel[] MyNotifications { get; private set; }
        private HttpSessionStateBase UserSession { get; set; }
        private CacheProvider Cache { get; set; }

        public SessionWrapper() {
            
        }

        public SessionWrapper(String Username, String Password, String Token, LogInModel model, HttpSessionStateBase session, bool Autolog = false) {
            WorkstationSession = new SessionClient(
                new InstanceContext( this ));

            NotificationPooler = false;
            WorkstationSession.ClientCredentials.UserName.UserName = Username;
            WorkstationSession.ClientCredentials.UserName.Password = Password;
            ConnectionToken = Token;
            SavedUsername = Username;
            OriginalInput = model;
            UserSession = session;

            Cache = new CacheProvider();
            //if (Autolog)
            //    CurrentUser = WorkstationSession.LogIn(SavedUsername, ConnectionToken);
        }

        public bool LogIn(){
            try
            {
                CurrentUser = WorkstationSession.LogIn(SavedUsername, ConnectionToken);
                
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
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

        #region Session Facade
        

        public IEnumerable<ProjectModel> GetAllProjects()
        {
            var cached = Cache.Get("AllProjects");
            if (cached == null)
                Cache.Set("AllProjects", WorkstationSession.GetAllProjects(), CachePriority.Default);

            return ((ProjectModel[])Cache.Get("AllProjects")).ToList();
        }

        public IEnumerable<UsersModel> GetAllUsers()
        {
            var cached = Cache.Get("AllUsers");
            if (cached == null)
                Cache.Set("AllUsers", WorkstationSession.GetAllUsers(), CachePriority.Default);

            return ((UsersModel[])Cache.Get("AllUsers")).ToList();
        }

       // public IEnumerable<UsersModel> GetAllUsers();

        public IEnumerable<TeamModel> GetAllTeams()
        {
            var cached = Cache.Get("AllTeams");
            if (cached == null)
                Cache.Set("AllTeams", WorkstationSession.GetAllTeams(), CachePriority.Default);

            return ((TeamModel[])Cache.Get("AllTeams")).ToList();

        }

        public IEnumerable<DepartmentModel> GetAllDepartments() {
            var cached = Cache.Get("AllDepartments");
            if (cached == null)
                Cache.Set("AllDepartments", WorkstationSession.GetAllDepartments(), CachePriority.Default);
            return ((DepartmentModel[])Cache.Get("AllDepartments")).ToList();
        }

        public IEnumerable<RankModel> GetAllRanks(){
            var cached = Cache.Get("AllRanks");
            if (cached == null)
                Cache.Set("AllRanks", WorkstationSession.GetAllDepartments(), CachePriority.Default);
            return ((RankModel[])Cache.Get("AllRanks")).ToList();
        }

        public void EditProject(ProjectModel project){
            if (WorkstationSession.EditProject(project)){
                var MyProjects = GetAllProjects();
                MyProjects.ToList().Remove(MyProjects.Single(proj => proj.id == project.id));
                MyProjects.ToList().Add(project);
                Cache.Set("AllProjects", MyProjects, CachePriority.Default);
            }
        }

    

        public void UpdateNotification()
        {
            try
            {
                WorkstationSession.UpdateNotifications(CurrentUser.id, CurrentUser.username);
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
            }
        }

        public void UpdateMessages(int targetid)
        {
            try
            {
                WorkstationSession.UpdateDirectMessages(CurrentUser.id, targetid, CurrentUser.username);
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        #endregion

        #region Callback
        public void NotificationPull(NotificationModel[] notifications, string caller)
        {

            // GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients.User(CurrentUser.username)
            //    .NotificationPull(notifications, CurrentUser.username);

            var unread = notifications.Count(notif => !notif.read);
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>()
                .Clients.User(CurrentUser.username)
                .update(unread);

          
            UserSession["SystemNotifications"] = notifications;
            UserSession["UnreadNotifications"] = unread;
        }

        public void SendMessage(MessageModel model){
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>()
                .Clients.Users(new String[] {
                    CurrentUser.username,
                    WorkstationSession.GetAllUsers().Single(user => user.id == model.to).username
                })
                .message(model);
        }

        public void MessagePull(MessageModel[] messages, string caller){
            
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>()
                .Clients.Users(
                    WorkstationSession.GetAllUsers().Select(user => user.username).ToArray()
                )
                .directmsg(messages);
            
        }

        #endregion Callback
    }
}