using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationManagementServices.Models{
    public class SystemInfoModel {
        public IEnumerable<DbConnectionModel> Databases { get; set; }
        public IEnumerable<SiteModel> WebSites { get; set; }
    }

    public class DbConnectionModel
    {
        public DbConnectionModel(){
        }
        public String DataSource { get; set; }
        public String State { get; set; }
        public String Database { get; set; }
        public String ServerInfo { get; set; }
    }

    public class SiteModel{
        public SiteModel()
        {
        }
        public String Name { get; set;  }
        public String State { get;set; }
        public String PhysicalPath { get; set; }
    }
}