using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class DanhGiaController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: DanhGia
        public ActionResult Index()
        {
            // Lấy thông tin khách hàng từ Session
            var email = Session["Email"]?.ToString();
            var hoVaTen = Session["HoVaTen"]?.ToString();

            // Lấy tất cả đánh giá kèm thông tin khách hàng và phòng
            var danhGias = db.DanhGias
                .Include("KhachHang")
                .Include("Phong")
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            // Tính điểm trung bình
            var tongDiem = danhGias.Any() ? danhGias.Average(d => d.Diem) : 0;
            var soDanhGia = danhGias.Count();
            var phanTramHaiLong = danhGias.Any() ? (danhGias.Count(d => d.Diem >= 4) * 100.0 / soDanhGia) : 0;

            // Tạo ViewModel
            var viewModel = new DanhGiaIndexViewModel
            {
                DanhSachDanhGia = danhGias,
                DanhSachPhong = db.Phongs
                    .Where(p => p.DaHoatDong)
                    .Select(p => new SelectListItem
                    {
                        Value = p.PhongId.ToString(),
                        Text = p.TenPhong
                    })
                    .ToList(),
                TenKhachHang = hoVaTen,
                EmailKhachHang = email,
                TongDiem = Math.Round(tongDiem, 1),
                SoDanhGia = soDanhGia,
                PhanTramHaiLong = Math.Round(phanTramHaiLong, 0)
            };

            return View(viewModel);
        }

        // POST: DanhGia/ThemDanhGia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDanhGia(DanhGiaViewModel model)
        {
            var email = Session["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để đánh giá!";
                return RedirectToAction("Index");
            }

            // Tìm khách hàng
            var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == email);
            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index");
            }

            try
            {
                // Tạo đánh giá mới
                var danhGia = new DanhGia
                {
                    PhongId = model.PhongId,
                    MaKhachHang = khachHang.MaKhachHang,
                    Diem = model.Diem,
                    BinhLuan = model.BinhLuan ?? string.Empty,
                    NgayTao = DateTime.Now
                };

                db.DanhGias.Add(danhGia);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá! Đánh giá của bạn đã được ghi nhận.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index");
            }
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