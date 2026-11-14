using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class CategoriesController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index() { return View(db.Categories.OrderBy(x => x.CategoryName).ToList()); }
        public ActionResult Details(int id)
        {
            var entry = db.Categories.Find(id);
            return entry == null ? (ActionResult)HttpNotFound() : View(entry);
        }

        public ActionResult Create() { return View("Form", new Category()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Category input)
        {
            if (!ModelState.IsValid) return View("Form", input);
            db.Categories.Add(new Category { CategoryName = input.CategoryName.Trim() });
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var entry = db.Categories.Find(id);
            return entry == null ? (ActionResult)HttpNotFound() : View("Form", entry);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Category input)
        {
            if (!ModelState.IsValid) return View("Form", input);
            var entry = db.Categories.Find(input.CategoryID);
            if (entry == null) return HttpNotFound();
            entry.CategoryName = input.CategoryName.Trim();
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var entry = db.Categories.Find(id);
            return entry == null ? (ActionResult)HttpNotFound() : View(entry);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var entry = db.Categories.Find(id);
            if (entry == null) return HttpNotFound();
            if (db.Products.Any(x => x.CategoryID == id))
            {
                TempData["Error"] = "Không thể xóa danh mục đang chứa sản phẩm.";
                return RedirectToAction("Index");
            }
            db.Categories.Remove(entry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
