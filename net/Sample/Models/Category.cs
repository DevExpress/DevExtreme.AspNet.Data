using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models {

    [Table("Categories")]
    public partial class Category {
        public Category() {
            Products = new HashSet<Product>();
        }

        [Key]
        public int CategoryID { get; set; }
        [Required]
        [MaxLength(15)]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public byte[] Picture { get; set; }

        [JsonIgnore]
        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
