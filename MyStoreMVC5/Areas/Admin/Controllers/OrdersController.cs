using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class OrdersController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index(string search, string sort = "newest", int page = 1)
        {
            var q = db.Orders.Include(x => x.Customer).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Customer.Username.Contains(search) ||
                    x.Customer.CustomerName.Contains(search));
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Pages = Math.Max(1, (int)Math.Ceiling(q.Count() / 10.0));
            page = Math.Max(1, Math.Min(page, (int)ViewBag.Pages));
            ViewBag.Page = page;
            switch (sort)
            {
                case "oldest": q = q.OrderBy(x => x.OrderID); break;
                case "total_desc": q = q.OrderByDescending(x => x.TotalAmount).ThenByDescending(x => x.OrderID); break;
                default: q = q.OrderByDescending(x => x.OrderID); break;
            }
            return View(q.Skip((page - 1) * 10).Take(10).ToList());
        }

        public ActionResult Details(int id)
        {
            var order = db.Orders.Include(x => x.Customer)
                .Include(x => x.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(x => x.OrderID == id);
            return order == null ? (ActionResult)HttpNotFound() : View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetStatus(int id, string status)
        {
            if (status != "COD_PENDING" && status != "PAID" && status != "CANCELLED")
                return new HttpStatusCodeResult(400);
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();
            order.PaymentStatus = status;
            db.SaveChanges();
            return RedirectToAction("Details", new { id });
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
