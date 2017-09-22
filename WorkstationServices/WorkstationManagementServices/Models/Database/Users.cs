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
    
    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.Project = new HashSet<Project>();
            this.Team1 = new HashSet<Team>();
            this.File = new HashSet<File>();
            this.File1 = new HashSet<File>();
            this.ChangeSet = new HashSet<ChangeSet>();
        }
    
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public Nullable<bool> encrypted { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> creationdate { get; set; }
        public Nullable<int> team_id { get; set; }
        public string rank { get; set; }
        public string profilepic { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual Rank Rank1 { get; set; }
        public virtual Users Users1 { get; set; }
        public virtual Users Users2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project> Project { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Team1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<File> File { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<File> File1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChangeSet> ChangeSet { get; set; }
    }
}
