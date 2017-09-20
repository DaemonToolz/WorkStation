using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkstationManagementServices.Models
{
    public class ProjectHeavyModel
    {
        public DetailedFileMode[] FileModels { get; set; }
    }

    public class DetailedFileMode{
        public String Filename { get; set; }
        public int Changes { get; set; }
        public int NewContent { get; set; }
        public int RemovedContent { get; set; }
    }

    public class ChangeSetModel
    {
        
    }
}