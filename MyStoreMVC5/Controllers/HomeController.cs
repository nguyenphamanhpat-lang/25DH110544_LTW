using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using MyStoreMVC5.Models;

namespace MyStoreMVC5.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index()
        {
            var model = SearchProducts(new ProductSearchVm());
            return View(model);
        }

        public ActionResult ProductList(ProductSearchVm filter)
        {
            return View("Index", SearchProducts(filter));
        }

        public ActionResult ProductDetail(int id)
        {
            var product = db.Products.Include(x => x.Category).FirstOrDefault(x => x.ProductID == id);
            return product == null ? (ActionResult)HttpNotFound() : View(product);
        }

        private ProductSearchVm SearchProducts(ProductSearchVm f)
        {
            f = f ?? new ProductSearchVm();
            f.Page = f.Page < 1 ? 1 : f.Page;
            f.PageSize = 8;
            f.Categories = db.Categories.OrderBy(x => x.CategoryName).ToList();

            var q = db.Products.Include(x => x.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var term = f.Search.Trim();
                q = q.Where(x => x.ProductName.Contains(term) ||
                    x.ProductDescription.Contains(term) || x.Category.CategoryName.Contains(term));
            }
            if (f.CategoryId.HasValue) q = q.Where(x => x.CategoryID == f.CategoryId.Value);
            if (f.MinPrice.HasValue) q = q.Where(x => x.ProductPrice >= f.MinPrice.Value);
            if (f.MaxPrice.HasValue) q = q.Where(x => x.ProductPrice <= f.MaxPrice.Value);
            f.TotalCount = q.Count();
            switch (f.Sort)
            {
                case "price_asc": q = q.OrderBy(x => x.ProductPrice).ThenBy(x => x.ProductID); break;
                case "price_desc": q = q.OrderByDescending(x => x.ProductPrice).ThenBy(x => x.ProductID); break;
                case "name_asc": q = q.OrderBy(x => x.ProductName).ThenBy(x => x.ProductID); break;
                case "name_desc": q = q.OrderByDescending(x => x.ProductName).ThenBy(x => x.ProductID); break;
                default: f.Sort = "newest"; q = q.OrderByDescending(x => x.ProductID); break;
            }
            if (f.Page > f.TotalPages) f.Page = f.TotalPages;
            f.Products = q.Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToList();
            return f;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
