using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers.SignalR {
    public class NotificationHub : Hub {
        public void NotificationPull(NotificationModel[] notifications, string userid)
        {
            Clients.User(userid).update(notifications.Count(notif => notif.read == false));
        }

        public void MessagePull(MessageModel[] messages, string userid)
        {
            Clients.User(userid).directmsg(messages);
        }

        public static ConcurrentDictionary<string, SessionWrapper> MyUsers = new ConcurrentDictionary<string, SessionWrapper>();

        public override Task OnConnected() {
            //MyUsers.TryAdd(Context.User.Identity.Name,  );
            string name = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, name);

            if (MyUsers.ContainsKey(name) && !MyUsers[name].NotificationPooler) {
                MyUsers[name].UpdateNotification();
                MyUsers[name].NotificationPooler = true;
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //SessionWrapper session;
            //MyUsers.TryRemove(Context.User.Identity.Name, out session);
           
            return base.OnDisconnected(stopCalled);
        }
    }
}