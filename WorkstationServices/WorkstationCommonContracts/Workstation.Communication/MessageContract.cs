using System;
using System.Runtime.Serialization;
namespace Contracts.Workstation.Communication{
    [DataContract]
    public class MessageContract { 
        
        [DataMember(IsRequired = true)]
        
        public String Message { get; set; }

        public String ToString(){
            return "{ \"Message\": \""+ Message +"\"}";
        }
    }
}
