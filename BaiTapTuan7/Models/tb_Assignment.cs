//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaiTapTuan7.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_Assignment
    {
        public int Assignment_Id { get; set; }
        public Nullable<int> Course_Id { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> Deadline { get; set; }
        public string Details { get; set; }
        public Nullable<int> Status { get; set; }
        public string File { get; set; }
    }
}