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
    
    public partial class GroupContents
    {
        public int Id { get; set; }
        public int IdGroup { get; set; }
        public int IdContent { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Contents Contents { get; set; }
        public virtual Groups Groups { get; set; }
    }
}
