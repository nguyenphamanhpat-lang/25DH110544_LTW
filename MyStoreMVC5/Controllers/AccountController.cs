using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Controllers
{
    public class AccountController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        [HttpGet, AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginVm { ReturnUrl = returnUrl });
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Login(LoginVm input)
        {
            if (!ModelState.IsValid) return View(input);
            var user = db.Users.Find(input.Username);
            if (user == null || !PasswordHasher.Verify(input.Password, user.Password))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(input);
            }
            FormsAuthentication.SetAuthCookie(user.Username, false);
            if (Url.IsLocalUrl(input.ReturnUrl)) return Redirect(input.ReturnUrl);
            return RedirectToAction("Index", (user.UserRole ?? "").Trim() == "A" ? "Home" : "Home",
                new { area = (user.UserRole ?? "").Trim() == "A" ? "Admin" : "" });
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Register() { return View(new RegisterVm()); }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Register(RegisterVm input)
        {
            if (!ModelState.IsValid) return View(input);
            input.Username = input.Username.Trim();
            if (db.Users.Any(x => x.Username == input.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(input);
            }
            using (var tx = db.Database.BeginTransaction())
            {
                try
                {
                    db.Users.Add(new AppUser { Username = input.Username,
                        Password = PasswordHasher.Hash(input.Password), UserRole = "C" });
                    db.Customers.Add(new Customer {
                        Username = input.Username, CustomerName = input.CustomerName.Trim(),
                        CustomerPhone = input.CustomerPhone.Trim(),
                        CustomerEmail = input.CustomerEmail,
                        CustomerAddress = input.CustomerAddress });
                    db.SaveChanges();
                    tx.Commit();
                }
                catch (System.Data.DataException)
                {
                    tx.Rollback();
                    ModelState.AddModelError("", "Không thể đăng ký tài khoản. Kiểm tra lại dữ liệu.");
                    return View(input);
                }
            }
            FormsAuthentication.SetAuthCookie(input.Username, false);
            return RedirectToAction("Index", "Home");
        }

        [Authorize, HttpGet]
        public ActionResult Edit()
        {
            var customer = db.Customers.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return HttpNotFound();
            return View(new ProfileVm {
                CustomerName = customer.CustomerName, CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail, CustomerAddress = customer.CustomerAddress
            });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ProfileVm input)
        {
            var customer = db.Customers.FirstOrDefault(x => x.Username == User.Identity.Name);
            if (customer == null) return HttpNotFound();
            if (!ModelState.IsValid) return View(input);
            customer.CustomerName = input.CustomerName.Trim();
            customer.CustomerPhone = input.CustomerPhone.Trim();
            customer.CustomerEmail = input.CustomerEmail;
            customer.CustomerAddress = input.CustomerAddress;
            db.SaveChanges();
            TempData["Notice"] = "Đã lưu thông tin.";
            return RedirectToAction("Edit");
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
