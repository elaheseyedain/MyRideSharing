//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyRideSharing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Seat
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RideId { get; set; }
    
        public virtual Ride Ride { get; set; }
        public virtual User User { get; set; }
    }
}
