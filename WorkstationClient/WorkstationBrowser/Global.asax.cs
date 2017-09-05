using System;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WorkstationBrowser.Controllers.SignalR;

namespace WorkstationBrowser
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

        }
        /*
        protected void Application_Error(object sender, EventArgs e)
        {
            
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                
                string errMsg;
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        errMsg = "Resource not found";
                        break;
                    case 500:
                        // server error
                        errMsg = "Internal server error";
                        break;
                    default:
                        errMsg = "We don't what it is nor when it stroke!";
                        break;
                }

                // clear error on server
                Server.ClearError();
                
                Response.Redirect($"~/Error/HttpGenericError?message={errMsg}");
            }
            else
            {
                Server.ClearError();
                var errMsg = "An error occured while treating your request!";
                Response.Redirect($"~/Error/HttpGenericError?message={errMsg}");
            }
        }
       */
    }
}
