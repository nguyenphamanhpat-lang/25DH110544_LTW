using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25DH110544_LTW.Models;

namespace _25DH110544_LTW.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private MyStoreEntities database = new MyStoreEntities();

        // 1. GET: Admin/Product/Index (Hiển thị danh sách sản phẩm kèm tên danh mục)
        public ActionResult Index()
        {
            var products = database.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // 2. GET: Admin/Product/Details/5 (Xem chi tiết sản phẩm)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = database.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // 3. GET: Admin/Product/Create (Hiển thị form thêm mới sản phẩm)
        public ActionResult Create()
        {
            // Đổ dữ liệu Category ra DropDownList để chọn danh mục cho sản phẩm
            ViewBag.CategoryID = new SelectList(database.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload hình ảnh nếu có chọn file
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    product.ProductImage = fileName; // Lưu tên file vào CSDL
                }

                database.Products.Add(product);
                database.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(database.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // 4. GET: Admin/Product/Edit/5 (Hiển thị form sửa sản phẩm)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = database.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.CategoryID = new SelectList(database.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    ImageFile.SaveAs(path);
                    product.ProductImage = fileName;
                }

                database.Entry(product).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(database.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // 5. GET: Admin/Product/Delete/5 (Xác nhận xóa sản phẩm)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = database.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = database.Products.Find(id);
            database.Products.Remove(product);
            database.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}