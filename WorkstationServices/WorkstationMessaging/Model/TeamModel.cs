using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model{
    [DataContract]
    public class TeamModel {
        [DataMember(IsRequired = true)]
        public int id { get; set; }
        [DataMember(IsRequired = true)]
        public string name { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<int> department_id { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<long> project_id { get; set; }

    }
}
