using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStoreMVC5.Models
{
    // EF6 mapping for the supplied MyStore SQL Server database. No automatic migrations.
    public class StoreDbContext : DbContext
    {
        static StoreDbContext() { Database.SetInitializer<StoreDbContext>(null); }
        public StoreDbContext() : base("name=MyStore") { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreOrder> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder m)
        {
            m.Entity<AppUser>().ToTable("User", "dbo").HasKey(x => x.Username);
            m.Entity<AppUser>().Property(x => x.Password).IsFixedLength().IsUnicode().HasMaxLength(50);
            m.Entity<AppUser>().Property(x => x.UserRole).IsFixedLength().IsUnicode().HasMaxLength(1);

            m.Entity<Customer>().ToTable("Customer", "dbo").HasKey(x => x.CustomerID);
            m.Entity<Customer>().Property(x => x.CustomerID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            m.Entity<Customer>().HasRequired(x => x.User).WithMany().HasForeignKey(x => x.Username).WillCascadeOnDelete(false);

            m.Entity<Category>().ToTable("Category", "dbo").HasKey(x => x.CategoryID);
            m.Entity<Category>().Property(x => x.CategoryID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            m.Entity<Product>().ToTable("Product", "dbo").HasKey(x => x.ProductID);
            m.Entity<Product>().Property(x => x.ProductID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            m.Entity<Product>().Property(x => x.ProductPrice).HasPrecision(18, 2);
            m.Entity<Product>().HasRequired(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryID).WillCascadeOnDelete(false);

            m.Entity<StoreOrder>().ToTable("Order", "dbo").HasKey(x => x.OrderID);
            m.Entity<StoreOrder>().Property(x => x.OrderID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            m.Entity<StoreOrder>().Property(x => x.OrderDate).HasColumnType("date");
            m.Entity<StoreOrder>().Property(x => x.TotalAmount).HasPrecision(18, 2);
            m.Entity<StoreOrder>().HasRequired(x => x.Customer).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerID).WillCascadeOnDelete(false);

            m.Entity<OrderDetail>().ToTable("OrderDetail", "dbo").HasKey(x => x.ID);
            m.Entity<OrderDetail>().Property(x => x.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            m.Entity<OrderDetail>().Property(x => x.UnitPrice).HasPrecision(18, 2);
            m.Entity<OrderDetail>().HasRequired(x => x.Product).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductID).WillCascadeOnDelete(false);
            m.Entity<OrderDetail>().HasRequired(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderID).WillCascadeOnDelete(false);
            base.OnModelCreating(m);
        }
    }
}
