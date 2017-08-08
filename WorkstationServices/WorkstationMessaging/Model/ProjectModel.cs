using System;
using System.Runtime.Serialization;

namespace Workstation.Model{
    [DataContract]
    public class ProjectModel {
        [DataMember(IsRequired = true)]
        public long id { get; set; }
        [DataMember(IsRequired = true)]
        public string name { get; set; }
        [DataMember(IsRequired = true)]
        public string root { get; set; }
    }
}