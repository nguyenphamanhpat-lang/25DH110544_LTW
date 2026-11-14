using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class CustomersController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index(string search, string sort = "name", int page = 1)
        {
            var query = db.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.CustomerName.Contains(search) ||
                    x.CustomerPhone.Contains(search) || x.Username.Contains(search));
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Pages = Math.Max(1, (int)Math.Ceiling(query.Count() / 10.0));
            page = Math.Max(1, Math.Min(page, (int)ViewBag.Pages));
            ViewBag.Page = page;
            switch (sort)
            {
                case "newest": query = query.OrderByDescending(x => x.CustomerID); break;
                case "name_desc": query = query.OrderByDescending(x => x.CustomerName).ThenBy(x => x.CustomerID); break;
                default: query = query.OrderBy(x => x.CustomerName).ThenBy(x => x.CustomerID); break;
            }
            return View(query.Skip((page - 1) * 10).Take(10).ToList());
        }

        public ActionResult Details(int id)
        {
            var customer = db.Customers.Include(x => x.Orders).FirstOrDefault(x => x.CustomerID == id);
            return customer == null ? (ActionResult)HttpNotFound() : View(customer);
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
