using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WorkstationSignalR_Dev.OWINSignalRChatStartup))]

namespace WorkstationSignalR_Dev
{
    public class OWINSignalRChatStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
