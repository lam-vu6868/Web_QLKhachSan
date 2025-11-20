using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Services;
using BCrypt.Net;

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

        public ActionResult LichSu(int? page)
        {
            // Kiểm tra session
            if (Session["MaKhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Login");
            }

            int maKhachHang = (int)Session["MaKhachHang"];
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var allDatPhong = db.DatPhongs
                .Include("ChiTietDatPhongs.Phong")
                .Include("ChiTietDatPhongs.LoaiPhong")
                .Include("ChiTietDatDichVus.DichVu")
                .Where(dp => dp.MaKhachHang == maKhachHang)
                .OrderByDescending(dp => dp.NgayDat);

            var totalItems = allDatPhong.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var lichSuDatPhong = allDatPhong
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(lichSuDatPhong);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DoiMatKhau(RangBuocHoSoViewModel model)
        {
            try
            {
                // Kiểm tra session
                if (Session["MaKhachHang"] == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục";
                    return RedirectToAction("DangNhap", "Login");
                }

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin";
                    return RedirectToAction("HoSo");
                }

                int maKhachHang = (int)Session["MaKhachHang"];
                var khachHang = db.KhachHangs.Find(maKhachHang);

                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng";
                    return RedirectToAction("HoSo");
                }

                // Tìm tài khoản trong bảng TaiKhoans
                var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.MaKhachHang == maKhachHang);
                
                if (taiKhoan == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tài khoản";
                    return RedirectToAction("HoSo");
                }

                // Xác thực mật khẩu hiện tại bằng BCrypt
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.MatKhauHienTai, taiKhoan.MatKhauHash);
                
                if (!isPasswordCorrect)
                {
                    TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác";
                    return RedirectToAction("HoSo");
                }

                // Băm mật khẩu mới bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);

                // Cập nhật mật khẩu vào bảng TaiKhoans (KHÔNG phải KhachHangs)
                taiKhoan.MatKhauHash = hashedPassword;
                db.SaveChanges();

                // Gửi email thông báo
                await SendPasswordChangeEmailAsync(khachHang.Email, khachHang.HoVaTen);

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Vui lòng kiểm tra email để xác nhận.";
                return RedirectToAction("HoSo");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return RedirectToAction("HoSo");
            }
        }

        private async Task SendPasswordChangeEmailAsync(string email, string tenKhachHang)
        {
            try
            {
                var emailService = new MailKitEmailService();
                string subject = "🔐 Thông Báo Thay Đổi Mật Khẩu Thành Công";
                
                // Đọc template từ file
                string templatePath = Server.MapPath("~/Views/DashboardKhachHang/EmailChangePassword.cshtml");
                string htmlTemplate = System.IO.File.ReadAllText(templatePath);
                
                // Thay thế các placeholder
                string htmlBody = htmlTemplate
                    .Replace("{{TenKhachHang}}", tenKhachHang)
                    .Replace("{{Email}}", email)
                    .Replace("{{ThoiGian}}", DateTime.Now.ToString("HH:mm:ss, dd/MM/yyyy"))
                    .Replace("{{LogoUrl}}", "https://res.cloudinary.com/dq1qfnr1z/image/upload/v1763633720/logo_tkukm5.png"); // Thay URL logo của bạn

                await emailService.SendEmailAsync(email, subject, htmlBody);
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw exception để không ảnh hưởng đến flow chính
                System.Diagnostics.Debug.WriteLine($"Lỗi gửi email: {ex.Message}");
            }
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