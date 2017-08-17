using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers.SignalR
{
    public class MessageHub : Hub
    {
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

        public void MessagePull(MessageModel[] messages, string userid)
        {
            Clients.User(userid).messages(messages.Count());
        }

    }
}