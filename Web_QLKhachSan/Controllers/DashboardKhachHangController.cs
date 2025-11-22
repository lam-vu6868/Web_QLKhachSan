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

        [HttpGet]
        public JsonResult GetChiTietDatPhong(int id)
        {
            try
            {
                // Kiểm tra session
                if (Session["MaKhachHang"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" }, JsonRequestBehavior.AllowGet);
                }

                int maKhachHang = (int)Session["MaKhachHang"];

                var datPhong = db.DatPhongs
                    .Include("ChiTietDatPhongs.Phong")
                    .Include("ChiTietDatPhongs.LoaiPhong.TienNghis.LoaiTienNghi")
                    .Include("ChiTietDatPhongs.LoaiPhong.TienIches.LoaiTienIch")
                    .Include("ChiTietDatDichVus.DichVu.LoaiDichVu")
                    .Include("KhachHang")
                    .FirstOrDefault(dp => dp.DatPhongId == id && dp.MaKhachHang == maKhachHang);

                if (datPhong == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" }, JsonRequestBehavior.AllowGet);
                }

                // Lấy thông tin chi tiết
                var chiTietPhongs = datPhong.ChiTietDatPhongs.Select(ct => new
                {
                    TenLoaiPhong = ct.LoaiPhong?.TenLoai,
                    TenPhong = ct.Phong?.TenPhong,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien,
                    TienNghis = ct.LoaiPhong?.TienNghis.Select(tn => new
                    {
                        TenTienNghi = tn.TenTienNghi,
                        Icon = tn.Icon,
                        LoaiTienNghi = tn.LoaiTienNghi?.TenLoai
                    }).ToList(),
                    TienIches = ct.LoaiPhong?.TienIches.Select(ti => new
                    {
                        TenTienIch = ti.TenTienIch,
                        Icon = ti.Icon,
                        LoaiTienIch = ti.LoaiTienIch?.TenLoai
                    }).ToList()
                }).ToList();

                var chiTietDichVus = datPhong.ChiTietDatDichVus.Select(dv => new
                {
                    TenDichVu = dv.DichVu?.TenDichVu,
                    LoaiDichVu = dv.DichVu?.LoaiDichVu?.TenLoai,
                    SoLuong = dv.SoLuong,
                    DonGia = dv.DonGia,
                    ThanhTien = dv.ThanhTien,
                    Icon = dv.DichVu?.Icon
                }).ToList();

                // Xác định trạng thái
                string trangThaiText = "";
                switch (datPhong.TrangThaiDatPhong)
                {
                    case 0: trangThaiText = "Chờ Xác Nhận"; break;
                    case 1: trangThaiText = "Đã Xác Nhận"; break;
                    case 2: trangThaiText = "Hoàn Thành"; break;
                    case 3: trangThaiText = "Đã Hủy"; break;
                    default: trangThaiText = "Không xác định"; break;
                }

                var result = new
                {
                    success = true,
                    data = new
                    {
                        MaDatPhong = datPhong.MaDatPhong ?? "#DP" + datPhong.DatPhongId,
                        TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
                        TrangThaiText = trangThaiText,
                        NgayNhan = datPhong.NgayNhan?.ToString("dd/MM/yyyy HH:mm"),
                        NgayTra = datPhong.NgayTra?.ToString("dd/MM/yyyy HH:mm"),
                        SoDem = datPhong.SoDem,
                        TongTien = datPhong.TongTien,
                        GhiChu = datPhong.GhiChu,
                        ChiTietPhongs = chiTietPhongs,
                        ChiTietDichVus = chiTietDichVus,
                        // Thông tin liên hệ khách sạn
                        HotelInfo = new
                        {
                            DiaChi = "123 Đường Bờ Biển, TP Paradise",
                            SoDienThoai = "(+84) 123 456 789",
                            Email = "support@serenehorizon.com",
                            MapUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3916.644233957414!2d106.66163457480823!3d10.990203189171902!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3174d1be2495c19d%3A0xafd977d94466ffc2!2zxJDhuqFpIGjhu41jIELDrG5oIETGsMahbmc!5e0!3m2!1svi!2s!4v1761220984026!5m2!1svi!2s"
                        }
                    }
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
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

        [HttpPost]
        public ActionResult HuyDatPhong(int datPhongId)
        {
            try
            {
                // Kiểm tra session
                if (Session["MaKhachHang"] == null)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập lại";
                    return RedirectToAction("LichSu");
                }

                int maKhachHang = (int)Session["MaKhachHang"];

                // Tìm đơn đặt phòng kèm theo chi tiết
                var datPhong = db.DatPhongs
                    .Include("ChiTietDatPhongs")
                    .FirstOrDefault(dp => dp.DatPhongId == datPhongId && dp.MaKhachHang == maKhachHang);

                if (datPhong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn đặt phòng";
                    return RedirectToAction("LichSu");
                }

                // Kiểm tra trạng thái (chỉ cho phép hủy nếu đang ở trạng thái chờ xác nhận hoặc đã xác nhận)
                if (datPhong.TrangThaiDatPhong != 0 && datPhong.TrangThaiDatPhong != 1)
                {
                    TempData["ErrorMessage"] = "Không thể hủy đơn đặt phòng này";
                    return RedirectToAction("LichSu");
                }

                // Cập nhật trạng thái đặt phòng thành 3 (Đã hủy)
                datPhong.TrangThaiDatPhong = 3;
                
                // Cập nhật trạng thái của tất cả phòng liên quan về 0 (trống)
                var chiTietDatPhongs = db.ChiTietDatPhongs
                    .Where(ct => ct.DatPhongId == datPhongId && ct.PhongId.HasValue)
                    .ToList();

                foreach (var chiTiet in chiTietDatPhongs)
                {
                    var phong = db.Phongs.FirstOrDefault(p => p.PhongId == chiTiet.PhongId);
                    if (phong != null)
                    {
                        phong.TrangThaiPhong = 0; // 0 = Trống
                    }
                }

                db.SaveChanges();

                TempData["SuccessMessage"] = "Hủy đặt phòng thành công!";
                return RedirectToAction("LichSu");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("LichSu");
            }
        }

        public ActionResult TaiFileICS(int datPhongId)
        {
            try
            {
                // Kiểm tra session
                if (Session["MaKhachHang"] == null)
                {
                    return RedirectToAction("DangNhap", "Login");
                }

                int maKhachHang = (int)Session["MaKhachHang"];

                // Tìm đơn đặt phòng
                var datPhong = db.DatPhongs
                    .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
                    .Include(dp => dp.KhachHang)
                    .FirstOrDefault(dp => dp.DatPhongId == datPhongId && dp.MaKhachHang == maKhachHang);

                if (datPhong == null)
                {
                    return HttpNotFound();
                }

                // Tạo nội dung file ICS
                var icsContent = GenerateICSContent(datPhong);

                // Trả về file để download
                var fileName = $"DatPhong_{datPhong.MaDatPhong ?? "DP" + datPhong.DatPhongId}.ics";
                return File(System.Text.Encoding.UTF8.GetBytes(icsContent), "text/calendar", fileName);
            }
            catch (Exception)
            {
                return RedirectToAction("LichSu");
            }
        }

        private string GenerateICSContent(DatPhong datPhong)
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyyMMddTHHmmssZ");
            
            // Format ngày giờ cho ICS (phải là UTC hoặc local time với timezone)
            var checkIn = datPhong.NgayNhan ?? DateTime.Now;
            var checkOut = datPhong.NgayTra ?? DateTime.Now.AddDays(1);
            
            var dtStart = checkIn.ToString("yyyyMMddTHHmmss");
            var dtEnd = checkOut.ToString("yyyyMMddTHHmmss");

            // Lấy thông tin phòng
            var roomInfo = "";
            if (datPhong.ChiTietDatPhongs != null && datPhong.ChiTietDatPhongs.Any())
            {
                roomInfo = string.Join(", ", datPhong.ChiTietDatPhongs.Select(ct => 
                    $"{ct.LoaiPhong?.TenLoai ?? "Phòng"} x{ct.SoLuong}"));
            }

            var summary = $"Đặt phòng - The Serene Horizon Hotel";
            var description = $"Mã đặt phòng: {datPhong.MaDatPhong ?? "#DP" + datPhong.DatPhongId}\\n" +
                            $"Phòng: {roomInfo}\\n" +
                            $"Số đêm: {datPhong.SoDem ?? 0}\\n" +
                            $"Tổng tiền: {(datPhong.TongTien ?? 0).ToString("N0")}đ\\n" +
                            $"Khách hàng: {datPhong.KhachHang?.HoVaTen ?? ""}\\n" +
                            $"SĐT: {datPhong.KhachHang?.SoDienThoai ?? ""}";

            var location = "The Serene Horizon Hotel, 123 Đường Bờ Biển, TP Paradise";

            // Tạo UID duy nhất
            var uid = $"{datPhong.DatPhongId}@serenehorizonhotel.com";

            var icsContent = $@"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//The Serene Horizon Hotel//Booking System//VI
CALSCALE:GREGORIAN
METHOD:PUBLISH
BEGIN:VEVENT
UID:{uid}
DTSTAMP:{timestamp}
DTSTART:{dtStart}
DTEND:{dtEnd}
SUMMARY:{summary}
DESCRIPTION:{description}
LOCATION:{location}
STATUS:CONFIRMED
SEQUENCE:0
BEGIN:VALARM
TRIGGER:-PT24H
ACTION:DISPLAY
DESCRIPTION:Nhắc nhở: Ngày mai bạn có lịch nhận phòng tại The Serene Horizon Hotel
END:VALARM
END:VEVENT
END:VCALENDAR";

            return icsContent;
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