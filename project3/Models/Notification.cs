//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace project3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Notification
    {
        public int no_ID { get; set; }
        public string NameNo { get; set; }
        public System.DateTime Time { get; set; }
        public string NoDetails { get; set; }
        public Nullable<int> pro_ID { get; set; }
        public Nullable<int> SellCus { get; set; }
        public Nullable<int> BuyCus { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Customer Customer1 { get; set; }
        public virtual Product Product { get; set; }
    }
}
