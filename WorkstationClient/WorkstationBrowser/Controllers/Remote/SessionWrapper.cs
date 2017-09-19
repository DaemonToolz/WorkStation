using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml.Serialization;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using WorkstationBrowser.BLL.FileTracker;
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

        private LogInModel OriginalInput { get; set; }

        private String SavedUsername { get; set; }
        public UsersModel CurrentUser { get; private set; }


        private String ConnectionToken { get; set; }

        public bool NotificationPooler { get; set; }
        public NotificationModel[] MyNotifications { get; private set; }

        private HttpSessionStateBase UserSession { get; set; }
        private CacheProvider Cache { get; set; }
        
        private XmlCommentProvider CommentManager {
            get => (XmlCommentProvider)Cache.Get(CacheCommentManagerQualifier);
            set {
                
                Cache.Set(CacheCommentManagerQualifier, value, CachePriority.Default, (arg) =>{
                    ((XmlCommentProvider) arg.CacheItem.Value).Dispose();
                }, 30.00);
            }
        }
        
        //private XmlCommentProvider CommentManager { get;set; }

        private bool DisposeOld(XmlCommentProvider @new){
            if (Cache.HasKey(CacheCommentManagerQualifier) && CommentManager != null)
                return !CommentManager.Equals(@new);
            return false;
        }

        private String CacheQualifier { get; set; }
        private String CacheMsgQualifier { get; set; }
        private String CacheNotifQualifier { get; set; }

        private String CacheCommentManagerQualifier { get { return $"{CurrentUser.id}_cmtmger"; } }
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

                DisposeCommentManager(); // Force Close file if it has not be done already
                WorkstationSession.LogOut(CurrentUser);
                return true;
            }
            catch
            {
                return false;
            }
        }


        #region FileTracker

        public void OpenFile(String path, String tracked, bool KeepOpen = false){
            var temp = new XmlCommentProvider(path, tracked, CurrentUser);
            if(KeepOpen) temp.OpenFile();
            CommentManager = temp;
        }

        public void AddComment(CommentModel comment)
        {
            var temp = CommentManager;
            temp.AddNode(comment);
            CommentManager = temp;
        }


        public void UpdateComment(CommentModel comment)
        {
            var temp = CommentManager;
            temp.UpdateComment(comment);
            CommentManager = temp;
        }



        public void DeleteComment(String id)
        {
            var temp = CommentManager;
            temp.DeleteComment(id);
            CommentManager = temp;
        }

        public IEnumerable<UsersModel> CommentActiveUsers()
        {
            return CommentManager.ActiveUsers();
        }

        public IEnumerable<CommentModel> ReadComments()
        {
            return CommentManager.ReadComments();
        }

        public void CloseFile()
        {
            if(Cache.HasKey(CacheCommentManagerQualifier))
                CommentManager.CloseFile();
        }

        public void DisposeCommentManager()
        {
            Cache.Remove(CacheCommentManagerQualifier);
        }
        #endregion


        #region Session Facade


        public IEnumerable<ProjectModel> GetAllProjects()
        {
            return Cache.GetAll("AllProjects", WorkstationSession.GetAllProjects);
        }

        public ProjectModel GetProject(long id)
        {
            return GetAllProjects().Single(project => project.id == id);
        }

        public IEnumerable<UsersModel> GetAllUsers()
        {
            return Cache.GetAll("AllUsers", WorkstationSession.GetAllUsers);
        }

        public bool EditUser(UsersModel user)
        {
            return Cache.Edit("AllUsers", GetAllUsers, (UsersModel model) => WorkstationSession.EditUser(model), user);
        }

        public IEnumerable<UsersModel> GetUsersByTeam(long teamid)
        {
            return GetAllUsers().Where(user => user.team_id == teamid);
        }

        public UsersModel GetUserById(int id){
            return GetAllUsers().Single(user => user.id == id);
        }

        public UsersModel GetUserByName(String name)
        {
            return GetAllUsers().Single(user => user.username.Equals(name));
        }


        public IEnumerable<TeamModel> GetAllTeams(){
            return Cache.GetAll("AllTeams", WorkstationSession.GetAllTeams);
        }

        public TeamModel GetTeamById(int id){
            return GetAllTeams().Single(team => team.id == id);
        }

        public bool EditTeam(TeamModel team)
        {
            return Cache.Edit("AllTeams", GetAllTeams, (TeamModel model) => WorkstationSession.EditTeam(model), team);
        }

        public bool DeleteTeam(int id)
        {
            return Cache.Delete("AllTeams", WorkstationSession.DeleteTeam, GetAllTeams, GetTeamById(id));
        }

        public bool DeleteProject(long id)
        {
            return Cache.Delete("AllProjects", WorkstationSession.DeleteProject, GetAllProjects, GetProject(id));
        }

        public bool CreateTeam(TeamModel model)
        {
            return Cache.Add<TeamModel>("AllTeams", WorkstationSession.CreateTeam, GetAllTeams, model);
        }

        public bool CreateProject(ProjectModel model)
        {
            return Cache.Add<ProjectModel>("AllProjects", WorkstationSession.CreateProject, GetAllProjects, model);
        }

        public TeamModel GetTeamByUser(UsersModel user)
        {
            return GetAllTeams().Single(team => team.id == user.team_id);
        }

        public IEnumerable<DepartmentModel> GetAllDepartments() {
            return Cache.GetAll("AllDepartments", WorkstationSession.GetAllDepartments);
        }

        public DepartmentModel GetDepartments(String name)
        {
            return GetAllDepartments().Single(dept => dept.name.Equals(name));
        }

        public IEnumerable<RankModel> GetAllRanks()
        {
            return Cache.GetAll("AllRanks", WorkstationSession.GetAllRanks);
        }

        public RankModel GetRankByName(String name) {
            return GetAllRanks().Single(rank => rank.name.Equals(name));
        }

        public IEnumerable<UsersModel> GetUsersByProject(long projectId)
        {
            var allTeams = GetAllTeams().Where(team => team.id == projectId);

            // REFACTORING HERE
            List<UsersModel> allUsers = new List<UsersModel>();
            foreach(var team in allTeams)
                allUsers.AddRange(GetUsersByTeam(team.id));

            return allUsers;
        }

        public IEnumerable<TaskModel> GetTasks(int project_id)
        {
            return Cache.GetAll((project_id) + "_tasks_0", () => WorkstationSession.GetAllTasks(project_id, null));
        }

        public TaskModel GetTaskById(long id, long projectid){
            return GetTasks((int)projectid).Single(task => id == task.id);
        }

        public IEnumerable<TaskModel> GetTasksByUser(int user_id){
            return Cache.GetAll("0_tasks_" + user_id, () => WorkstationSession.GetAllTasks(null, user_id));
        }

        public IEnumerable<ChangeSetModel> GetAllChangeSets(String filename)
        {
            return Cache.GetAll($"f{filename.GetHashCode()}", () => WorkstationSession.GetChangeSetsPerFile(filename));
        }

        public void CreateChangeSet(ChangeSetModel model)
        {
            WorkstationSession.CreateChangeSet(model);
        }

        public IEnumerable<ChangeSetModel> GetChangeSetPerProject(long projectId)
        {
            return WorkstationSession.GetChangeSetsPerProject(projectId);
        }

        public IEnumerable<ChangeSetModel> GetChangeSetPerFile(String trackerid)
        {
            return WorkstationSession.GetChangeSetsPerFile(trackerid);
        }

        public IEnumerable<FileModel> GetFiles(int ProjectId)
        {
            return Cache.GetAll($"SPF_{ProjectId}", () => WorkstationSession.GetFiles(ProjectId));
        }

        public bool CreateFile(FileModel model){
            return Cache.Add<FileModel>($"SPF_{model.project_id}", WorkstationSession.CreateFile, GetFiles, model, (int)model.project_id);
        }

        //public bool DeleteFile(FileModel model)
        //{
        //    return Cache.Delete<FileModel>($"SPF_{model.project_id}", WorkstationSession.DeleteFile, model);
        //}


        //public bool UpdateFile(FileModel model)
        //{
        //return Cache.Edit<FileModel>($"SPF_{model.project_id}", WorkstationSession.CreateFile, GetFiles, model, (int)model.project_id);
        //}



        public bool CreateTask(TaskModel model)
        {/*
            var createResult = WorkstationSession.CreateTask(model);
            if (createResult == null) return false;

            var tasks = GetTasks((int)model.project_id);// Cache.Remove((model.project_id) + "_tasks_0");
            var tasksList = tasks.ToList();
            tasksList.Add(createResult);
            

            GetTasks((int)model.project_id);
            if (model.user_id != null){
                Cache.Remove("0_tasks_" + ((int)model.user_id));
                GetTasksByUser((int)model.user_id);
            }
            */
            return Cache.CrossAdd<TaskModel>(
                (model.project_id) + "_tasks_0", 
                "0_tasks_" + ((int)model.user_id), 
                WorkstationSession.CreateTask, GetTasks, GetTasksByUser, model,
                (int)model.project_id, (int)model.user_id, (model.user_id != null));
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

        public bool EditProject(ProjectModel project){
            return Cache.Edit("AllProjects", GetAllProjects, (ProjectModel model) => WorkstationSession.EditProject(model), project);
        }

        public IEnumerable<MessageModel> MyMessages(bool sended = false, bool received = true, bool direct = false, bool mailbox = true) {
            
            return WorkstationSession.GetAllMessages(CurrentUser, sended, received, direct, mailbox).OrderByDescending(notif => notif.stamp).ToArray();
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
            WorkstationSession.AcknowledgeNotification(model, (int)CurrentUser.id);
        }

        public void DeleteNotification(NotificationModel model){
            WorkstationSession.DeleteNotification(model.id, (int)CurrentUser.id);
        }

        public IEnumerable<NotificationModel> GetNotifications()
        {
            return WorkstationSession.GetAllNotifications((int)CurrentUser.id);
        }

        public void UpdateNotification()
        {
            try
            {
                WorkstationSession.UpdateNotifications((int)CurrentUser.id, CurrentUser.username);
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
                WorkstationSession.UpdateDirectMessages((int)CurrentUser.id, targetid, CurrentUser.username);
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