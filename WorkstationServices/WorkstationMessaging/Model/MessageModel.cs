using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model {
    [DataContract]
    public class MessageModel {
        [DataMember(IsRequired = false)]
        public long id { get; set; }

        [DataMember(IsRequired = true)]
        public int from { get; set; }

        [DataMember(IsRequired = true)]
        public int to { get; set; }

        [DataMember(IsRequired = false)]
        public string title { get; set; }

        [DataMember(IsRequired = true)]
        public string content { get; set; }

        [DataMember(IsRequired = false)]
        public bool read { get; set; }

        [DataMember(IsRequired = false)]
        public bool direct { get; set; }

    }
}
