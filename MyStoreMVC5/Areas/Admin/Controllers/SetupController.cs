using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    // Initial admin bootstrap. Only available from loopback and when no admin exists.
    public class SetupController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        private bool Permitted()
        {
            var ip = Request.UserHostAddress;
            return (ip == "127.0.0.1" || ip == "::1") && !db.Users.Any(x => x.UserRole == "A");
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!Permitted()) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            return View(new AdminUserVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(AdminUserVm input)
        {
            if (!Permitted()) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (string.IsNullOrWhiteSpace(input.Password) || input.Password.Length < 8)
                ModelState.AddModelError("Password", "Mật khẩu ít nhất 8 ký tự.");
            if (!ModelState.IsValid) return View(input);
            var username = input.Username.Trim();
            if (db.Users.Any(x => x.Username == username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(input);
            }
            using (var tx = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                if (db.Users.Any(x => x.UserRole == "A"))
                {
                    tx.Rollback();
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                db.Users.Add(new AppUser {
                    Username = username, Password = PasswordHasher.Hash(input.Password), UserRole = "A"
                });
                db.SaveChanges();
                tx.Commit();
            }
            TempData["Notice"] = "Tạo Admin thành công. Hãy đăng nhập.";
            return RedirectToAction("Login", "Account", new { area = "" });
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
