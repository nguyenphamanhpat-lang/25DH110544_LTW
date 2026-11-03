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
    public class CustomerController : Controller
    {
        private MyStoreEntities database = new MyStoreEntities();

        // GET: Admin/Customer
        public ActionResult Index(string searchTerm)
        {
            var customers = database.Customers.AsQueryable();

            // Tìm kiếm khách hàng theo tên hoặc email/số điện thoại nếu cần
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c => c.CustomerName.Contains(searchTerm) ||
                                               c.CustomerEmail.Contains(searchTerm) ||
                                               c.CustomerPhone.Contains(searchTerm));
            }

            return View(customers.ToList());
        }

        // GET: Admin/Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = database.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // GET: Admin/Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = database.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // POST: Admin/Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                database.Entry(customer).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Admin/Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Customer customer = database.Customers.Find(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // POST: Admin/Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = database.Customers.Find(id);
            database.Customers.Remove(customer);
            database.SaveChanges();
            return RedirectToAction("Index");
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