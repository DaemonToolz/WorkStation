using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkstationBrowser.Models{
    public class MessageHeavyModel {
        [Required]
        public String title { get; set; }

        [Required]
        public String content { get; set; }

        [Required]
        public int[] to { get; set; }

    }
}