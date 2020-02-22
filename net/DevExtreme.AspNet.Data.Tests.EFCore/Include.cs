using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class Include {

        [Table(nameof(Include) + "_" + nameof(Category))]
        public class Category {
            [Key]
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }

            [InverseProperty(nameof(Include.Product.Category))]
            public virtual ICollection<Product> Products { get; set; }
        }

        [Table(nameof(Include) + "_" + nameof(ProductID))]
        public class Product {
            [Key]
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }

            public int? CategoryID { get; set; }

            [ForeignKey(nameof(CategoryID))]
            [InverseProperty(nameof(Include.Category.Products))]
            public virtual Category Category { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var categories = context.Set<Category>();
                var products = context.Set<Product>();

                var beverages = new Category {
                    CategoryName = "Beverages"
                };

                var chai = new Product {
                    ProductName = "Chai",
                    UnitPrice = 18,
                    Category = beverages
                };

                categories.Add(beverages);
                products.Add(chai);

                context.SaveChanges();

                Assert.Null(Record.Exception(delegate {
                    var loadResult = DataSourceLoader.Load(products.Include(p => p.Category), new SampleLoadOptions {
                        Filter = new[] { "UnitPrice", ">", "15" }
                    });

                    loadResult.data.Cast<object>().ToArray();
                }));
            });
        }

    }

}
