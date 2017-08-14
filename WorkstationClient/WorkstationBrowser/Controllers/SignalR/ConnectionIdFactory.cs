using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WorkstationBrowser.Controllers.SignalR
{
    public class ConnectionIdFactory : IConnectionIdFactory
    {
        public string CreateConnectionId(IRequest request)
            {
                if (request.Cookies["srconnectionid"] != null)
                {
                    return request.Cookies["srconnectionid"].ToString();
                }

                return Guid.NewGuid().ToString();
            
        }
    }

    public interface IConnectionIdFactory{
        String CreateConnectionId(IRequest request);
    }
}