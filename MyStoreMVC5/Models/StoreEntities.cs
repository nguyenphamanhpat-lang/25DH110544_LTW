using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStoreMVC5.Models
{
    public class AppUser
    {
        [Key, Required, StringLength(255)]
        public string Username { get; set; }
        [Required, StringLength(50)]
        public string Password { get; set; }
        [Required, StringLength(1)]
        public string UserRole { get; set; } // A = Admin; C = Customer
    }

    public class Customer
    {
        [Key] public int CustomerID { get; set; }
        [Required, StringLength(200)] public string CustomerName { get; set; }
        [Required, StringLength(15)] public string CustomerPhone { get; set; }
        [EmailAddress] public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        [Required, StringLength(255)] public string Username { get; set; }
        public AppUser User { get; set; }
        public ICollection<StoreOrder> Orders { get; set; } = new List<StoreOrder>();
    }

    public class Category
    {
        [Key] public int CategoryID { get; set; }
        [Required] public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public class Product
    {
        [Key] public int ProductID { get; set; }
        [Required] public int CategoryID { get; set; }
        [Required] public string ProductName { get; set; }
        [Required] public string ProductDescription { get; set; }
        [Range(0.01, 999999999999)] public decimal ProductPrice { get; set; }
        public string ProductImage { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public class StoreOrder
    {
        [Key] public int OrderID { get; set; }
        [Required] public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        [Required] public string AddressDelivery { get; set; }
        public Customer Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public class OrderDetail
    {
        [Key] public int ID { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        [Range(1, 99)] public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Product Product { get; set; }
        public StoreOrder Order { get; set; }
    }
}
