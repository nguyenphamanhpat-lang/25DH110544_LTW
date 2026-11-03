using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _25DH110544_LTW.Models;

namespace _25DH110544_LTW.Models.ViewModel
{
    public class ProductSearchVM
    {
        // Tiêu chí tìm kiếm theo từ khóa (tên, mô tả, danh mục)
        public string SearchTerm { get; set; }

        // Tiêu chí tìm kiếm nâng cao theo khoảng giá
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Thứ tự sắp xếp
        public string SortOrder { get; set; }

        // Danh sách sản phẩm phân trang (Sử dụng thư viện PagedList)
        public PagedList.IPagedList<Product> Products { get; set; }
    }
}