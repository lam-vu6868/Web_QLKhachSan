using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Services;

namespace Web_QLKhachSan.Controllers
{
    public class DanhGiaController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();
        private CloudinaryService _cloudinaryService = new CloudinaryService();

        // GET: DanhGia
        public ActionResult Index(int page = 1)
        {
            // Lấy thông tin khách hàng từ Session
            var email = Session["Email"]?.ToString();
            var hoVaTen = Session["HoVaTen"]?.ToString();
            
            // Tìm khách hàng từ email
            var khachHang = !string.IsNullOrEmpty(email) 
                ? db.KhachHangs.FirstOrDefault(k => k.Email == email) 
                : null;

            // Phân trang: 6 đánh giá mỗi trang
            int pageSize = 6;
            
            // Lấy tất cả đánh giá để tính toán
            var allDanhGias = db.DanhGias
                .Include("KhachHang")
                .Include("Phong")
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            // Tính tổng số trang
            var totalRecords = allDanhGias.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Lấy dữ liệu cho trang hiện tại
            var danhGias = allDanhGias
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính điểm trung bình (từ tất cả đánh giá)
            var tongDiem = allDanhGias.Any() ? allDanhGias.Average(d => d.Diem) : 0;
            var soDanhGia = allDanhGias.Count();
            var phanTramHaiLong = allDanhGias.Any() ? (allDanhGias.Count(d => d.Diem >= 4) * 100.0 / soDanhGia) : 0;

            // Lấy danh sách phòng đã đặt (trạng thái hoàn thành) của khách hàng hiện tại
            List<SelectListItem> danhSachPhongDaDat = new List<SelectListItem>();
            
            if (khachHang != null)
            {
                // Lấy chính xác phòng có trạng thái = 2 (hoàn thành)
                var phongIdDaHoanThanh = db.ChiTietDatPhongs
                    .Where(ct => ct.DatPhong.MaKhachHang == khachHang.MaKhachHang 
                              && ct.DatPhong.TrangThaiDatPhong == 2 // Chỉ trạng thái 2
                              && ct.PhongId.HasValue)
                    .Select(ct => ct.PhongId.Value)
                    .Distinct()
                    .ToList();

                // Lấy danh sách phòng đã đánh giá (DISTINCT để tránh trùng lặp)
                var phongIdDaDanhGia = db.DanhGias
                    .Where(dg => dg.MaKhachHang == khachHang.MaKhachHang)
                    .Select(dg => dg.PhongId)
                    .Distinct()
                    .ToList();

                // CHỈ lấy phòng hoàn thành VÀ chưa đánh giá
                var phongIdChuaDanhGia = phongIdDaHoanThanh
                    .Where(id => !phongIdDaDanhGia.Contains(id))
                    .ToList();

                // NẾU có phòng chưa đánh giá thì mới tạo dropdown
                if (phongIdChuaDanhGia.Any())
                {
                    danhSachPhongDaDat = db.Phongs
                        .Where(p => phongIdChuaDanhGia.Contains(p.PhongId))
                        .OrderBy(p => p.TenPhong)
                        .Select(p => new SelectListItem
                        {
                            Value = p.PhongId.ToString(),
                            Text = p.TenPhong
                        })
                        .ToList();
                }

            }

            // Tạo ViewModel
            var viewModel = new DanhGiaIndexViewModel
            {
                DanhSachDanhGia = danhGias,
                DanhSachPhong = danhSachPhongDaDat,
                TenKhachHang = hoVaTen,
                EmailKhachHang = email,
                TongDiem = Math.Round(tongDiem, 1),
                SoDanhGia = soDanhGia,
                PhanTramHaiLong = Math.Round(phanTramHaiLong, 0),
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: DanhGia/GetReviews - Ajax endpoint for pagination
        [HttpGet]
        public JsonResult GetReviews(int page = 1)
        {
            int pageSize = 6;
            
            var allDanhGias = db.DanhGias
                .Include("KhachHang")
                .Include("Phong")
                .Include("DanhGiaHinhAnhs")
                .OrderByDescending(d => d.NgayTao)
                .ToList();

            var totalRecords = allDanhGias.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var danhGias = allDanhGias
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    danhGiaId = d.DanhGiaId,
                    khachHang = new
                    {
                        hoVaTen = d.KhachHang.HoVaTen,
                        email = d.KhachHang.Email,
                        anhDaiDien = d.KhachHang.AnhDaiDienUrl,
                        maKhachHang = d.KhachHang.MaKhachHang
                    },
                    diem = d.Diem,
                    binhLuan = d.BinhLuan,
                    ngayTao = d.NgayTao,
                    phong = new
                    {
                        tenPhong = d.Phong.TenPhong
                    },
                    hinhAnhs = d.DanhGiaHinhAnhs.Select(h => h.DuongDanAnh).ToList()
                })
                .ToList();

            return Json(new
            {
                success = true,
                data = danhGias,
                currentPage = page,
                totalPages = totalPages
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: DanhGia/ThemDanhGia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDanhGia(DanhGiaViewModel model, HttpPostedFileBase[] AnhDanhGia)
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
                // Kiểm tra xem khách hàng đã đánh giá phòng này chưa
                var daDanhGia = db.DanhGias.Any(dg => dg.MaKhachHang == khachHang.MaKhachHang 
                                                   && dg.PhongId == model.PhongId);
                if (daDanhGia)
                {
                    TempData["ErrorMessage"] = "Bạn đã đánh giá phòng này rồi. Mỗi phòng chỉ được đánh giá một lần!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra xem khách hàng có đặt phòng này với trạng thái hoàn thành không
                var daDatPhong = db.ChiTietDatPhongs.Any(ct => 
                    ct.DatPhong.MaKhachHang == khachHang.MaKhachHang 
                    && ct.PhongId == model.PhongId
                    && ct.DatPhong.TrangThaiDatPhong == 2); // Hoàn thành
                
                if (!daDatPhong)
                {
                    TempData["ErrorMessage"] = "Bạn chỉ có thể đánh giá những phòng đã đặt và hoàn thành!";
                    return RedirectToAction("Index");
                }

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

                // Upload ảnh lên Cloudinary (tối đa 5 ảnh)
                if (AnhDanhGia != null && AnhDanhGia.Length > 0)
                {
                    int count = 0;
                    foreach (var file in AnhDanhGia)
                    {
                        if (file != null && file.ContentLength > 0 && count < 5)
                        {
                            try
                            {
                                var imageUrl = _cloudinaryService.UploadAnhDanhGia(file, khachHang.MaKhachHang, khachHang.HoVaTen);
                                
                                if (!string.IsNullOrEmpty(imageUrl))
                                {
                                    db.DanhGiaHinhAnhs.Add(new DanhGiaHinhAnh
                                    {
                                        DanhGiaId = danhGia.DanhGiaId,
                                        DuongDanAnh = imageUrl
                                    });
                                    count++;
                                }
                            }
                            catch (Exception imgEx)
                            {
                                // Log lỗi nhưng vẫn tiếp tục
                                System.Diagnostics.Debug.WriteLine($"Lỗi upload ảnh: {imgEx.Message}");
                            }
                        }
                    }
                    db.SaveChanges();
                }

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