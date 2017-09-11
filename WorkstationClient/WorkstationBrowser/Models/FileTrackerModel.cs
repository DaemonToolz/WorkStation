using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Web;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Models{
    
    public class FileTrackerModel
    {
        [Required]
        public String TrackedFile { get; set; }

        [Required]
        public UsersModel[] Users{ get; set; }

        [Required]
        public CommentModel[] Comments { get; set; }

    }
}