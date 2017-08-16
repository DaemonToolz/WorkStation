using System;
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
                team_id = CurrentUser.team_id,
                 rank = CurrentUser.Rank1.name,
                 rights = CurrentUser.Rank1.rights
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
                     team_id = user.team_id,
                     rank = user.Rank1.name,
                     rights = user.Rank1.rights

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

        public bool EditProject(ProjectModel newInfo)
        {
            try
            {
                Project project = entities.Project.Single(proj => newInfo.id == proj.id);
                project.name = newInfo.name;
                project.root = newInfo.root;
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
                root = project.root
            };
        }

        public bool EditUser(UsersModel newInfo) {
            try {
                Users user = entities.Users.Single(usr => newInfo.id == usr.id);
                user.email = newInfo.email;
                user.username = newInfo.username;
                user.rank = newInfo.rank;
                user.team_id = newInfo.team_id;

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
                    rights = user.Rank1.rights
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
                        read = notification.read
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
                    title = model.title
                });

            return TaskModels;

        }


        public void CreateTask(TaskModel newTask)
        {
            Task task = new Task(){
                title = newTask.title,
                begin = newTask.begin,
                description = newTask.description,
                end = newTask.end,
                project_id = newTask.project_id,
                user_id = newTask.user_id
            };

            entities.Task.Add(task);
            entities.SaveChanges();
            
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

                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        #region Callback

        private IUpdateNotificationCallback Callback;
        private System.Timers.Timer Timer;
        private int connected_id;
        private string hubcaller;

        void OnTimerElapsed(object sender, ElapsedEventArgs e){
            Callback.NotificationPull(GetAllNotifications(connected_id), hubcaller);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileName"></param>
        public void UpdateNotifications(int userid, string hubcaller)
        {
            this.connected_id = userid;
            this.hubcaller = hubcaller;
            Callback = OperationContext.Current.GetCallbackChannel<IUpdateNotificationCallback>();

            Timer = new System.Timers.Timer(5000);
            Timer.Elapsed += OnTimerElapsed;
            Timer.Enabled = true;
            Timer.Start();
        }

        public void Unregister()
        {
            Timer.Stop();
            Timer = null;

        }

        #endregion


    }


}
