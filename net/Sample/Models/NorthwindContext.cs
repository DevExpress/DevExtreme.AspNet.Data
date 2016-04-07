using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Sample.Models {
    public partial class NorthwindContext : DbContext {
        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            options.UseSqlServer(@"data source=.\SQLEXPRESS;initial catalog=Northwind;integrated security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>(entity => {
                entity.HasIndex(e => e.CategoryName).HasName("CategoryName");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Picture).HasColumnType("image");
            });

            modelBuilder.Entity<Order_Details>(entity => {
                entity.HasKey(e => new { e.OrderID, e.ProductID });

                entity.Property(e => e.Discount).HasDefaultValue(0f);

                entity.Property(e => e.Quantity).HasDefaultValue(1);

                entity.Property(e => e.UnitPrice).HasDefaultValue(0m);
            });

            modelBuilder.Entity<Order>(entity => {
                entity.Property(e => e.Freight).HasDefaultValue(0m);
            });

            modelBuilder.Entity<Product>(entity => {
                entity.Property(e => e.Discontinued).HasDefaultValue(false);

                entity.Property(e => e.ReorderLevel).HasDefaultValue(0);

                entity.Property(e => e.UnitPrice).HasDefaultValue(0m);

                entity.Property(e => e.UnitsInStock).HasDefaultValue(0);

                entity.Property(e => e.UnitsOnOrder).HasDefaultValue(0);
            });
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order_Details> Order_Details { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
    }
}