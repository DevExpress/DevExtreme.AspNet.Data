using Microsoft.EntityFrameworkCore;

namespace Sample.Models {

    public partial class NorthwindContext : DbContext {
        public NorthwindContext() {
        }

        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options) {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Shippers> Shippers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>(entity => {
                entity.HasIndex(e => e.CategoryName)
                    .HasDatabaseName("CategoryName");
            });

            modelBuilder.Entity<Customer>(entity => {
                entity.HasIndex(e => e.City)
                    .HasDatabaseName("City");

                entity.HasIndex(e => e.CompanyName)
                    .HasDatabaseName("CompanyName");

                entity.HasIndex(e => e.PostalCode)
                    .HasDatabaseName("PostalCode");

                entity.HasIndex(e => e.Region)
                    .HasDatabaseName("Region");

                entity.Property(e => e.CustomerId).ValueGeneratedNever();
            });

            modelBuilder.Entity<OrderDetail>(entity => {
                entity.HasKey(e => new { e.OrderId, e.ProductId });

                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("OrdersOrder_Details");

                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("ProductsOrder_Details");

                entity.Property(e => e.Discount).HasDefaultValueSql("(0)");

                entity.Property(e => e.Quantity).HasDefaultValueSql("(1)");

                entity.Property(e => e.UnitPrice).HasDefaultValueSql("(0)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            modelBuilder.Entity<Order>(entity => {
                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("CustomersOrders");

                entity.HasIndex(e => e.EmployeeId)
                    .HasDatabaseName("EmployeesOrders");

                entity.HasIndex(e => e.OrderDate)
                    .HasDatabaseName("OrderDate");

                entity.HasIndex(e => e.ShipPostalCode)
                    .HasDatabaseName("ShipPostalCode");

                entity.HasIndex(e => e.ShipVia)
                    .HasDatabaseName("ShippersOrders");

                entity.HasIndex(e => e.ShippedDate)
                    .HasDatabaseName("ShippedDate");

                entity.Property(e => e.Freight).HasDefaultValueSql("(0)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipVia)
                    .HasConstraintName("FK_Orders_Shippers");
            });

            modelBuilder.Entity<Product>(entity => {
                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("CategoryID");

                entity.HasIndex(e => e.ProductName)
                    .HasDatabaseName("ProductName");

                entity.HasIndex(e => e.SupplierId)
                    .HasDatabaseName("SuppliersProducts");

                entity.Property(e => e.Discontinued).HasDefaultValueSql("(0)");

                entity.Property(e => e.ReorderLevel).HasDefaultValueSql("(0)");

                entity.Property(e => e.UnitPrice).HasDefaultValueSql("(0)");

                entity.Property(e => e.UnitsInStock).HasDefaultValueSql("(0)");

                entity.Property(e => e.UnitsOnOrder).HasDefaultValueSql("(0)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_Categories");
            });
        }
    }
}
