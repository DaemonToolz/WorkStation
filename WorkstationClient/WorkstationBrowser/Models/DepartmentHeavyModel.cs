using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Web;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Models{
    
    public class DepartmentHeavyModel {
        [Required]
        public DepartmentModel OriginalModel { get; set; }

        [Required]
        public UsersModel[] Users{ get; set; }

        [Required]
        public TeamModel[] Teams { get; set; }

    }
}