using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Workstation.Model;

namespace WorkstationUWP.Common{
    
    public class SessionWrapper {
       
        private LogInModel OriginalInput { get; set; }

        private String SavedUsername { get; set; }
        public UsersModel CurrentUser { get; private set; }


        private String ConnectionToken { get; set; }

        public bool NotificationPooler { get; set; }

 
        //private XmlCommentProvider CommentManager { get;set; }

        

        private String CacheQualifier { get; set; }
        private String CacheMsgQualifier { get; set; }
        private String CacheNotifQualifier { get; set; }

        private String CacheCommentManagerQualifier { get { return $"{CurrentUser.id}_cmtmger"; } }
        public SessionWrapper() {
        }

        public SessionWrapper(String Username, String Password, String Token, LogInModel model, bool Autolog = false)
        {
            ConnectionToken = Token;
            SavedUsername = Username;
            OriginalInput = model;
              
            //if (Autolog)
            //    CurrentUser = WorkstationSession.LogIn(SavedUsername, ConnectionToken);
        }

        public async System.Threading.Tasks.Task<bool> LogInAsync(){
            try
            {
                var jsonUser = await RequestHandler.Call("http://localhost:10856/Mobile.svc/Mobile/Login","POST", null, null, 
                    new {Username=SavedUsername, Token = ConnectionToken});

                var userObj = JsonConvert.DeserializeObject<dynamic>(jsonUser);
                jsonUser = JsonConvert.SerializeObject(userObj.LogInResult);
                CurrentUser = JsonConvert.DeserializeObject<UsersModel>(jsonUser);
                
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

       
     
    }
}