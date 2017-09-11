using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WorkstationBrowser.SessionReference;

namespace WorkstationBrowser.Models {
    public class CommentModel{
        
        public String Id { get; set; }

        public String AuthorName { get; set; }

        [Required]
        public String Content { get; set; }

        [Required]
        public UsersModel  Author { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}