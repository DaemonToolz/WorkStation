using System;
using System.Collections.Generic;

namespace WorkstationAuthenticationV2.Models
{
    public partial class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? Encrypted { get; set; }
    }
}
