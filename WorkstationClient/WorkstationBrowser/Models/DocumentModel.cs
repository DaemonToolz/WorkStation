using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.Models {

    public class DocumentModel {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Extension { get; set; }

        [Required]
        public String Path { get; set; }

        [Required]
        public bool Directory { get; set; }

        [Required]
        public String Parent { get; set; }
    }
}