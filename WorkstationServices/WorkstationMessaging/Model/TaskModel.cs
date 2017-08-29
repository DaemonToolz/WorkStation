using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model
{
    [DataContract]
    public class TaskModel : GenericModel {
        [DataMember(IsRequired = true)]
        public string title { get; set; }
        [DataMember(IsRequired = false)]
        public string description { get; set; }
        [DataMember(IsRequired = true)]
        public System.DateTime begin { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<System.DateTime> end { get; set; }
        [DataMember(IsRequired = false)]
        public Nullable<int> user_id { get; set; }
        [DataMember(IsRequired = true)]
        public long project_id { get; set; }
    }
}
