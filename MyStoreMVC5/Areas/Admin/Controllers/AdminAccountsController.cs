using System;
using System.Linq;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class AdminAccountsController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index(string search, int page = 1)
        {
            page = Math.Max(1, page);
            var q = db.Users.Where(x => x.UserRole == "A");
            if (!string.IsNullOrWhiteSpace(search)) q = q.Where(x => x.Username.Contains(search));
            ViewBag.Search = search;
            ViewBag.Pages = Math.Max(1, (int)Math.Ceiling(q.Count() / 10.0));
            page = Math.Min(page, (int)ViewBag.Pages);
            ViewBag.Page = page;
            return View(q.OrderBy(x => x.Username).Skip((page - 1) * 10).Take(10).ToList());
        }

        public ActionResult Create() { return View("Form", new AdminUserVm()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(AdminUserVm input)
        {
            if (string.IsNullOrWhiteSpace(input.Password) || input.Password.Length < 8)
                ModelState.AddModelError("Password", "Mật khẩu cần ít nhất 8 ký tự.");
            if (!ModelState.IsValid) return View("Form", input);
            string username = input.Username.Trim();
            if (db.Users.Any(x => x.Username == username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View("Form", input);
            }
            db.Users.Add(new AppUser { Username = username,
                Password = PasswordHasher.Hash(input.Password), UserRole = "A" });
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            var entry = db.Users.Find(id);
            if (entry == null || (entry.UserRole ?? "").Trim() != "A") return HttpNotFound();
            return View("Form", new AdminUserVm { Username = entry.Username });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AdminUserVm input)
        {
            var entry = db.Users.Find(input.Username);
            if (entry == null || (entry.UserRole ?? "").Trim() != "A") return HttpNotFound();
            if (!string.IsNullOrEmpty(input.Password) && input.Password.Length < 8)
                ModelState.AddModelError("Password", "Mật khẩu mới cần ít nhất 8 ký tự.");
            if (!ModelState.IsValid) return View("Form", input);
            if (!string.IsNullOrEmpty(input.Password))
            {
                entry.Password = PasswordHasher.Hash(input.Password);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var entry = db.Users.Find(id);
            if (entry == null || (entry.UserRole ?? "").Trim() != "A") return HttpNotFound();
            return View(entry);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var entry = db.Users.Find(id);
            if (entry == null || (entry.UserRole ?? "").Trim() != "A") return HttpNotFound();
            if (id == User.Identity.Name || db.Users.Count(x => x.UserRole == "A") <= 1)
            {
                TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập hoặc tài khoản Admin cuối cùng.";
                return RedirectToAction("Index");
            }
            db.Users.Remove(entry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
