using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.Models{
    public class LogInModel {
        [Required]
        public String Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        
        public bool AutoConnect { get; set; }
    }
}