using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Services;

namespace MyStoreMVC5.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        [HttpGet]
        public ActionResult Checkout()
        {
            var customer = db.Customers.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return RedirectToAction("Login", "Account");
            var items = CartService.Refresh(Session, db);
            if (items.Count == 0) return RedirectToAction("Index", "Cart");
            return View(new CheckoutVm { AddressDelivery = customer.CustomerAddress, Items = items });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVm input)
        {
            var customer = db.Customers.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return new HttpStatusCodeResult(403);
            // Do not rely on posted cart, posted prices or client-calculated totals.
            var lines = CartService.Refresh(Session, db);
            input.Items = lines;
            if (lines.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng đang trống.");
                return View(input);
            }
            if (!ModelState.IsValid) return View(input);

            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new StoreOrder {
                        CustomerID = customer.CustomerID,
                        OrderDate = DateTime.Today,
                        TotalAmount = lines.SumValue(),
                        PaymentStatus = "COD_PENDING",
                        AddressDelivery = input.AddressDelivery.Trim()
                    };
                    db.Orders.Add(order);
                    foreach (var line in lines)
                        order.OrderDetails.Add(new OrderDetail {
                            ProductID = line.ProductID, Quantity = line.Quantity, UnitPrice = line.UnitPrice
                        });
                    db.SaveChanges();
                    tx.Commit();
                    CartService.Clear(Session);
                    return RedirectToAction("OrderSuccess", new { id = order.OrderID });
                }
                catch (System.Data.DataException)
                {
                    tx.Rollback();
                    ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng thử lại.");
                    return View(input);
                }
            }
        }

        [HttpGet]
        public ActionResult OrderSuccess(int id)
        {
            var order = OwnedOrder(id);
            return order == null ? (ActionResult)HttpNotFound() : View(order);
        }

        [HttpGet]
        public ActionResult OrderHistory(int page = 1)
        {
            var customer = db.Customers.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return HttpNotFound();
            page = Math.Max(1, page);
            ViewBag.Page = page;
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(db.Orders.Count(x => x.CustomerID == customer.CustomerID) / 10.0));
            if (page > (int)ViewBag.TotalPages) page = (int)ViewBag.TotalPages;
            ViewBag.Page = page;
            var orders = db.Orders.Where(x => x.CustomerID == customer.CustomerID)
                .OrderByDescending(x => x.OrderID).Skip((page - 1) * 10).Take(10).ToList();
            return View(orders);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var order = OwnedOrder(id);
            return order == null ? (ActionResult)HttpNotFound() : View(order);
        }

        private StoreOrder OwnedOrder(int id)
        {
            return db.Orders.Include(x => x.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(x => x.OrderID == id && x.Customer.Username == User.Identity.Name);
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
