using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using System.Data.Entity;

namespace Web_QLKhachSan.Controllers
{
    public class HomeController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        public ActionResult Index()
        {
            // ✅ DEBUG: Kiểm tra Session và User.Identity
            System.Diagnostics.Debug.WriteLine("=== HOME INDEX DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"User.Identity.Name: {User.Identity.Name}");
            System.Diagnostics.Debug.WriteLine($"Session[HoVaTen]: {Session["HoVaTen"]}");
            System.Diagnostics.Debug.WriteLine($"Session[MaKhachHang]: {Session["MaKhachHang"]}");
            System.Diagnostics.Debug.WriteLine($"Session[Email]: {Session["Email"]}");
            System.Diagnostics.Debug.WriteLine("========================");

            // Tạo ViewModel
            var viewModel = new HomeIndexViewModel();

            // Load dịch vụ để hiển thị trên trang chủ
            viewModel.LoaiDichVus = db.LoaiDichVus
                .OrderBy(l => l.LoaiDichVuId)
                .Take(4)
                .ToList();

            // Load loại phòng từ database
            viewModel.SearchModel.RoomTypes = db.LoaiPhongs
                .Where(lp => lp.Phongs.Any(p => p.DaHoatDong)) // Chỉ lấy loại phòng có phòng đang hoạt động
                .OrderBy(lp => lp.TenLoai)
                .ToList();

            // Set default search values
            viewModel.SearchModel.CheckInDate = DateTime.Now.Date;
            viewModel.SearchModel.CheckOutDate = DateTime.Now.Date.AddDays(1);
            viewModel.SearchModel.GuestCount = 2;

            // Thống kê cho hero section
            ViewBag.TotalRooms = db.Phongs.Count(p => p.DaHoatDong);
            ViewBag.TotalCustomers = db.KhachHangs.Count();
            
            // Tính điểm đánh giá trung bình
            var avgRating = db.DanhGias.Any() 
                ? db.DanhGias.Average(d => (double?)d.Diem) ?? 0 
                : 0;
            ViewBag.AverageRating = Math.Round(avgRating, 1);

            // === 3 PHÒNG NỔI BẬT ===
            
            // 1. PHÒNG PHỔ BIẾN - Phòng được đặt nhiều nhất
            var phongPhoBien = db.ChiTietDatPhongs
                .Where(ct => ct.PhongId.HasValue && ct.Phong.DaHoatDong)
                .GroupBy(ct => ct.PhongId)
                .Select(g => new {
                    PhongId = g.Key.Value,
                    SoLanDat = g.Count()
                })
                .OrderByDescending(x => x.SoLanDat)
                .FirstOrDefault();
            
            ViewBag.PhongPhoBien = phongPhoBien != null 
                ? db.Phongs.Include(p => p.LoaiPhong).FirstOrDefault(p => p.PhongId == phongPhoBien.PhongId)
                : db.Phongs.Include(p => p.LoaiPhong).Where(p => p.DaHoatDong).FirstOrDefault();

            // 2. PHÒNG CAO CẤP - Phòng có giá cao nhất
            ViewBag.PhongCaoCap = db.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => p.DaHoatDong && p.Gia.HasValue)
                .OrderByDescending(p => p.Gia)
                .FirstOrDefault();

            // 3. PHÒNG GIA ĐÌNH - Phòng chứa nhiều người nhất (theo SoNguoiToiDa của LoaiPhong)
            ViewBag.PhongGiaDinh = db.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => p.DaHoatDong && p.LoaiPhong.SoNguoiToiDa.HasValue)
                .OrderByDescending(p => p.LoaiPhong.SoNguoiToiDa)
                .FirstOrDefault();

            // === ĐÁNH GIÁ & NHẬN XÉT ===
            ViewBag.DanhGias = db.DanhGias
                .Include(d => d.KhachHang)
                .Include(d => d.Phong)
                .Include(d => d.DanhGiaHinhAnhs)
                .OrderByDescending(d => d.NgayTao)
                .Take(5)
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SearchRooms(DateTime checkInDate, DateTime checkOutDate, int? guestCount, int? roomTypeId)
        {
            // Validate dates
            if (checkInDate < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Ngày nhận phòng phải từ hôm nay trở đi!";
                return RedirectToAction("Index");
            }

            if (checkOutDate <= checkInDate)
            {
                TempData["ErrorMessage"] = "Ngày trả phòng phải sau ngày nhận phòng!";
                return RedirectToAction("Index");
            }

            // Validate guest count (optional)
            if (guestCount.HasValue && (guestCount.Value < 1 || guestCount.Value > 4))
            {
                TempData["ErrorMessage"] = "Số khách phải từ 1 đến 4 người!";
                return RedirectToAction("Index");
            }

            // Store search parameters in TempData for PhongNghi controller
            TempData["SearchCheckIn"] = checkInDate;
            TempData["SearchCheckOut"] = checkOutDate;
            TempData["SearchGuests"] = guestCount;
            TempData["SearchRoomType"] = roomTypeId;

            // Redirect to room listing page with search parameters
            return RedirectToAction("Index", "PhongNghi", new
            {
                checkin = checkInDate.ToString("yyyy-MM-dd"),
                checkout = checkOutDate.ToString("yyyy-MM-dd"),
                guests = guestCount,
                roomtype = roomTypeId
            });
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