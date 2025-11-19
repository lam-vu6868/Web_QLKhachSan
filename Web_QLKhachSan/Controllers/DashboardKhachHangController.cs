using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Services;

namespace Web_QLKhachSan.Controllers
{
    [Authorize]
    public class DashboardKhachHangController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();
        private CloudinaryService _cloudinaryService = new CloudinaryService();

        // GET: DashboardKhachHang
        public ActionResult TaiKhoan()
        {
            // Kiểm tra session
            if (Session["MaKhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }
            return View();
        }
        public ActionResult HoSo()
        {
            // Kiểm tra session
            if (Session["MaKhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            // Load thông tin khách hàng từ database vào Session
            if (Session["MaKhachHang"] != null)
            {
                int maKhachHang = (int)Session["MaKhachHang"];
                var khachHang = db.KhachHangs.Find(maKhachHang);
                
                if (khachHang != null)
                {
                    Session["HoVaTen"] = khachHang.HoVaTen;
                    Session["SoDienThoai"] = khachHang.SoDienThoai;
                    Session["NgaySinh"] = khachHang.NgaySinh;
                    Session["DiaChi"] = khachHang.DiaChi;
                    Session["AnhDaiDienUrl"] = khachHang.AnhDaiDienUrl;
                }
            }

            return View();
        }

        [HttpPost]
        public JsonResult CapNhatThongTin(string hoVaTen, string soDienThoai, string ngaySinh, string diaChi)
        {
            try
            {
                if (Session["MaKhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục" });
                }

                int maKhachHang = (int)Session["MaKhachHang"];
                var khachHang = db.KhachHangs.Find(maKhachHang);

                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                // Cập nhật thông tin
                if (!string.IsNullOrWhiteSpace(hoVaTen))
                {
                    khachHang.HoVaTen = hoVaTen.Trim();
                    Session["HoVaTen"] = hoVaTen.Trim();
                }

                if (!string.IsNullOrWhiteSpace(soDienThoai))
                {
                    khachHang.SoDienThoai = soDienThoai.Trim();
                }

                if (!string.IsNullOrWhiteSpace(ngaySinh))
                {
                    DateTime ngaySinhDate;
                    if (DateTime.TryParse(ngaySinh, out ngaySinhDate))
                    {
                        khachHang.NgaySinh = ngaySinhDate;
                        Session["NgaySinh"] = ngaySinhDate;
                    }
                }

                if (!string.IsNullOrWhiteSpace(diaChi))
                {
                    khachHang.DiaChi = diaChi.Trim();
                }

                khachHang.NgayCapNhat = DateTime.Now;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadAvatar()
        {
            try
            {
                if (Session["MaKhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục" });
                }

                if (Request.Files.Count == 0)
                {
                    return Json(new { success = false, message = "Không tìm thấy file ảnh" });
                }

                var file = Request.Files["avatar"];
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "File ảnh không hợp lệ" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Chỉ chấp nhận file ảnh định dạng JPG, PNG, GIF" });
                }

                // Validate file size (max 5MB)
                if (file.ContentLength > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "Kích thước ảnh không được vượt quá 5MB" });
                }

                int maKhachHang = (int)Session["MaKhachHang"];
                var khachHang = db.KhachHangs.Find(maKhachHang);

                if (khachHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin khách hàng" });
                }

                // Xóa ảnh cũ trên Cloudinary (nếu có)
                if (!string.IsNullOrEmpty(khachHang.AnhDaiDienUrl))
                {
                    _cloudinaryService.DeleteImage(khachHang.AnhDaiDienUrl);
                }

                // Upload ảnh mới lên Cloudinary
                // Folder sẽ tự động được tạo: LuuTruAnh_QLKhachSan/AnhKhachHang/Avatar/{HoVaTen}_MaKhachHang{MaKH}
                var avatarUrl = _cloudinaryService.UploadAvatarKhachHang(file, maKhachHang, khachHang.HoVaTen);

                // Update database
                khachHang.AnhDaiDienUrl = avatarUrl;
                khachHang.NgayCapNhat = DateTime.Now;
                db.SaveChanges();

                // Update session
                Session["AnhDaiDienUrl"] = avatarUrl;

                return Json(new { success = true, message = "Cập nhật ảnh đại diện thành công!", avatarUrl = avatarUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        public ActionResult LichSu()
        {
            // Kiểm tra session
            if (Session["MaKhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            return View();
        }
        public ActionResult CapDo()
        {
            // Kiểm tra session
            if (Session["MaKhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            return View();
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