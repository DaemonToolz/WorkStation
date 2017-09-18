using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.Models{
   
    public class VerComparativeItem{
        [Required]
        public int Code { get; set; }

        [Required]
        public int BeginLine { get; set; }

        [Required]
        public int EndLine { get; set; }
        
        [Required]
        public String[] Differences { get; set; }
    }
}