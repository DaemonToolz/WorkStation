using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.Models{
    public class NotificationModel {
        public String Title { get; set; }
        public String Content { get; set; }
        public bool Read { get; set; }
    }
}