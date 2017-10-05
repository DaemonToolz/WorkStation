using System;
using System.Runtime.Serialization;
using WorkstationMessaging.Model;

namespace Workstation.Model{
    [DataContract]
    public class ProjectModel : GenericModel{
        [DataMember(IsRequired = true)]
        public string name { get; set; }
        [DataMember(IsRequired = true)]
        public string root { get; set; }
        [DataMember(IsRequired = true)]
        public string projpic { get; set; }
        [DataMember(IsRequired = true)]

        public short precedence { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<int> admin_id { get; set; }
    }
}