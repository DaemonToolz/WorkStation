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
        #region Callback
        public void NotificationPull(NotificationModel[] notifications, string caller)
        {

            // GlobalHost.ConnectionManager.GetHubContext<NotificationHub>().Clients.User(CurrentUser.username)
            //    .NotificationPull(notifications, CurrentUser.username);
            
            GlobalHost.ConnectionManager.GetHubContext<NotificationHub>()
                .Clients.User(CurrentUser.username)
                .update(notifications.Count(notif => notif.read == false));

          
            UserSession["SystemNotifications"] = notifications;
            UserSession["UnreadNotifications"] = notifications.Count(notif => !notif.read);
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