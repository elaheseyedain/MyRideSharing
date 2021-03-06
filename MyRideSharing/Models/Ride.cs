﻿//------------------------------------------------------------------------------
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
    using System.ComponentModel.DataAnnotations;

    public partial class Ride
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ride()
        {
            this.Seats = new HashSet<Seat>();
        }


        public int Id { get; set; }
        public Nullable<decimal> SourceLatitude { get; set; }
        public Nullable<decimal> SourceLongitude { get; set; }
        public Nullable<decimal> DestinationLatitude { get; set; }
        public Nullable<decimal> DestinationLongitude { get; set; }
        //public Nullable<System.DateTime> StartTime { get; set; }
        //public Nullable<System.DateTime> EndTime { get; set; }


        [Display(Name = "تاریخ")]
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        [Display(Name = "زمان حرکت")]
        public String myStartTime { get; set; }

        [Display(Name = "زمان رسیدن")]
        public String myEndTime { get; set; }


        [Display(Name = "تخمین مدت")]
        public int Duration { get; set; }

        [UIHint("DateTime")]
        //[RegularExpression(@"^(13\d{2}|[1-9]\d)/(1[012]|0?[1-9])/([12]\d|3[01]|0?[1-9]) ([01][0-9]|2[0-3]):([0-5]?[0-9])$", ErrorMessage = "تاریخ ثبت را بدرستی وارد کنید")]
        [Display(Name = "تاریخ")]
        public DateTime myDate { get; set; }

        [Display(Name = "قیمت هر صندلی")]
        public Nullable<int> Price { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "مبدا")]
        public string SourcePlace { get; set; }

        [Display(Name = "مقصد")]
        public string DestinationPlace { get; set; }

        [Display(Name = "تعداد صندلی")]
        public short EmptySeats { get; set; }
        public int CarOwnerId { get; set; }

        public virtual CarOwner CarOwner { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Seat> Seats { get; set; }
    }
}
