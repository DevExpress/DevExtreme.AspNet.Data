using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace Sample.Models {

    [Table("Customers")]
    public class Customer {
        public Customer() {
            Orders = new HashSet<Order>();
        }

        [Key]
        public string CustomerID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Country { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }

        [JsonIgnore]
        [InverseProperty("Customer")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
