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
    
    public partial class Tokens
    {
        public int Id { get; set; }
        public int IdType { get; set; }
        public int IdUser { get; set; }
        public Nullable<int> IdContent { get; set; }
        public string Code { get; set; }
        public System.DateTime ExpireDate { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual Contents Contents { get; set; }
        public virtual TokenTypes TokenTypes { get; set; }
        public virtual Users Users { get; set; }
    }
}
