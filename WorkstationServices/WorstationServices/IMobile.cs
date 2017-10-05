using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;
using Workstation.Model;
using WorkstationMessaging.Model;
using WorkstationServices.Security;

namespace WorkstationServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMobile" in both code and config file together.
    [ServiceContract]
    public interface IMobile
    {
        [OperationContract]
        [FaultContract(typeof(InputValidationFaultContract))]
        [AttributeInspector]
        [WebInvoke(Method ="POST" ,UriTemplate = "Mobile/Login", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UsersModel LogIn(String Username, String Token);

        
    }
}
