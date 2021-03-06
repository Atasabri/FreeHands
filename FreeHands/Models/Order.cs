//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FreeHands.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderOptions = new HashSet<OrderOption>();
        }
    
        public int ID { get; set; }
        public System.DateTime Date { get; set; }
        public int Number { get; set; }
        public bool IsOrderd { get; set; }
        public bool IsUseVisa { get; set; }
        public string Status { get; set; }
        public Nullable<int> Driver_ID { get; set; }
        public Nullable<int> Offer_ID { get; set; }
        public int User_ID { get; set; }
        public int Product_ID { get; set; }
        public bool Accepted { get; set; }
        public bool Ready { get; set; }
        public bool ISArrivedToUser { get; set; }
        public double UserLat { get; set; }
        public Nullable<double> TaxForVisa { get; set; }
        public Nullable<double> Total { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<int> Delivery { get; set; }
        public Nullable<System.DateTime> DateTimeNeeded { get; set; }
        public string Reason { get; set; }
        public double UserLog { get; set; }
        public bool ProblemInArriveToUser { get; set; }
        public Nullable<double> Price { get; set; }
    
        public virtual Driver Driver { get; set; }
        public virtual Offer Offer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderOption> OrderOptions { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
