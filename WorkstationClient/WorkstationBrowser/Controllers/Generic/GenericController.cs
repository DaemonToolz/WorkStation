using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorkstationBrowser.Controllers.Remote;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Controllers.Generic {
    public class GenericController : Controller {
        protected SessionWrapper _Session {
            get => Session["WorkstationConnection"] as SessionWrapper;
            set {
                if (value == null || Session["WorkstationConnection"] == null)
                    Session["WorkstationConnection"] = value;
            }
        }

        protected int _UnreadNotifications {
            get => int.Parse(Session["UnreadNotifications"].ToString());
            set => Session["UnreadNotifications"] = value;
        }


   
        protected Dictionary<String, bool> _UserRights {
            get => Session["CurrentUserRights"] as Dictionary<String, bool>;
            set {
                if(Session["CurrentUserRights"] == null)
                    Session["CurrentUserRights"] = value;
            }
        }

        protected NotificationModel[] _UserNotifications
        {
            get => Session["SystemNotifications"] as NotificationModel[];
            set => Session["SystemNotifications"] = value;
        }


        protected MessageModel[] _UserMessages {
            get => Session["MyMessages"] as MessageModel[];
            set => Session["MyMessages"] = value;
        }

    }
}