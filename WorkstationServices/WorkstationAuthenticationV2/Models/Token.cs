using System;
using System.Collections.Generic;

namespace WorkstationAuthenticationV2.Models
{
    public partial class Token
    {
        public string Jni { get; set; }
        public string Token1 { get; set; }  // Final Token
        public string TKey { get; set; }    // Ciphering / Deciphering key
        public DateTime Exp { get; set; }
        public DateTime Beg { get; set; }
        /// <summary>
        /// NOTE: Refactor
        /// Boundmac was originally designed to map a MAC address with a Token
        /// It quickly changed and became the unciphered payload
        /// </summary>
        public string Boundmac { get; set; }
    }
}
