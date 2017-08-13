using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WorkstationBrowser.Controllers.SignalR
{
    public class MessageHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}