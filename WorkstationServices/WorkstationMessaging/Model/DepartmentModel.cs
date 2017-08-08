using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WorkstationMessaging.Model{
    [DataContract]
    public class DepartmentModel{
        [DataMember(IsRequired = true)]
        public int id { get; set; }
        [DataMember(IsRequired = true)]
        public string name { get; set; }

    }
}
