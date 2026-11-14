using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class HomeController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index()
        {
            var data = new DashboardVm {
                Products = db.Products.Count(), Categories = db.Categories.Count(),
                Customers = db.Customers.Count(), Orders = db.Orders.Count(),
                PaidRevenue = db.Orders.Where(x => x.PaymentStatus == "PAID")
                    .Select(x => (decimal?)x.TotalAmount).Sum() ?? 0
            };
            return View(data);
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
