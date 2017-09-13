using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model
{
    [DataContract]
    public class FileModel{
        [DataMember(IsRequired = true)]
        public string tracker_id { get; set; }

        [DataMember(IsRequired = true)]
        public string name { get; set; }

        [DataMember(IsRequired = true)]
        public int owner_id { get; set; }

        [DataMember(IsRequired = true)]
        public int last_updater { get; set; }

        [DataMember(IsRequired = true)]
        public System.DateTime creation_date { get; set; }

        [DataMember(IsRequired = true)]
        public System.DateTime last_update { get; set; }

        [DataMember(IsRequired = true)]
        public int change_count { get; set; }

        [DataMember(IsRequired = true)]
        public long project_id { get; set; }

    }
}
