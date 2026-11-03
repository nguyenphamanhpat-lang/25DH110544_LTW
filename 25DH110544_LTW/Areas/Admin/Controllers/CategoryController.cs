using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25DH110544_LTW.Models;

namespace _25DH110544_LTW.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // Khởi tạo DbContext để kết nối CSDL từ Entity Framework
        private MyStoreEntities database = new MyStoreEntities();

        // 1. GET: Admin/Category/Index (Hiển thị danh sách danh mục)
        public ActionResult Index()
        {
            var categories = database.Categories.ToList();
            return View(categories);
        }

        // 2. GET: Admin/Category/Create (Hiển thị form thêm mới)
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create (Xử lý lưu dữ liệu thêm mới)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                database.Categories.Add(category);
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // 3. GET: Admin/Category/Edit/5 (Hiển thị form chỉnh sửa theo ID)
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Category category = database.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: Admin/Category/Edit/5 (Xử lý cập nhật dữ liệu chỉnh sửa)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                database.Entry(category).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // 4. GET: Admin/Category/Delete/5 (Hiển thị trang xác nhận xóa)
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Category category = database.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // POST: Admin/Category/Delete/5 (Xử lý xóa danh mục khỏi CSDL)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = database.Categories.Find(id);
            database.Categories.Remove(category);
            database.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}