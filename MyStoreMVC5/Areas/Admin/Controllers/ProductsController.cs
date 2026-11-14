using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MyStoreMVC5.Models;
using MyStoreMVC5.Security;

namespace MyStoreMVC5.Areas.Admin.Controllers
{
    [RoleAuthorize("A")]
    public class ProductsController : Controller
    {
        private readonly StoreDbContext db = new StoreDbContext();

        public ActionResult Index(ProductSearchVm filter)
        {
            filter = filter ?? new ProductSearchVm();
            filter.Page = Math.Max(1, filter.Page);
            filter.PageSize = 10;
            filter.Categories = db.Categories.OrderBy(x => x.CategoryName).ToList();
            var q = db.Products.Include(x => x.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string keyword = filter.Search.Trim();
                q = q.Where(x => x.ProductName.Contains(keyword) ||
                    x.ProductDescription.Contains(keyword) || x.Category.CategoryName.Contains(keyword));
            }
            if (filter.CategoryId.HasValue) q = q.Where(x => x.CategoryID == filter.CategoryId.Value);
            if (filter.MinPrice.HasValue) q = q.Where(x => x.ProductPrice >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) q = q.Where(x => x.ProductPrice <= filter.MaxPrice.Value);
            filter.TotalCount = q.Count();
            switch (filter.Sort)
            {
                case "price_asc": q = q.OrderBy(x => x.ProductPrice).ThenBy(x => x.ProductID); break;
                case "price_desc": q = q.OrderByDescending(x => x.ProductPrice).ThenBy(x => x.ProductID); break;
                case "name_asc": q = q.OrderBy(x => x.ProductName).ThenBy(x => x.ProductID); break;
                case "name_desc": q = q.OrderByDescending(x => x.ProductName).ThenBy(x => x.ProductID); break;
                default: filter.Sort = "newest"; q = q.OrderByDescending(x => x.ProductID); break;
            }
            filter.Page = Math.Min(filter.Page, filter.TotalPages);
            filter.Products = q.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            return View(filter);
        }

        public ActionResult Details(int id)
        {
            var entry = db.Products.Include(x => x.Category).FirstOrDefault(x => x.ProductID == id);
            return entry == null ? (ActionResult)HttpNotFound() : View(entry);
        }

        public ActionResult Create()
        {
            Categories();
            return View("Form", new Product());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Product input, HttpPostedFileBase image)
        {
            ModelState.Remove("Category");
            if (db.Categories.Find(input.CategoryID) == null)
                ModelState.AddModelError("CategoryID", "Danh mục không hợp lệ.");
            ValidateUpload(image);
            if (!ModelState.IsValid) { Categories(); return View("Form", input); }
            var entry = new Product {
                CategoryID = input.CategoryID, ProductName = input.ProductName.Trim(),
                ProductDescription = input.ProductDescription.Trim(), ProductPrice = input.ProductPrice
            };
            db.Products.Add(entry);
            db.SaveChanges();
            if (image != null)
            {
                try { entry.ProductImage = SaveImage(image); db.SaveChanges(); }
                catch (IOException) { TempData["Error"] = "Đã tạo sản phẩm nhưng không lưu được ảnh."; }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var entry = db.Products.Find(id);
            if (entry == null) return HttpNotFound();
            Categories();
            return View("Form", entry);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Product input, HttpPostedFileBase image)
        {
            ModelState.Remove("Category");
            if (db.Categories.Find(input.CategoryID) == null)
                ModelState.AddModelError("CategoryID", "Danh mục không hợp lệ.");
            ValidateUpload(image);
            if (!ModelState.IsValid) { Categories(); return View("Form", input); }
            var entry = db.Products.Find(input.ProductID);
            if (entry == null) return HttpNotFound();
            entry.ProductName = input.ProductName.Trim();
            entry.ProductDescription = input.ProductDescription.Trim();
            entry.ProductPrice = input.ProductPrice;
            entry.CategoryID = input.CategoryID;
            if (image != null)
            {
                try { entry.ProductImage = SaveImage(image); }
                catch (IOException) { TempData["Error"] = "Không thể lưu ảnh mới."; }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var entry = db.Products.Find(id);
            return entry == null ? (ActionResult)HttpNotFound() : View(entry);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var entry = db.Products.Find(id);
            if (entry == null) return HttpNotFound();
            if (db.OrderDetails.Any(x => x.ProductID == id))
            {
                TempData["Error"] = "Không thể xóa sản phẩm đã phát sinh đơn hàng.";
                return RedirectToAction("Index");
            }
            db.Products.Remove(entry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void Categories()
        {
            ViewBag.Categories = new SelectList(db.Categories.OrderBy(x => x.CategoryName).ToList(),
                "CategoryID", "CategoryName");
        }

        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private void ValidateUpload(HttpPostedFileBase image)
        {
            if (image == null) return;
            string ext = Path.GetExtension(image.FileName ?? "").ToLowerInvariant();
            if (image.ContentLength < 1 || image.ContentLength > 5 * 1024 * 1024 ||
                !AllowedExtensions.Contains(ext) || image.ContentType == null ||
                !image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("image", "Chỉ hỗ trợ ảnh JPG/PNG/WebP, tối đa 5 MB.");
        }

        private string SaveImage(HttpPostedFileBase file)
        {
            // Verify file signatures to avoid accepting executable/text disguised as images.
            byte[] head = new byte[12];
            int n = file.InputStream.Read(head, 0, head.Length);
            file.InputStream.Position = 0;
            var jpg = n >= 3 && head[0] == 0xff && head[1] == 0xd8 && head[2] == 0xff;
            var png = n >= 8 && head[0] == 0x89 && head[1] == 0x50 &&
                head[2] == 0x4e && head[3] == 0x47 && head[4] == 0x0d && head[5] == 0x0a;
            var webp = n >= 12 && head[0] == 0x52 && head[1] == 0x49 &&
                head[2] == 0x46 && head[3] == 0x46 && head[8] == 0x57 && head[9] == 0x45;
            string ext = Path.GetExtension(file.FileName ?? "").ToLowerInvariant();
            if (!(jpg && (ext == ".jpg" || ext == ".jpeg") || png && ext == ".png" || webp && ext == ".webp"))
                throw new IOException("Invalid image signature.");
            string directory = Server.MapPath("~/Content/images");
            Directory.CreateDirectory(directory);
            string filename = Guid.NewGuid().ToString("N") + ext;
            file.SaveAs(Path.Combine(directory, filename));
            return "/Content/images/" + filename;
        }

        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
