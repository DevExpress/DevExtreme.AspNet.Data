using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace Sample.Models {

    [Table("Products")]
    public class Product {
        public Product() {
            Order_Details = new HashSet<Order_Details>();
        }

        [Key]
        public int ProductID { get; set; }
        public int? CategoryID { get; set; }
        public bool Discontinued { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public short? ReorderLevel { get; set; }
        public int? SupplierID { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }

        [JsonIgnore]
        [InverseProperty("Product")]
        public virtual ICollection<Order_Details> Order_Details { get; set; }

        [ForeignKey("CategoryID")]
        [InverseProperty("Products")]
        public virtual Category Category { get; set; }
    }
}
