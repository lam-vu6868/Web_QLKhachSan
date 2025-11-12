using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class PhongNghiController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: PhongNghi
        public ActionResult Index(string sort, int? page, int? priceLevel, int[] loaiPhong, int[] tienIch)
        {
            // Lấy danh sách loại phòng từ database
            var loaiPhongs = db.LoaiPhongs.ToList();
            ViewBag.LoaiPhongs = loaiPhongs;

            // Lấy danh sách tiện ích từ database
            var tienIches = db.TienIches.ToList();
            ViewBag.TienIches = tienIches;

            // Tính giá min/max từ database
            var allPhongs = db.Phongs.Include("LoaiPhong")
                                    .Where(p => p.DaHoatDong == true)
                                    .ToList();
            
            var prices = allPhongs.Select(p => p.Gia ?? p.LoaiPhong.GiaCoBan ?? 0).Where(price => price > 0).ToList();
            
            if (prices.Any())
            {
                ViewBag.MinPrice = prices.Min();
                ViewBag.MaxPrice = prices.Max();
            }
            else
            {
                ViewBag.MinPrice = 50;
                ViewBag.MaxPrice = 500;
            }

            // Lấy danh sách phòng với thông tin liên quan
            var phongs = db.Phongs.Include("LoaiPhong")
                                  .Include("LoaiPhong.TienIches")
                                  .Include("PhongAnhs")
                                  .Where(p => p.DaHoatDong == true)
                                  .AsQueryable();

            // Logic: 2 bộ lọc độc lập - chỉ áp dụng 1 trong 2
            // Ưu tiên: nếu có loaiPhong thì bỏ qua priceLevel, và ngược lại
            
            bool hasLoaiPhongFilter = loaiPhong != null && loaiPhong.Length > 0;
            bool hasPriceFilter = priceLevel.HasValue;
            
            if (hasLoaiPhongFilter)
            {
                // Nếu lọc theo loại phòng -> BỎ QUA lọc giá, chỉ lọc theo loại phòng
                phongs = phongs.Where(p => loaiPhong.Contains(p.LoaiPhongId));
                
                // Reset priceLevel trong ViewBag
                ViewBag.PriceLevel = null;
                ViewBag.SelectedLoaiPhong = loaiPhong;
            }
            else if (hasPriceFilter)
            {
                // Nếu lọc theo giá -> BỎ QUA lọc loại phòng, chỉ lọc theo giá
                var priceLevels = new decimal[] { 300000, 400000, 600000, 900000 };
                if (priceLevel.Value >= 0 && priceLevel.Value < priceLevels.Length)
                {
                    var maxPrice = priceLevels[priceLevel.Value];
                    phongs = phongs.Where(p => (p.Gia ?? p.LoaiPhong.GiaCoBan ?? 0) <= maxPrice);
                }
                
                // Reset loaiPhong trong ViewBag
                ViewBag.PriceLevel = priceLevel;
                ViewBag.SelectedLoaiPhong = null;
            }
            else
            {
                // Không có filter nào -> hiển thị tất cả
                ViewBag.PriceLevel = null;
                ViewBag.SelectedLoaiPhong = null;
            }

            // Lọc theo tiện ích (vẫn giữ nguyên, không bị ảnh hưởng)
            if (tienIch != null && tienIch.Length > 0)
            {
                phongs = phongs.Where(p => p.LoaiPhong.TienIches.Any(ti => tienIch.Contains(ti.TienIchId)));
            }

            // Giữ lại filter tiện ích
            ViewBag.SelectedTienIch = tienIch;

            // Xử lý sắp xếp
            switch (sort)
            {
                case "price-asc":
                    phongs = phongs.OrderBy(p => p.Gia ?? p.LoaiPhong.GiaCoBan ?? 0);
                    break;
                case "price-desc":
                    phongs = phongs.OrderByDescending(p => p.Gia ?? p.LoaiPhong.GiaCoBan ?? 0);
                    break;
                default:
                    // Mặc định: sắp xếp theo PhongId
                    phongs = phongs.OrderBy(p => p.PhongId);
                    break;
            }

            ViewBag.CurrentSort = sort ?? "default";
            
            // Phân trang
            int pageSize = 6; // 6 phòng mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1
            
            var totalItems = phongs.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            
            var pagedPhongs = phongs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            return View(pagedPhongs);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}