using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model
{
    [DataContract]
    public class ChangeSetModel
    {
        [DataMember(IsRequired = true)]
        public System.Guid id { get; set; }

        [DataMember(IsRequired = true)]
        public string shortName { get; set; }

        [DataMember(IsRequired = true)]
        public string description { get; set; }

        [DataMember(IsRequired = true)]
        public int addition { get; set; }

        [DataMember(IsRequired = true)]
        public int deletion { get; set; }

        [DataMember(IsRequired = true)]
        public int edition { get; set; }

        [DataMember(IsRequired = true)]
        public string trackerId { get; set; }

        [DataMember(IsRequired = true)]
        public Nullable<Guid> parent{ get; set; }

        [DataMember(IsRequired = true)]
        public DateTime stamp { get; set; }

        [DataMember(IsRequired = true)]
        public int origin { get; set; }

    }
}
