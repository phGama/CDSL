//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ContentDeliverySystem.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ContentTypes
    {
        public ContentTypes()
        {
            this.Contents = new HashSet<Contents>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<Contents> Contents { get; set; }
    }
}
