//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DDFrAS
{
    using System;
    using System.Collections.Generic;
    
    public partial class SWITCH
    {
        public int Switch_ID { get; set; }
        public string Switch_Name { get; set; }
    
        public virtual PORT PORT { get; set; }
        public virtual PRODUCT PRODUCT { get; set; }
    }
}
