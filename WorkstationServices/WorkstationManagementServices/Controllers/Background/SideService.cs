using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace WorkstationManagementServices.Controllers.Background {
    public class SideService {
        internal String AppName { get; set; }
        internal String FriendlyName { get; set; }
        internal int Port { get; set; }
        
        internal List<ServiceDescriptor> Accesses { get; set; }
        internal String Base { get; set; }

        public SideService() {

        }


        public String CallService(String access, String serviceName, Dictionary<String, Object> inputs)
        {
            if (!Accesses.Any(rec => rec.Access.Equals(access) && rec.ServiceName.Equals(serviceName)))
                throw new InvalidOperationException("This service is not supported");

            var AskedService = Accesses.Single(rec => rec.ServiceName.Equals(serviceName));

            if (AskedService.Input.All(input => inputs.Any(rec => rec.Key.Equals(input.Key) && rec.Value.Equals(input.Value.GetType()))))
                throw new InvalidOperationException("Could not parse the provided arguments");
            
            return WebRequest(AskedService, inputs);
        }

        private string  WebRequest(ServiceDescriptor selected, Dictionary<String, Object> inputs){
            //try
           // {
                var path = selected.Path;
                foreach (var input in inputs)
                    path = selected.Path.Replace("{"+input.Key+"}", (Convert.ChangeType(input.Value, selected.Input[input.Key])).ToString());
               
                var webRequest = (HttpWebRequest)System.Net.WebRequest.Create($@"{Base}:{Port}{path}");
                webRequest.Method = selected.Access;
                webRequest.ContentType = selected.ResponseType;
                webRequest.UserAgent = "Workstation Probing Agent";
                webRequest.Headers.Add("Token", "Jkd855c6x9Aqcf");
                using (HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse()){
                    using (var s = response.GetResponseStream()){
                        using (var sr = new System.IO.StreamReader(s)){
                            return sr.ReadToEnd();
                        }
                    }
                }
            //}
            //catch {
            //    return "{}";
            //}


        }
    }

    public class ServiceDescriptor{
        internal String ServiceName { get; set; }
        internal String Access { get; set; }
        internal Dictionary<String,Type> Input { get; set; }
        internal String ResponseType { get; set; }

        internal String Path { get; set; }
    }
}