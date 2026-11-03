using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25DH110544_LTW.Models;

namespace _25DH110544_LTW.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities database = new MyStoreEntities();

        // GET: Admin/Order (Hiển thị danh sách đơn hàng kèm theo thông tin khách hàng)
        public ActionResult Index()
        {
            var orders = database.Orders.Include(o => o.Customer).OrderByDescending(o => o.OrderDate);
            return View(orders.ToList());
        }

        // GET: Admin/Order/Details/5 (Xem chi tiết đơn hàng và các sản phẩm trong đơn)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Order order = database.Orders.Include(o => o.Customer).Include(o => o.OrderDetails.Select(od => od.Product)).FirstOrDefault(o => o.OrderID == id);

            if (order == null) return HttpNotFound();

            return View(order);
        }

        // GET: Admin/Order/Edit/5 (Cập nhật trạng thái đơn hàng)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Order order = database.Orders.Find(id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                database.Entry(order).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                database.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}