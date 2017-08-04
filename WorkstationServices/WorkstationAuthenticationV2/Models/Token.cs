using System;
using System.Collections.Generic;

namespace WorkstationAuthenticationV2.Models
{
    public partial class Token
    {
        public string Jni { get; set; }
        public string Token1 { get; set; }
        public string TKey { get; set; }
        public DateTime Exp { get; set; }
        public DateTime Beg { get; set; }
        public string Boundmac { get; set; }
    }
}
