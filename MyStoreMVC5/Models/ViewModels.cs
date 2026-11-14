using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStoreMVC5.Models
{
    public class ProductSearchVm
    {
        public string Search { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Sort { get; set; } = "newest";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    public class LoginVm
    {
        [Required, Display(Name = "Tên đăng nhập")] public string Username { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Mật khẩu")] public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class RegisterVm
    {
        [Required, StringLength(255), Display(Name="Tên đăng nhập")] public string Username { get; set; }
        [Required, MinLength(6), DataType(DataType.Password), Display(Name="Mật khẩu")] public string Password { get; set; }
        [Required, Display(Name="Họ tên")] public string CustomerName { get; set; }
        [Required, StringLength(15), Phone, Display(Name="Số điện thoại")] public string CustomerPhone { get; set; }
        [EmailAddress, Display(Name="Email")] public string CustomerEmail { get; set; }
        [Display(Name="Địa chỉ")] public string CustomerAddress { get; set; }
    }

    public class ProfileVm
    {
        [Required, Display(Name="Họ tên")] public string CustomerName { get; set; }
        [Required, StringLength(15), Phone, Display(Name="Số điện thoại")] public string CustomerPhone { get; set; }
        [EmailAddress] public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
    }

    public class CartLine
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Amount => Quantity * UnitPrice;
    }

    public class CheckoutVm
    {
        [Required, Display(Name="Địa chỉ nhận hàng")] public string AddressDelivery { get; set; }
        public List<CartLine> Items { get; set; } = new List<CartLine>();
        public decimal Total => Items.SumValue();
    }

    public static class CartLineExtensions
    {
        public static decimal SumValue(this IEnumerable<CartLine> lines)
        {
            decimal total = 0;
            if (lines != null) foreach (var item in lines) total += item.Amount;
            return total;
        }
    }

    public class AdminUserVm
    {
        [Required, StringLength(255)] public string Username { get; set; }
        [DataType(DataType.Password)] public string Password { get; set; }
    }

    public class DashboardVm
    {
        public int Products { get; set; }
        public int Categories { get; set; }
        public int Customers { get; set; }
        public int Orders { get; set; }
        public decimal PaidRevenue { get; set; }
    }
}
