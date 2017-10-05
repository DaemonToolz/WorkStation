using System;
using System.ComponentModel.DataAnnotations;

namespace WorkstationUWP.Common{
    public class LogInModel {
        public String Username { get; set; }
        [DataType(DataType.Password)]
        public String Password { get; set; }
        
        public bool AutoConnect { get; set; }
    }
}