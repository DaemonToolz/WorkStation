﻿using System;
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
    [ServiceContract(SessionMode = SessionMode.Required)]
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
        IList<UsersModel> GetAllUsers();

        [OperationContract]
        IList<DepartmentModel> GetAllDepartments();

        [OperationContract]
        IList<TeamModel> GetAllTeams();

        [OperationContract]
        TeamModel GetTeamPerUser(int userid);

        [OperationContract]
        TeamModel GetTeamPerId(int id);


    }
}
