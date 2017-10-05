using System;
using System.Runtime.Serialization;
using WorkstationMessaging.Model;

namespace Workstation.Model{
    [DataContract]
    public class UsersModel : GenericModel{

        [DataMember(IsRequired = true)]
        public string username { get; set; }

        [DataMember(IsRequired = true)]
        public string email { get; set; }

        [DataMember(IsRequired = true)]
        public Nullable<int> team_id { get; set; }

        [DataMember(IsRequired = true)]
        public string rank { get; set; }

        [DataMember(IsRequired = true)]
        public string rights{ get; set; }

        [DataMember(IsRequired = true)]
        public string profilepic { get; set; }


    }
}