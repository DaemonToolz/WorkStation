using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Workstation.Model;

namespace WorkstationMessaging.Model {
    [DataContract, KnownType(typeof(ProjectModel)), KnownType(typeof(TaskModel)), KnownType(typeof(TeamModel)), KnownType(typeof(UsersModel))]
    public class GenericModel {
        [DataMember(IsRequired = true)]
        public long id { get; set; }
    }
}
