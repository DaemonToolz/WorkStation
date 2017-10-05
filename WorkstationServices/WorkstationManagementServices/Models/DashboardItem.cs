using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkstationManagementServices.Models.Database;

namespace WorkstationManagementServices.Models {
    public class DashboardItem {
        public String Name { get; set; }
        public String Parent { get; set; }

        public String CommentFile => $"_comtrack{Name}.xml";
        public String Absolute => Parent + Name;
        public String CommentAbsolute => Parent + CommentFile;

        public IEnumerable<CommentModel> Comments { get; set; }
        public ChangeSet[] Changes { get; set; }
    }

    public class CommentModel {
        public String Author { get; set; }
        public String Date { get; set; }
        public String Content { get; set; }
    }
}