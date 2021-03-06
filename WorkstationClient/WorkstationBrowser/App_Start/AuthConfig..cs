﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(WorkstationBrowser.AuthConfig))]
namespace WorkstationBrowser{
   
    public partial class AuthConfig{
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Login/Login"),
                LogoutPath = new PathString("/Login/Logout"),
                CookieName = ".WorkstationCookie",
                SlidingExpiration = true,
                ExpireTimeSpan = new System.TimeSpan(0,10,0),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                AuthenticationMode = AuthenticationMode.Active
            });

           
            app.MapSignalR();
        }
    }
}