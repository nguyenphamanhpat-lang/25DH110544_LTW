using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Services;

namespace MyStoreMVC5.Controllers
{
    public class CartController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        [HttpGet]
        public ActionResult Index() { return View(CartService.Refresh(Session, db)); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddItem(int productId, int quantity = 1)
        {
            if (!CartService.Add(Session, db, productId, quantity))
                TempData["Error"] = "Sản phẩm không hợp lệ hoặc số lượng vượt giới hạn 99.";
            else
                TempData["Notice"] = "Đã thêm sản phẩm vào giỏ.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateItem(int productId, int quantity)
        {
            if (!CartService.Update(Session, productId, quantity))
                TempData["Error"] = "Số lượng phải từ 0 đến 99.";
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RemoveItem(int productId)
        {
            CartService.Update(Session, productId, 0);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
