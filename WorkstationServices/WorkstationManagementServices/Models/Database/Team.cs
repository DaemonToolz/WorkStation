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
    
    public partial class Team
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Team()
        {
            this.Users = new HashSet<Users>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<int> department_id { get; set; }
        public Nullable<long> project_id { get; set; }
        public string teampic { get; set; }
        public Nullable<int> manager_id { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users { get; set; }
        public virtual Project Project { get; set; }
        public virtual Users Users1 { get; set; }
    }
}
