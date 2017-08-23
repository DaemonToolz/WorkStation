using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Workstation.Model;
using WorkstationMessaging.Model;
using WorkstationServices.Security;

namespace WorkstationServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IUpdateNotificationCallback))]
    public interface ISession
    {
        [OperationContract(IsInitiating = true, IsOneWay = false, IsTerminating = false)]
        [FaultContract(typeof(InputValidationFaultContract))]
        [AttributeInspector]
        UsersModel LogIn(String Username, String Token);

        [OperationContract(IsInitiating = false, IsOneWay = true, IsTerminating = true)]
        void LogOut(UsersModel user);

        [OperationContract]
        IList<ProjectModel> GetAllProjects();

        [OperationContract]
        ProjectModel GetProject(long id);

        [OperationContract]
        bool EditProject(ProjectModel newInfo);

        [OperationContract]
        bool DeleteProject(ProjectModel newInfo);


        [OperationContract]
        IList<UsersModel> GetAllUsers();

        [OperationContract]
        bool EditUser(UsersModel newInfo);

        [OperationContract]
        bool DeleteUser(UsersModel user);

        [OperationContract]
        UsersModel GetUserId(int id);


        [OperationContract]
        IList<DepartmentModel> GetAllDepartments();

        [OperationContract]
        IList<TeamModel> GetAllTeams();

        [OperationContract]
        TeamModel GetTeamPerUser(int userid);

        [OperationContract]
        TeamModel GetTeamPerId(int id);


        [OperationContract]
        IEnumerable<RankModel>  GetAllRanks();

        [OperationContract]
        RankModel GetRankByName(string name);

        [OperationContract]
        bool AcknowledgeNotification(NotificationModel original, int userid);
      

        [OperationContract]
        IEnumerable<NotificationModel> GetAllNotifications(int userid);

        [OperationContract(IsOneWay = true)]
        void CreateNotification(NotificationModel notification, int[] users, bool all = false);

        [OperationContract(IsOneWay = true)]
        void UpdateNotifications(int userid, string caller);

        [OperationContract(IsOneWay = true)]
        void UpdateDirectMessages(int userid, int targetid, string caller);


        [OperationContract(IsOneWay = true)]
        void DeleteNotification(long notificationid, int userid);

        [OperationContract]
        IEnumerable<TaskModel> GetAllTasks(long? project_id, int? user_id);

        [OperationContract]
        TaskModel GetTaskId(long id);


        [OperationContract]
        void CreateTask (TaskModel newTask);

        [OperationContract]
        bool DeleteTask(TaskModel newTask);

        [OperationContract]
        bool EditTask(TaskModel newTask);

        [OperationContract]
        IEnumerable<MessageModel> GetAllMessages(UsersModel caller, bool sended , bool received, bool direct_only , bool indirect_only);

        [OperationContract]
        bool SendMessage(MessageModel caller);


        [OperationContract]
        bool DeleteMessage(MessageModel caller);

        [OperationContract]
        bool EditTeam(TeamModel newInfo);


    }

    public interface IUpdateNotificationCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotificationPull(IEnumerable<NotificationModel> notifications, String caller);

        [OperationContract(IsOneWay = true)]
        void MessagePull(IEnumerable<MessageModel> notifications, String caller);

    }
}
