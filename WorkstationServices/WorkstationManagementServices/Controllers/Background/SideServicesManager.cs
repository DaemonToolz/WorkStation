using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace WorkstationManagementServices.Controllers.Background {
    public static class SideServicesManager {
        private static readonly List<String> ConnectedUsers = new List<string>();

        private static readonly Dictionary<String, Process> SideServices = new Dictionary<string, Process>();
        public static Dictionary<String, SideService> AccessibleServices = new Dictionary<string, SideService>();

        internal static readonly String ScriptsFile =
                @"C:\Users\macie\Source\Repos\WorkStation\WorkstationServices\WorkstationManagementServices\Controllers\Background\Data\Scripts.xml"
            ;

        public static Dictionary<String,Dictionary<String,String>> GetSideServicesInfo()
        {
            var Temporary = new Dictionary<String, Dictionary<String, String>>();
            foreach (var pair in SideServices)
            {
                Temporary.Add(pair.Key, new Dictionary<string, string>()
                {
                    { "Name", pair.Value.ProcessName},
                    { "Responding", pair.Value.Responding.ToString() },
                    { "Exited", pair.Value.HasExited.ToString() },
                });
            }

            return Temporary;
        }


        static SideServicesManager(){
            AppDomain.CurrentDomain.ProcessExit += CloseSideServices;


            var Document = XDocument.Load(ScriptsFile);
            var MyScripts = Document.Root.Elements("script");
            AccessibleServices = new Dictionary<string, SideService>();

            foreach (var node in MyScripts){        
                try{
                    var name = node.Attribute("name").Value;

                    var p =  new Process {
                            StartInfo = {
                                LoadUserProfile = true,
                                FileName = $@"{node.Element("path").Value.Trim()}/{node.Element("exe").Value.Trim()}"
                            }
                        };
                    p.Start();
                    SideServices.Add(name, p);

                    var services = node.Descendants("services").Elements("service");

                  
                    foreach (var service in services)
                    {
                        var Descriptors = new List<ServiceDescriptor>();
                        foreach (var access in service.Descendants("access"))
                        {
                            var inputsStr = access.Attribute("input").Value;
                            var Inputs = new Dictionary<String, Type>();
                            if (inputsStr.Contains(";"))
                            {
                                Inputs = inputsStr.Split(';').Select(input => input.Split(':'))
                                    .ToDictionary(sub => sub[0], sub => Type.GetType(sub[1]));
                            }
                            else
                            {
                                var temp = inputsStr.Split(':');
                                Inputs.Add(temp[0], Type.GetType(temp[1]));
                            }

                            Descriptors.Add(new ServiceDescriptor()
                            {
                                Input = Inputs,
                                Access = access.Attribute("type").Value,
                                ResponseType = access.Attribute("response").Value,
                                ServiceName = access.Attribute("name").Value,
                                Path = access.Value
                            });
                        }
                        /*
                        var Descriptors = (from access in service.Descendants("access")
                            let inputsStr = access.Attribute("input").Value
                            let Inputs = inputsStr.Split(';').Select(input => input.Split(':')).ToDictionary(sub => sub[0], sub => Type.GetType(sub[1]))
                            select new ServiceDescriptor()
                            {
                                Input = Inputs,
                                Access = access.Attribute("type").Value,
                                ResponseType = access.Attribute("response").Value,
                                ServiceName = access.Attribute("name").Value,
                                Path = access.Value
                            }).ToList();
                            */

                        AccessibleServices.Add(service.Attribute("name").Value, new SideService()
                        {
                            AppName = name,
                            FriendlyName = service.Attribute("name").Value,
                            Port = int.Parse(service.Attribute("port").Value),
                            Accesses = Descriptors,
                            Base = service.Attribute("base").Value
                        });
                    }
                }
                catch
                {
                    //throw;
                    // Meaning some services won't be available
                }
            }

        }

        public static void RegisterUser(String self){
            if(!ConnectedUsers.Contains(self))
                ConnectedUsers.Add(self);
        }

        public static void Unregister(String self){
            if (ConnectedUsers.Contains(self))
                ConnectedUsers.Remove(self);
        }

        public static String CallService(String appname, String serviceName, String access, String accessName, Dictionary<String, Object> inputs)
        {
            if (!AccessibleServices.ContainsKey(serviceName) && !AccessibleServices.Values.Any(rec => rec.AppName.Equals(appname))) 
                throw new InvalidOperationException("Application not registered");

            return AccessibleServices.Single(rec => rec.Key.Equals(serviceName)).Value
                .CallService(access, accessName, inputs);
        }


        private static void CloseSideServices(object sender, EventArgs e){
            foreach (var process in SideServices.Values)
                process.Kill();
        }
    }
}