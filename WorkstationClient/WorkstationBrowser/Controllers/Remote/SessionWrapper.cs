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

        private String CacheQualifier { get; set; }
        private String CacheMsgQualifier { get; set; }
        private String CacheNotifQualifier { get; set; }

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
                CacheQualifier = $"usr{CurrentUser.id}";
                CacheMsgQualifier = $"{CacheQualifier}_msg";
                CacheNotifQualifier = $"{CacheQualifier}_ntf";

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

        public ProjectModel GetProject(long id)
        {
            return GetAllProjects().Single(project => project.id == id);
        }

        public IEnumerable<UsersModel> GetAllUsers()
        {
            var cached = Cache.Get("AllUsers");
            if (cached == null)
                Cache.Set("AllUsers", WorkstationSession.GetAllUsers(), CachePriority.Default);
            
            return ((UsersModel[])Cache.Get("AllUsers")).ToList();
        }

        public bool EditUser(UsersModel user)
        {
            if (!WorkstationSession.EditUser(user)) return false;

            var AllUsers = GetAllUsers();

            var UserModels = AllUsers as IList<UsersModel> ?? AllUsers.ToList();
            UserModels.Remove(UserModels.Single(usr => usr.id == user.id));
            UserModels.Add(user);

            Cache.Set("AllUsers", UserModels.ToArray(), CachePriority.Default);
            return true;
        }

        public IEnumerable<UsersModel> GetUsersByTeam(long teamid)
        {
            return GetAllUsers().Where(user => user.team_id == teamid);
        }

        public UsersModel GetUserById(int id){
            return GetAllUsers().Single(user => user.id == id);
        }

        public IEnumerable<TeamModel> GetAllTeams()
        {
            var cached = Cache.Get("AllTeams");
            if (cached == null)
                Cache.Set("AllTeams", WorkstationSession.GetAllTeams(), CachePriority.Default);

            return ((TeamModel[])Cache.Get("AllTeams")).ToList();

        }

        public TeamModel GetTeamById(int id){
            return GetAllTeams().Single(team => team.id == id);
        }

        public void EditTeam(TeamModel team)
        {
            if (!WorkstationSession.EditTeam(team)) return;

            var AllTeams = GetAllTeams();

            var TeamModels = AllTeams as IList<TeamModel> ?? AllTeams.ToList();
            TeamModels.Remove(TeamModels.Single(model => model.id == team.id));
            TeamModels.Add(team);

            Cache.Set("AllTeams", TeamModels.ToArray(), CachePriority.Default);
        }


        public TeamModel GetTeamByUser(UsersModel user)
        {
            return GetAllTeams().Single(team => team.id == user.team_id);
        }

        public IEnumerable<DepartmentModel> GetAllDepartments() {
            var cached = Cache.Get("AllDepartments");
            if (cached == null)
                Cache.Set("AllDepartments", WorkstationSession.GetAllDepartments(), CachePriority.Default);
            return ((DepartmentModel[])Cache.Get("AllDepartments")).ToList();
        }

        public DepartmentModel GetDepartments(String name)
        {
            return GetAllDepartments().Single(dept => dept.name.Equals(name));
        }

        public IEnumerable<RankModel> GetAllRanks(){
            var cached = Cache.Get("AllRanks");
            if (cached == null)
                Cache.Set("AllRanks", WorkstationSession.GetAllRanks(), CachePriority.Default);
            return ((RankModel[])Cache.Get("AllRanks")).ToList();
        }

        public RankModel GetRankByName(String name) {
            var AllRanks = GetAllRanks();
            return AllRanks.Single(rank => rank.name.Equals(name));
        }


        public IEnumerable<TaskModel> GetTasks(int project_id)
        {
            String CacheKey = (project_id) + "_tasks_0";
            var cached = Cache.Get(CacheKey);
            if (cached == null)
                Cache.Set(CacheKey, WorkstationSession.GetAllTasks(project_id, null), CachePriority.Default);
            return ((TaskModel[])Cache.Get(CacheKey)).ToList();
        }

        public TaskModel GetTaskById(long id, long projectid){
            var AllTasks = GetTasks((int)projectid);
            return AllTasks.Single(task => id == task.id);
        }

        public IEnumerable<TaskModel> GetTasksByUser(int user_id){
            String CacheKey =  "0_tasks_" + user_id;
            var cached = Cache.Get(CacheKey);
            if (cached == null)
                Cache.Set(CacheKey, WorkstationSession.GetAllTasks(null, user_id), CachePriority.Default);
            return ((TaskModel[])Cache.Get(CacheKey)).ToList();
        }



        public bool CreateTask(TaskModel model){
            if (!WorkstationSession.CreateTask(model)) return false;
            Cache.Remove((model.project_id) + "_tasks_0");
            GetTasks((int)model.project_id);
            if (model.user_id != null){
                Cache.Remove("0_tasks_" + ((int)model.user_id));
                GetTasksByUser((int)model.user_id);
            }

            return true;
        }

        public bool EditTask(TaskModel model){
            if (!WorkstationSession.EditTask(model)) return false;

            var AllTasks = GetTasks((int)model.project_id);

            TaskModel old;
            var TasksModel = AllTasks.ToList();
            TasksModel.Remove(old = TasksModel.Single(task => task.id == model.id));
            TasksModel.Add(model);

            Cache.Set((model.project_id) + "_tasks_0", TasksModel.ToArray(), CachePriority.Default);

            if (model.user_id != null) {
                if (old.user_id != null){
                    AllTasks  = GetTasksByUser((int) old.user_id);
                    TasksModel = AllTasks.ToList();
                    if (TasksModel.Any(task => task.id == old.id))
                    {
                        TasksModel.Remove(TasksModel.Single(task => task.id == old.id));
                        Cache.Set("0_tasks_" + ((int) old.user_id), TasksModel.ToArray(), CachePriority.Default);
                    }
                }

                AllTasks = GetTasksByUser((int)model.user_id);
                TasksModel = AllTasks.ToList();
                if (TasksModel.All(task => task.id != model.id))
                {
                    TasksModel.Add(model);
                    Cache.Set("0_tasks_" + ((int) model.user_id), TasksModel.ToArray(), CachePriority.Default);
                }
            }

            return true;
        }

        public bool DeleteTask(TaskModel model){
           
            if (!WorkstationSession.DeleteTask(model)) return false;
            var AllTasks = GetTasks((int)model.project_id);

            var TasksModel =  AllTasks.ToList();
            TasksModel.Remove(TasksModel.Single(task => task.id == model.id));
     
            Cache.Set((model.project_id) + "_tasks_0", TasksModel.ToArray(), CachePriority.Default);

            if (model.user_id != null)
            {
                AllTasks = GetTasksByUser((int)model.user_id);
                TasksModel = AllTasks.ToList();
                TasksModel.Remove(model);
                Cache.Set("0_tasks_" + ((int)model.user_id), TasksModel.ToArray(), CachePriority.Default);
            }

            return true;
        }

        public void EditProject(ProjectModel project){
            if (!WorkstationSession.EditProject(project)) return;
            var MyProjects = GetAllProjects();

            var projectModels = MyProjects as IList<ProjectModel> ?? MyProjects.ToList();
            projectModels.Remove(projectModels.Single(proj => proj.id == project.id));
            projectModels.Add(project);

            Cache.Set("AllProjects", projectModels.ToArray(), CachePriority.Default);
        }



        public IEnumerable<MessageModel> MyMessages(bool sended = false, bool received = true, bool direct = false, bool mailbox = true) {
            
            return WorkstationSession.GetAllMessages(CurrentUser, sended, received, direct, mailbox);
        }

        private void MarkAsRead(MessageModel message) {
            if (WorkstationSession.MarkAsRead(message)) return;
            message.read = false;
            
        }

        public void MarkAsRead(params MessageModel[] messages){
            foreach (var message in messages) {
                if (message.read) continue;
                message.read = true;
                MarkAsRead(message);
            }
            /*
            var AllMessages = MyMessages();
            var MessageModels = AllMessages as IList<MessageModel> ?? AllMessages.ToList();
            foreach (var message in messages)
            {
                MessageModels.Remove(MessageModels.Single(msg => msg.id == message.id));
                MessageModels.Add(message);
            }
            */
        }
        

        public void DeleteMessage(params MessageModel[] models) {
            foreach (var message in models)
                WorkstationSession.DeleteMessage(message);
        }

        public bool SendMessage(MessageModel model){
            return WorkstationSession.SendMessage(model);
        }

        public void AcknowledgeNotification(NotificationModel model)
        {
            model.read = true;
            WorkstationSession.AcknowledgeNotification(model, CurrentUser.id);
        }

        public void DeleteNotification(NotificationModel model){
            WorkstationSession.DeleteNotification(model.id, CurrentUser.id);
        }

        public IEnumerable<NotificationModel> GetNotifications()
        {
            return WorkstationSession.GetAllNotifications(CurrentUser.id);
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

        public void CreateNotification(String title, String content, bool all, params int[] targets) {
            WorkstationSession.CreateNotification(new NotificationModel() {
                title = title, content = content
            }, targets, all);
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

        public void SendDirectMessage(MessageModel model){
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