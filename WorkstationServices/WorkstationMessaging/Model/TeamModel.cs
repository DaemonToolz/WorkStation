using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model{
    [DataContract]
    public class TeamModel : GenericModel{
      
        [DataMember(IsRequired = true)]
        public string name { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<int> department_id { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<long> project_id { get; set; }
        [DataMember(IsRequired = true)]
        public string teampic { get; set; }
        [DataMember(IsRequired = true)]
        public Nullable<int> manager_id { get; set; }

    }
}
