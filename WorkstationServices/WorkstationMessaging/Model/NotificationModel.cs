using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model
{
    [DataContract]
    public class NotificationModel {
        [DataMember(IsRequired = false)]
        public long id { get; set; }
        [DataMember(IsRequired = true)]
        public string title { get; set; }
        [DataMember(IsRequired = true)]
        public string content { get; set; }

        [DataMember(IsRequired = true)]
        public bool read{ get; set; }

        [DataMember(IsRequired = true)]
        public DateTime stamp { get; set; }
    }
}
