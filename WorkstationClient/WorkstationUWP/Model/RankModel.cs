using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model{
    [DataContract]
    public class RankModel {
        [DataMember(IsRequired = true)]
        public String name { get; set; }
        [DataMember(IsRequired = true)]
        public String rights { get; set; }
    }
}
