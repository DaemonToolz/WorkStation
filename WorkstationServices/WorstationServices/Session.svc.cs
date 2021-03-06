﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Timers;
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
        private static readonly IdGenerator _IdGenerator;



        private ServicesManagementEntities entities;
        static Session(){
            OnlineUsers = new HashSet<UsersModel>();
            _IdGenerator = new IdGenerator(1);

            for (int i = 0; i < 3; ++i)
                _IdGenerator.GenerateId(10);
        }

        public UsersModel LogIn(string Username, string Token){
            entities = new ServicesManagementEntities();
            var CurrentUser = entities.Users.Single(user => user.username.Equals(Username));
            UsersModel CurrentUserModel = new UsersModel(){
                id = CurrentUser.id,
                email = CurrentUser.email,
                username = Username,
                team_id = CurrentUser.team_id,
                rank = CurrentUser.Rank1.name,
                rights = CurrentUser.Rank1.rights,
                profilepic = CurrentUser.profilepic
            };

            OnlineUsers.Add(CurrentUserModel);
            return CurrentUserModel;
        }

        public void LogOut(UsersModel user)
        {
            Unregister();
            OnlineUsers.Remove(user);
        }
        

        public IList<ProjectModel> GetAllProjects()
        {
            List<ProjectModel> projects = new List<ProjectModel>();

            foreach (var project in entities.Project)
                projects.Add(new ProjectModel(){
                    id = project.id,
                    name = project.name,
                    root = project.root,
                    projpic = project.projpic,
                    precedence = project.precedence,
                    admin_id = project.admin_id
                });

            return projects;
        }

        public ProjectModel CreateProject(ProjectModel model)
        {
            try
            {
                Project project = new Project
                {
                    name = model.name,
                    root = model.root,
                    projpic = model.projpic,
                    precedence = model.precedence,
                    admin_id = model.admin_id
                };
                entities.Project.Add(project);
                entities.SaveChanges();
                model.id = project.id;
                return model;
            }
            catch
            {
                return null;
            }
        }

        public IList<UsersModel> GetAllUsers(){
            List<UsersModel> users = new List<UsersModel>();

            foreach (var user in entities.Users)
                users.Add(new UsersModel()
                {
                     id = user.id,
                     email = user.email,
                     username = user.username,
                     team_id = user.team_id,
                     rank = user.Rank1.name,
                     rights = user.Rank1.rights,
                     profilepic = user.profilepic
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
                    name = team.name,
                    teampic = team.teampic,
                    manager_id = team.manager_id
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
                    project_id = team.project_id,
                    teampic = team.teampic,
                    manager_id = team.manager_id
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
                    project_id = team.project_id,
                    teampic = team.teampic,
                    manager_id = team.manager_id
                };
            }
            catch { return null; }
        }

        public bool EditTeam(TeamModel newInfo)
        {
            try
            {
                Team team = entities.Team.First(teamObj => teamObj.id == newInfo.id);
                team.name = newInfo.name;
                team.project_id = newInfo.project_id;
                team.department_id = newInfo.department_id;
                team.teampic = newInfo.teampic;

                team.manager_id = newInfo.manager_id;
                entities.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool DeleteTeam(TeamModel model){
            try
            {
  
                Team team = entities.Team.Single(rec => model.id == rec.id);
          

                entities.Team.Remove(team);
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            try
            {
                Team team = new Team
                {
                     department_id = model.department_id,
                     name = model.name,
                     project_id = model.project_id,
                     teampic = model.teampic,
                     manager_id = model.manager_id
                };

                entities.Team.Add(team);
                entities.SaveChanges();
                model.id = team.id;
                model.teampic = team.teampic;
                return model;
            }
            catch
            {
                return null;
            }
        }

        public bool EditProject(ProjectModel newInfo)
        {
            try
            {
                Project project = entities.Project.Single(proj => newInfo.id == proj.id);
                project.name = newInfo.name;
                project.root = newInfo.root;
                project.precedence = newInfo.precedence;
                project.projpic = newInfo.projpic;
                project.admin_id = newInfo.admin_id;
                entities.SaveChanges();

                return true;
            }
            catch{
                return false;
            }
        }

        public bool DeleteProject(ProjectModel newInfo)
        {
            try
            {
                Project project = entities.Project.Single(proj => newInfo.id == proj.id && newInfo.root.Equals(proj.root) && newInfo.name.Equals(proj.name));
                
                entities.Project.Remove(project);
                entities.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public ProjectModel GetProject(long id)
        {
            Project project = entities.Project.Single(record => record.id == id);
            return new ProjectModel()
            {
                id = id,
                name = project.name,
                root = project.root,
                projpic = project.projpic,
                precedence = project.precedence,
                admin_id = project.admin_id
            };
        }




        public bool EditUser(UsersModel newInfo) {
            try {
                Users user = entities.Users.Single(usr => newInfo.id == usr.id);
                user.email = newInfo.email;
                user.username = newInfo.username;
                user.rank = newInfo.rank;
                user.team_id = newInfo.team_id;

                if(newInfo.profilepic != user.profilepic)
                    user.profilepic = newInfo.profilepic;

                entities.SaveChanges();


                return true;
            } catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool DeleteUser(UsersModel currentUser)
        {
            try
            {
                Users user = entities.Users.Single(usr => currentUser.id == usr.id);
                entities.Users.Remove(user);
                entities.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public UsersModel GetUserId(int id) {
            try
            {
                var user = entities.Users.First(users => users.id == id);
                return new UsersModel()
                {
                    id = user.id,
                    email = user.email,
                    team_id = user.team_id,
                    username = user.username,
                    rank = user.rank,
                    rights = user.Rank1.rights,
                    profilepic = user.profilepic
                };
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<RankModel> GetAllRanks()
        {
            List<RankModel> rankModels = new List<RankModel>();
            foreach (var rank in entities.Rank) {
                rankModels.Add(new RankModel() {
                    name = rank.name,
                    rights = rank.rights
                });
            }

            return rankModels;
        }

        public RankModel GetRankByName(string name) {
            try
            {
                var rank = entities.Rank.First(ranks => ranks.name.Equals(name));
                return new RankModel()
                {
                    name = rank.name,
                    rights = rank.rights
                };
            }
            catch
            {
                return null;
            }
        }


        public IEnumerable<NotificationModel> GetAllNotifications(int userid) {
            try
            {
                List<NotificationModel> notificationModels = new List<NotificationModel>();

                foreach (var notification in entities.NotificationUser.Where(
                    notif => notif.Users.id.Equals(userid)))
                {
                    notificationModels.Add(new NotificationModel()
                    {
                        id = notification.id_notification,
                        content = notification.Notification.content,
                        title = notification.Notification.title,
                        read = notification.read,
                        stamp = notification.Notification.stamp
                    });

                }

                return notificationModels;
            }
            catch
            {
                return null;
            }
        }


        public void CreateNotification(NotificationModel notification, int[] users, bool all = false) {
            Notification finalNotification = new Notification(){
                content = notification.content,
                title = notification.title,      
                stamp = DateTime.Now
            };

            entities.Notification.Add(finalNotification);
            entities.SaveChanges();
            finalNotification = entities.Notification 
                .Single(notif => notif.content.Equals(finalNotification.content) &&
                               notif.title.Equals(finalNotification.title));
            
            foreach (var user in (all  ? entities.Users.ToList() : entities.Users.Where(user => users.Any(uids => uids == user.id)).ToList())) {
                NotificationUser notifUser = new NotificationUser() {
                    id_notification = finalNotification.id,
                    read = false,
                    id_user = user.id
                };

                entities.NotificationUser.Add(notifUser);
            }
            entities.SaveChanges();

        }


        public bool AcknowledgeNotification(NotificationModel original, int userid)
        {
            var notification =
                entities.NotificationUser.First(notif => original.id.Equals(notif.id_notification) &&
                                                         userid.Equals(notif.id_user));

            notification.read = original.read;
            entities.SaveChanges();

            return true;

        }
        public void DeleteNotification(long notificationid, int userid)
        {
            entities.NotificationUser.Remove(
                entities.NotificationUser.First(notif => notif.id_user == userid &&
                                                         notif.id_notification == notificationid));
            entities.SaveChanges();

            if (entities.NotificationUser.Count(notif => notificationid == notif.id_notification) == 0)
            {
                entities.Notification.Remove(entities.Notification.First(notif => notif.id == notificationid));
                entities.SaveChanges();
            }
        }

        public IEnumerable<TaskModel> GetAllTasks(long? project_id, int? user_id)
        {
            if (project_id == null && user_id == null)
                return new List<TaskModel>();

            IQueryable<Task> AllTasks;
            if (project_id == null || project_id <= 0)
            {
                AllTasks = entities.Task.Where(task => user_id == task.user_id);
            }
            else {
                AllTasks =
                    user_id == null || user_id <= 0
                        ? entities.Task.Where(task => project_id == task.project_id)
                        : entities.Task.Where(task => project_id == task.project_id && user_id == task.user_id);
            }

            List<TaskModel> TaskModels = new List<TaskModel>();
            foreach (var model in AllTasks)
                TaskModels.Add(new TaskModel()
                {
                    id = model.id,
                    begin = model.begin,
                    end = model.end,
                    description = model.description,
                    project_id = model.project_id,
                    user_id = model.user_id,
                    title = model.title,
                    precedence = model.precedence,
                    progress = model.progress
                });

            return TaskModels;
            
        }

        public TaskModel GetTaskId(long id)
        {
            var task = entities.Task.Single(record => record.id == id);
            return new TaskModel()
            {
                id = id,
                begin = task.begin,
                description = task.description,
                end = task.end,
                project_id = task.project_id,
                title = task.title,
                user_id = task.user_id,
                precedence = task.precedence,
                progress = task.progress
            };
        }


        public TaskModel CreateTask(TaskModel newTask)
        {
            try
            {
                Task task = new Task()
                {
                    title = newTask.title,
                    begin = newTask.begin,
                    description = newTask.description,
                    end = newTask.end,
                    project_id = newTask.project_id,
                    user_id = newTask.user_id,
                    precedence = newTask.precedence,
                    progress = newTask.progress
                };

                entities.Task.Add(task);
                entities.SaveChanges();

                newTask.id = task.id;

                return newTask;
            }
            catch
            {
                return null;
            }
        }


        public bool DeleteTask(TaskModel oldTask){
            try
            {
                entities.Task.Remove(entities.Task.Single(task => oldTask.id == task.id));
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditTask(TaskModel newTask)
        {
            try
            {
                Task task = entities.Task.Single(record => record.id == newTask.id);
                task.project_id = newTask.project_id;
                task.title = newTask.title;
                task.begin = newTask.begin;
                task.description = newTask.description;
                task.user_id = newTask.user_id;
                task.end = newTask.end;
                task.precedence = newTask.precedence;
                task.progress = newTask.progress;
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MessageModel> GetAllMessages(UsersModel caller, bool sended = false, bool received = true, bool direct_only = false, bool indirect_only = true){
            List<MessageModel> allMessageModels = new List<MessageModel>();
            foreach (var message in entities.Message.Where(
                record => 
                    ((sended && record.from == caller.id) || (received && record.to == caller.id)) 
                        && ((record.direct && direct_only) || (!record.direct && indirect_only))))
            {
                allMessageModels.Add(new MessageModel()
                {
                    id = message.id,
                    content = message.content,
                    from = message.from,
                    to = message.to,
                    read = message.read,
                    title = message.title,
                    direct = message.direct,
                    stamp = message.stamp
                });
            }

            return allMessageModels;
        }


        public  bool SendMessage(MessageModel model)
        {
            try
            {
                entities.Message.Add(new Message()
                {
                    content = model.content,
                    read = false,
                    title = model.title,
                    to = model.to,
                    from = model.from,
                    direct = model.direct,
                    stamp = DateTime.Now
                });
                entities.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteMessage(MessageModel caller) {
            try {
                entities.Message.Remove(entities.Message.First(message => message.id == caller.id));
                entities.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
        }

        public bool MarkAsRead(MessageModel caller)
        {
            var message = entities.Message.Single(record => record.id == caller.id);
            message.read = caller.read;
            entities.SaveChanges();
            return message.read;
        }


        public FileModel CreateFile(FileModel model)
        {
            try
            {
                File file = new File()
                {
                    name = model.name,
                    project_id = model.project_id,
                    owner_id = model.owner_id,
                    change_count = 0,
                    creation_date = DateTime.Now,
                    last_updater = model.owner_id,
                    last_update = DateTime.Now,
                    tracker_id = _IdGenerator.GenerateId(75)
                };

                
                entities.File.Add(file);
                entities.SaveChanges();

                model.change_count = 0;
                model.creation_date = file.creation_date;
                model.last_update = file.last_update;
                model.tracker_id = file.tracker_id;
                model.last_updater = file.last_updater;

                return model;
            }
            catch
            {
                return null;
            }
        }

        public bool UpdateFile(FileModel model){
            try
            {
                var RelatedFile = entities.File.Single(record => record.tracker_id.Equals(model.tracker_id));

                RelatedFile.name = model.name;
                RelatedFile.change_count++;
                RelatedFile.last_updater = model.last_updater;
                RelatedFile.last_update = model.last_update;
                RelatedFile.owner_id = model.owner_id;
                RelatedFile.project_id = model.project_id;

                entities.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFile(FileModel model)
        {
            try
            {
                entities.File.Remove(entities.File.Single(record => record.tracker_id.Equals(model.tracker_id)));
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FileModel GetFile(String trackerId){
            try
            {
                var File = entities.File.Single(record => record.tracker_id.Equals(trackerId));
                return new FileModel(){
                    tracker_id = trackerId,
                    change_count = File.change_count,
                    creation_date = File.creation_date,
                    last_update = File.last_update,
                    last_updater = File.last_updater,
                    name = File.name,
                    owner_id = File.owner_id,
                    project_id = File.project_id
                };

                
            }
            catch{
                return null;
            }
        }

        public IEnumerable<FileModel> GetFiles(long projectId)
        {
            try
            {
                var Files = entities.File.Where(record => record.project_id.Equals(projectId));
                List<FileModel> Models = new List<FileModel>();

                foreach (var File in Files)
                {
                    Models.Add(new FileModel()
                    {
                        tracker_id = File.tracker_id,
                        change_count = File.change_count,
                        creation_date = File.creation_date,
                        last_update = File.last_update,
                        last_updater = File.last_updater,
                        name = File.name,
                        owner_id = File.owner_id,
                        project_id = projectId
                    });
                }

                return Models;

            }
            catch
            {
                return null;
            }
        }

        public ChangeSetModel CreateChangeSet(ChangeSetModel model){
            try
            {
                var ChangeSet = new ChangeSet()
                {
                    id = Guid.NewGuid(),
                    shortName = model.shortName,
                    edition = model.edition,
                    addition = model.addition,
                    deletion = model.deletion,
                    description = model.description,
                    trackerId = model.trackerId,
                    parent = model.parent,
                    stamp = model.stamp,
                    origin = model.origin
                };

                entities.ChangeSet.Add(ChangeSet);
                entities.SaveChanges();
                model.id = ChangeSet.id;
                return model;
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteChangeSet(ChangeSetModel model)
        {
            try
            {
                entities.ChangeSet.Remove(entities.ChangeSet.Single(record => record.id.Equals(model.id)));
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ChangeSetModel GetChangeSet(Guid id){
            try
            {
                var ChangeSet = entities.ChangeSet.Single(rec => rec.id.Equals(id));
                return new ChangeSetModel(){
                    id = id,
                    addition = ChangeSet.addition,
                    deletion = ChangeSet.deletion,
                    description = ChangeSet.description,
                    edition = ChangeSet.edition,
                    shortName = ChangeSet.shortName,
                    trackerId = ChangeSet.trackerId,
                    parent = ChangeSet.parent,
                    origin = ChangeSet.origin,
                    stamp = ChangeSet.stamp
                };
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<ChangeSetModel> GetChangeSetsPerFile(String trackerId){
            try
            {
                var ChangeSets = entities.ChangeSet.Where(rec => rec.trackerId.Equals(trackerId));

                List<ChangeSetModel> models = new List<ChangeSetModel>();
                foreach(var changeSet in ChangeSets)
                    models.Add(new ChangeSetModel(){
                        id = changeSet.id,
                        addition = changeSet.addition,
                        deletion = changeSet.deletion,
                        description = changeSet.description,
                        edition = changeSet.edition,
                        shortName = changeSet.shortName,
                        trackerId = trackerId,
                        parent = changeSet.parent,
                        origin = changeSet.origin,
                        stamp = changeSet.stamp
                    });

                return models;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<ChangeSetModel> GetChangeSetsPerProject(long projectId){
            try
            {
                var ChangeSets = entities.ChangeSet.Where(rec => rec.File.project_id.Equals(projectId));

                List<ChangeSetModel> models = new List<ChangeSetModel>();
                foreach (var changeSet in ChangeSets)
                    models.Add(new ChangeSetModel(){
                        id = changeSet.id,
                        addition = changeSet.addition,
                        deletion = changeSet.deletion,
                        description = changeSet.description,
                        edition = changeSet.edition,
                        shortName = changeSet.shortName,
                        trackerId = changeSet.trackerId,
                        parent = changeSet.parent,
                        origin = changeSet.origin,
                        stamp = changeSet.stamp
                    });

                return models;
            }
            catch
            {
                return null;
            }
        }

        #region Callback

        private IUpdateNotificationCallback Callback = null;
        private System.Timers.Timer NotificationTimer = null;
        private System.Timers.Timer MessagenTimer = null;

        private int connected_id = -1;
        private string hubcaller = null;
        private int targetid = -1;

        void NotificationOnNotificationTimerElapsed(object sender, ElapsedEventArgs e){
            Callback.NotificationPull(GetAllNotifications(connected_id), hubcaller);
           
        }

        void MessageOnNotificationTimerElapsed(object sender, ElapsedEventArgs e)
        {
     
            Callback.MessagePull(
                GetAllMessages(
                    new UsersModel(){ id = connected_id }, true, true, true, false)
                    .Where(msg => msg.to == targetid || msg.from == targetid).ToArray(), hubcaller);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        public void UpdateNotifications(int userid, string hubcaller)
        {
            if (this.connected_id == -1)
                this.connected_id = userid;

            if (this.hubcaller == null)
                this.hubcaller = hubcaller;

            if(Callback == null)
                Callback = OperationContext.Current.GetCallbackChannel<IUpdateNotificationCallback>();

            if (NotificationTimer == null)
            {
                NotificationTimer = new System.Timers.Timer(5000);
                NotificationTimer.Elapsed += NotificationOnNotificationTimerElapsed;
                NotificationTimer.Enabled = true;
                NotificationTimer.Start();
            }
        }


        public void UpdateDirectMessages(int userid, int targetid,string caller) {
            if (this.connected_id == -1)
                this.connected_id = userid;

            if (this.hubcaller == null)
                hubcaller = caller;

            this.targetid = targetid;

            if (Callback == null)
                Callback = OperationContext.Current.GetCallbackChannel<IUpdateNotificationCallback>();

            if (MessagenTimer == null)
            {
                MessagenTimer = new System.Timers.Timer(2000);
                MessagenTimer.Elapsed += MessageOnNotificationTimerElapsed;
                MessagenTimer.Enabled = true;
                MessagenTimer.Start();
            }
        }

        public void Unregister()
        {
            NotificationTimer?.Stop();
            MessagenTimer?.Stop();
            NotificationTimer = null;
            MessagenTimer = null;
        }

    

        #endregion


    }


}
