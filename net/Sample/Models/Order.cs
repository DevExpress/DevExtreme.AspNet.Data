using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace Sample.Models {

    [Table("Orders")]
    public class Order {
        public Order() {
            Order_Details = new HashSet<Order_Details>();
        }

        [Key]
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public decimal? Freight { get; set; }
        [Required] public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        [Required] public string ShipCountry { get; set; }
        public string ShipName { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipRegion { get; set; }
        public int? ShipVia { get; set; }

        [JsonIgnore]
        [InverseProperty("Order")]
        public virtual ICollection<Order_Details> Order_Details { get; set; }

        [ForeignKey("CustomerID")]
        [InverseProperty("Orders")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ShipVia")]
        [InverseProperty("Orders")]
        public virtual Shipper Shipper { get; set; }
    }
}
