//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkstationManagementServices.Models.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class File
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public File()
        {
            this.ChangeSet = new HashSet<ChangeSet>();
        }
    
        public string tracker_id { get; set; }
        public string name { get; set; }
        public int owner_id { get; set; }
        public int last_updater { get; set; }
        public System.DateTime creation_date { get; set; }
        public System.DateTime last_update { get; set; }
        public int change_count { get; set; }
        public long project_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChangeSet> ChangeSet { get; set; }
        public virtual Project Project { get; set; }
        public virtual Users Users { get; set; }
        public virtual Users Users1 { get; set; }
    }
}
