//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkstationServices.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public long id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public bool read { get; set; }
        public bool direct { get; set; }
        public System.DateTime stamp { get; set; }
    
        public virtual Users Users { get; set; }
        public virtual Users Users1 { get; set; }
    }
}
