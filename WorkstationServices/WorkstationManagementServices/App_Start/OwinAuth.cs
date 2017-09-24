using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WorkstationManagementServices.App_Start.OwinAuth))]

namespace WorkstationManagementServices.App_Start
{
    public class OwinAuth
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
