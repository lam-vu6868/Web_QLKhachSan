using BCrypt.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_QLKhachSan.Areas;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: Login/DangNhap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(DangNhap model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var db = new DB_QLKhachSanEntities())
            {
                var taiKhoan = db.TaiKhoans.FirstOrDefault(k =>
                    k.TenDangNhap == model.TenDangNhapHoacEmail ||
                    k.Email == model.TenDangNhapHoacEmail);

                if (taiKhoan == null || !BCrypt.Net.BCrypt.Verify(model.MatKhau, taiKhoan.MatKhauHash))
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    return View(model);
                }

                var khachHang = db.KhachHangs.Find(taiKhoan.MaKhachHang);
                if (khachHang == null)
                {
                    ViewBag.ErrorMessage = "Tài khoản không liên kết với khách hàng.";
                    return View(model);
                }

                // kiểm tra xem có token trong db ko
                var ttToken = db.TokenLogins.FirstOrDefault(t => t.MaTaiKhoan == taiKhoan.TaiKhoanId); // Sửa: Dùng FirstOrDefault và so sánh MaTaiKhoan
                if (ttToken != null)
                {
                    ViewBag.ErrorMessage = "Tài khoản của bạn đang được đăng nhập trên trình duyệt khác vui lòng thử lại sau!";
                    return View(model);
                }

                // Xử lý Remember Me
                if (model.GhiNhoDangNhap)
                {
                    // 1. TẠO AUTH COOKIE (Persistent)
                    FormsAuthentication.SetAuthCookie(taiKhoan.TenDangNhap, true);

                    // 2. TẠO REMEMBER ME TOKEN
                    string Matoken = Guid.NewGuid().ToString("N");
                    var token = new TokenLogin()
                    {
                        MaTaiKhoan = taiKhoan.TaiKhoanId,
                        NgayTao = DateTime.Now,
                        NgayHetHan = DateTime.Now.AddDays(30),
                        token = Matoken
                    };
                    db.TokenLogins.Add(token);
                    db.SaveChanges(); // <-- Lưu token vào DB

                    // 3. TẠO REMEMBER ME COOKIE
                    var cookie = new HttpCookie("RememberMeToken", Matoken)
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true,
                        Secure = FormsAuthentication.RequireSSL,
                        Path = FormsAuthentication.FormsCookiePath
                    };
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    // 1. TẠO AUTH COOKIE (Session)
                    FormsAuthentication.SetAuthCookie(taiKhoan.TenDangNhap, false);

                    // 2. (Không làm gì cả, không tạo token)
                }

           

                // =================================================================
                // SỬA LỖI: ĐƯA LOGIC TẠO SESSION RA NGOÀI
                // Logic này phải chạy cho cả 2 trường hợp (if và else)
                // =================================================================
                Session["MaKhachHang"] = khachHang.MaKhachHang;
                Session["HoVaTen"] = khachHang.HoVaTen;
                Session["Email"] = khachHang.Email;
                Session["TenDangNhap"] = khachHang.TenDangNhap;

                // xem người dùng này là ai
                if (taiKhoan.LoaiTaiKhoan == 1)
                {
                    // chuyển đến đến trang admin
                    return RedirectToAction("Index", "DashboardAdmin", new { area = "Admin" });
                }
                else if (taiKhoan.LoaiTaiKhoan == 2)
                {
                    // chuyển đến đến trang nhân viên
                    return RedirectToAction("Index", "DashboardNhanVien", new { area = "NhanVien" });
                }
             

                TempData["SuccessMessage"] = $"Chào mừng {khachHang.HoVaTen} đã đến với Serene Horizon !";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Login/DangKy
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: Login/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DangKy(DangKy model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var db = new DB_QLKhachSanEntities())
            {
                if (db.KhachHangs.Any(x => x.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống!");
                    return View(model);
                }

                if (db.KhachHangs.Any(x => x.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã được sử dụng!");
                    return View(model);
                }

                var khachHang = new KhachHang
                {
                    HoVaTen = model.HoVaTen,
                    TenDangNhap = model.TenDangNhap,
                    Email = model.Email,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    DaDongYDieuKhoan = model.DaDongYDieuKhoan,
                    NgayTao = DateTime.Now
                };
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                var otp = new Random().Next(100000, 999999).ToString();
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = model.TenDangNhap,
                    MatKhauHash = khachHang.MatKhau,
                    LoaiTaiKhoan = 3,
                    MaKhachHang = khachHang.MaKhachHang,
                    DaKichHoat = false,
                    NgayTao = DateTime.Now,
                    OTPCode = otp,
                    OTPExpires = DateTime.Now.AddMinutes(5),
                    Email = model.Email
                };
                db.TaiKhoans.Add(taiKhoan);
                db.SaveChanges();

                TempData["OTPCode"] = otp;
                TempData["Email"] = model.Email;

                await GuiEmailXacThuc(model.Email, model.HoVaTen, otp);

                ViewBag.Email = model.Email;
                ViewBag.Action = "XacThucEmailDangKy"; // ✅ Set action cho form POST
                ViewBag.ResendAction = "ResendOTP"; // ✅ Set action cho AJAX resend
                ViewBag.BackAction = "DangKy";
                ViewBag.BackText = "đăng ký";
                ViewBag.PageTitle = "Xác Thực Tài Khoản - Đăng Ký"; // ✅ Tiêu đề riêng
                return View("XacThucEmail");
            }
        }


        // POST: Login/ResendOTP - Gửi lại OTP đăng ký
        [HttpPost]
        public async Task<JsonResult> ResendOTP(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return Json(new { success = false, message = "Email không hợp lệ" });

                using (var db = new DB_QLKhachSanEntities())
                {
                    var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == email);
                    if (khachHang == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản với email này" });

                    var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == khachHang.TenDangNhap && !t.DaKichHoat);
                    if (taiKhoan == null)
                        return Json(new { success = false, message = "Tài khoản đã được kích hoạt hoặc không tồn tại" });

                    var newOTP = new Random().Next(100000, 999999).ToString();
                    taiKhoan.OTPCode = newOTP;
                    taiKhoan.OTPExpires = DateTime.Now.AddMinutes(5);
                    db.SaveChanges();

                    TempData["OTPCode"] = newOTP;
                    TempData["Email"] = email;
                    TempData.Keep("OTPCode");
                    TempData.Keep("Email");

                    bool emailSent = await GuiEmailXacThuc(email, khachHang.HoVaTen, newOTP, true);

                    return Json(new
                    {
                        success = emailSent,
                        message = emailSent ? "Mã OTP mới đã được gửi đến email của bạn!" : "Không thể gửi email. Vui lòng thử lại sau."
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ResendOTP] Lỗi: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }


        // GET: Login/DangXuat
        public ActionResult DangXuat()
        {
            // Bước 1: Xóa token Remember Me trong database nếu có
            var rememberCookie = Request.Cookies["RememberMeToken"];
            if (rememberCookie != null && !string.IsNullOrEmpty(rememberCookie.Value))
            {
                try
                {
                    using (var db = new DB_QLKhachSanEntities())
                    {
                        // Tìm và xóa token trong database
                        var token = db.TokenLogins.FirstOrDefault(t => t.token == rememberCookie.Value);
                        if (token != null)
                        {
                            db.TokenLogins.Remove(token);
                            db.SaveChanges();
                            System.Diagnostics.Debug.WriteLine($"[LOGOUT] Đã xóa token: {token.TokenId}");
                        }
                    }

                    // Xóa cookie RememberMeToken trên browser
                    var expiredCookie = new HttpCookie("RememberMeToken")
                    {
                        Expires = DateTime.Now.AddDays(-1) // Set ngày quá khứ để xóa
                    };
                    Response.Cookies.Add(expiredCookie);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[LOGOUT ERROR] {ex.Message}");
                }
            }

            // Bước 2: Xóa Session
            Session.Clear();
            Session.Abandon();

            // Bước 3: Đăng xuất FormsAuthentication (xóa cookie .ASPXAUTH)
            FormsAuthentication.SignOut();

            TempData["SuccessMessage"] = "Đăng xuất thành công! Hẹn gặp lại bạn.";
            return RedirectToAction("DangNhap");
        }

        // ============================================================================
        // ĐĂNG KÝ - XÁC THỰC EMAIL
        // ============================================================================

        // GET: Login/XacThucEmailDangKy
        public ActionResult XacThucEmailDangKy()
        {
            return View("XacThucEmail");
        }

        // POST: Login/XacThucEmailDangKy - Xác thực OTP đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacThucEmailDangKy(string OTPCode, string Email)
        {
            string savedOTP = TempData["OTPCode"] as string;
            string savedEmail = TempData["Email"] as string;

            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(savedEmail))
            {
                ViewBag.ErrorMessage = "Phiên xác thực đã hết hạn. Vui lòng đăng ký lại.";
                ViewBag.Email = Email;
                return View("XacThucEmail");
            }

            if (OTPCode != savedOTP)
            {
                ViewBag.ErrorMessage = "Mã OTP không chính xác. Vui lòng thử lại.";
                ViewBag.Email = savedEmail;
                TempData["OTPCode"] = savedOTP;
                TempData["Email"] = savedEmail;
                return View("XacThucEmail");
            }

            using (var db = new DB_QLKhachSanEntities())
            {
                var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.OTPCode == savedOTP && t.Email == savedEmail);

                if (taiKhoan == null || taiKhoan.OTPExpires < DateTime.Now)
                {
                    ViewBag.ErrorMessage = "Mã OTP đã hết hạn. Vui lòng đăng ký lại.";
                    ViewBag.Email = savedEmail;
                    return View("XacThucEmail");
                }

                // ✅ Kích hoạt tài khoản và xóa OTP
                taiKhoan.DaKichHoat = true;
                taiKhoan.OTPCode = null;
                taiKhoan.OTPExpires = null;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Xác thực thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToAction("DangNhap");
            }
        }

        // ============================================================================
        // QUÊN MẬT KHẨU
        // ============================================================================

        // GET: Login/QuenMatKhau
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        // POST: Login/QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuenMatKhau(QuenMatKhau model)
        {
            // ✅ Validate Model
            if (!ModelState.IsValid)
{
  return View(model);
         }

    using (var db = new DB_QLKhachSanEntities())
   {
       var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == model.Email);
    if (khachHang == null)
 {
    ModelState.AddModelError("Email", "Không tìm thấy tài khoản với email này.");
                  return View(model);
                }

  var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.Email == model.Email);
          if (taiKhoan == null)
        {
 ModelState.AddModelError("Email", "Không tìm thấy tài khoản.");
     return View(model);
             }

     var otp = new Random().Next(100000, 999999).ToString();
                taiKhoan.OTPCode = otp;
           taiKhoan.OTPExpires = DateTime.Now.AddMinutes(5);
db.SaveChanges();

   TempData["OTPCode_QuenMatKhau"] = otp;
          TempData["Email_QuenMatKhau"] = model.Email;

       await GuiEmailQuenMatKhau(model.Email, khachHang.HoVaTen, otp);

        // ✅ SET VIEWBAG để dùng chung View XacThucEmail
    ViewBag.Email = model.Email;
        ViewBag.Action = "XacThucQuenMatKhau"; // POST action
        ViewBag.ResendAction = "ResendOTPQuenMatKhau"; // AJAX resend
      ViewBag.BackAction = "QuenMatKhau";
                ViewBag.BackText = "quên mật khẩu";
 ViewBag.PageTitle = "Xác Thực Mã OTP - Quên Mật Khẩu"; // ✅ Tiêu đề riêng

 return View("XacThucEmail"); // ✅ Dùng chung View
    }
        }

        // GET: Login/XacThucQuenMatKhau
        public ActionResult XacThucQuenMatKhau()
        {
       // ✅ Redirect về XacThucEmail với ViewBag phù hợp
   ViewBag.Action = "XacThucQuenMatKhau";
            ViewBag.ResendAction = "ResendOTPQuenMatKhau";
     ViewBag.BackAction = "QuenMatKhau";
            ViewBag.BackText = "quên mật khẩu";
     ViewBag.PageTitle = "Xác Thực Mã OTP - Quên Mật Khẩu";
          return View("XacThucEmail");
        }
        // POST: Login/XacThucQuenMatKhau - Xác thực OTP quên mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacThucQuenMatKhau(string OTPCode, string Email)
        {
            string savedOTP = TempData["OTPCode_QuenMatKhau"] as string;
            string savedEmail = TempData["Email_QuenMatKhau"] as string;

            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(savedEmail))
            {
                ViewBag.ErrorMessage = "Phiên xác thực đã hết hạn. Vui lòng thử lại.";
                ViewBag.Email = Email;
                // ✅ Set ViewBag cho View XacThucEmail
                ViewBag.Action = "XacThucQuenMatKhau";
                ViewBag.ResendAction = "ResendOTPQuenMatKhau";
                ViewBag.BackAction = "QuenMatKhau";
                ViewBag.BackText = "quên mật khẩu";
                ViewBag.PageTitle = "Xác Thực Mã OTP - Quên Mật Khẩu";
                return View("XacThucEmail"); // ✅ ĐÚNG
            }

            if (OTPCode != savedOTP)
            {
                ViewBag.ErrorMessage = "Mã OTP không chính xác. Vui lòng thử lại.";
                ViewBag.Email = savedEmail;
                TempData["OTPCode_QuenMatKhau"] = savedOTP;
                TempData["Email_QuenMatKhau"] = savedEmail;
                // ✅ Set ViewBag cho View XacThucEmail
                ViewBag.Action = "XacThucQuenMatKhau";
                ViewBag.ResendAction = "ResendOTPQuenMatKhau";
                ViewBag.BackAction = "QuenMatKhau";
                ViewBag.BackText = "quên mật khẩu";
                ViewBag.PageTitle = "Xác Thực Mã OTP - Quên Mật Khẩu";
                return View("XacThucEmail"); // ✅ ĐÚNG
            }

            using (var db = new DB_QLKhachSanEntities())
            {
                var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.OTPCode == savedOTP && t.Email == savedEmail);

                if (taiKhoan == null || taiKhoan.OTPExpires < DateTime.Now)
                {
                    ViewBag.ErrorMessage = "Mã OTP đã hết hạn. Vui lòng thử lại.";
                    ViewBag.Email = savedEmail;
                    // ✅ Set ViewBag cho View XacThucEmail
                    ViewBag.Action = "XacThucQuenMatKhau";
                    ViewBag.ResendAction = "ResendOTPQuenMatKhau";
                    ViewBag.BackAction = "QuenMatKhau";
                    ViewBag.BackText = "quên mật khẩu";
                    ViewBag.PageTitle = "Xác Thực Mã OTP - Quên Mật Khẩu";
                    return View("XacThucEmail"); // ✅ ĐÚNG
                }

                // ✅ Xóa OTP sau khi xác thực thành công
                taiKhoan.OTPCode = null;
                taiKhoan.OTPExpires = null;
                db.SaveChanges();

                // Chuyển hướng đến trang đặt lại mật khẩu
                TempData["Email_QuenMatKhau_Reset"] = savedEmail;
                return RedirectToAction("DatLaiMatKhau");
            }
        }

        // GET: Login/DatLaiMatKhau
        public ActionResult DatLaiMatKhau()
        {
            string email = TempData["Email_QuenMatKhau_Reset"] as string;
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Phiên đặt lại mật khẩu đã hết hạn. Vui lòng thử lại.";
                return RedirectToAction("QuenMatKhau");
            }

            ViewBag.Email = email;
            TempData.Keep("Email_QuenMatKhau_Reset"); // ✅ Giữ lại cho POST
            return View();
        }

        // POST: Login/DatLaiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DatLaiMatKhau(DatLaiMatKhau model)
        {
            // Lấy email từ TempData
            string email = TempData["Email_QuenMatKhau_Reset"] as string;

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Phiên đặt lại mật khẩu đã hết hạn. Vui lòng thử lại.";
                return RedirectToAction("QuenMatKhau");
            }

            // Gán email vào model
            model.Email = email;

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.Email = email;
                TempData["Email_QuenMatKhau_Reset"] = email; // Giữ lại
                return View(model);
            }

            using (var db = new DB_QLKhachSanEntities())
            {
                var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.Email == email);
                if (taiKhoan == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                    ViewBag.Email = email;
                    TempData["Email_QuenMatKhau_Reset"] = email;
                    return View(model);
                }

                // Cập nhật mật khẩu mới
                taiKhoan.MatKhauHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
                taiKhoan.OTPCode = null;
                taiKhoan.OTPExpires = null;

                // Cập nhật mật khẩu cho bảng KhachHang
                var khachHang = db.KhachHangs.Find(taiKhoan.MaKhachHang);
                if (khachHang != null)
                {
                    khachHang.MatKhau = taiKhoan.MatKhauHash;
                }

                db.SaveChanges();

                // ✅ Gửi email thông báo đặt lại mật khẩu thành công
                try
                {
                    string hoVaTen = khachHang?.HoVaTen ?? "Khách hàng";
                    bool emailSent = await GuiEmailThongBaoDatLaiMatKhauThanhCong(email, hoVaTen);

                    if (emailSent)
                    {
                        TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công!";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.";
                        TempData["EmailWarning"] = "Lưu ý: Không thể gửi email xác nhận, nhưng mật khẩu của bạn đã được cập nhật thành công.";
                        System.Diagnostics.Debug.WriteLine($"[WARNING] Không thể gửi email xác nhận đến {email}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERROR] Lỗi khi gửi email thông báo: {ex.Message}");
                    TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.";
                }

                return RedirectToAction("DangNhap");
            }
        }

        // gửi mail khi đặt mật khẩu thành công
        // POST: Login/ResendOTPQuenMatKhau - Gửi lại OTP cho quên mật khẩu
        [HttpPost]
        public async Task<JsonResult> ResendOTPQuenMatKhau(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return Json(new { success = false, message = "Email không hợp lệ" });

                using (var db = new DB_QLKhachSanEntities())
                {
                    var khachHang = db.KhachHangs.FirstOrDefault(k => k.Email == email);
                    if (khachHang == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản với email này" });

                    var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.Email == email);
                    if (taiKhoan == null)
                        return Json(new { success = false, message = "Không tìm thấy tài khoản" });

                    var newOTP = new Random().Next(100000, 999999).ToString();
                    taiKhoan.OTPCode = newOTP;
                    taiKhoan.OTPExpires = DateTime.Now.AddMinutes(5);
                    db.SaveChanges();

                    TempData["OTPCode_QuenMatKhau"] = newOTP;
                    TempData["Email_QuenMatKhau"] = email;
                    TempData.Keep("OTPCode_QuenMatKhau");
                    TempData.Keep("Email_QuenMatKhau");

                    bool emailSent = await GuiEmailQuenMatKhau(email, khachHang.HoVaTen, newOTP, true);

                    return Json(new
                    {
                        success = emailSent,
                        message = emailSent ? "Mã OTP mới đã được gửi đến email của bạn!" : "Không thể gửi email. Vui lòng thử lại sau."
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ResendOTPQuenMatKhau] Lỗi: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }




        #region Helper Methods

        private async Task<bool> GuiEmailXacThuc(string email, string hoVaTen, string otp, bool isResend = false)
        {
            try
            {
                var emailService = new MailKitEmailService();
                string subject = isResend ? "Mã OTP mới - Xác thực tài khoản Serene Horizon" : "Xác thực email đăng ký tài khoản tại Serene Horizon";
                string tieuDe = isResend ? "Mã OTP Mới" : "Xác Thực Tài Khoản";
                string noiDung = isResend ? "Bạn đã yêu cầu gửi lại mã xác thực. Vui lòng sử dụng mã mới dưới đây:" : "Cảm ơn bạn đã đăng ký tài khoản tại Serene Horizon. Vui lòng sử dụng mã dưới đây để hoàn tất quá trình xác thực:";

                string body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{subject}</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 20px auto; padding: 20px; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05); border-top: 5px solid #007bff;'>
        <h2 style='color: #007bff; border-bottom: 2px solid #eee; padding-bottom: 10px;'>{tieuDe}</h2>
        <p><strong>Chào {hoVaTen},</strong></p>
        <p>{noiDung}</p>
        <div style='text-align: center; margin: 30px 0; padding: 15px; background-color: #e9f5ff; border: 1px dashed #007bff; border-radius: 4px;'>
            <p style='font-size: 1.1rem; color: #555; margin-bottom: 5px;'>Mã xác thực của bạn (OTP):</p>
            <p style='font-size: 2.5rem; font-weight: bold; color: #d9534f; margin: 0;'>{otp}</p>
        </div>
        <p style='color: #888; font-size: 0.9rem;'>Mã này chỉ có hiệu lực trong vòng <strong>5 phút</strong>. Vui lòng không chia sẻ mã này với bất kỳ ai.</p>
        <p>Trân trọng,<br/><strong>Serene Horizon Team</strong></p>
        <div style='margin-top: 20px; padding-top: 10px; border-top: 1px solid #eee; text-align: center;'>
            <p style='font-size: 0.8rem; color: #aaa;'>&copy; {DateTime.Now.Year} Serene Horizon. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                bool emailSent = await emailService.SendEmailAsync(email, subject, body);

                TempData[emailSent ? "EmailSuccess" : "EmailError"] = emailSent
                    ? "Mã xác thực đã được gửi đến email của bạn!"
                    : "Không thể gửi email xác thực. Vui lòng kiểm tra lại email hoặc liên hệ quản trị viên.";

                return emailSent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR] {ex.Message}");
                TempData["EmailError"] = $"Lỗi khi gửi email: {ex.Message}";
                return false;
            }
        }


        private async Task<bool> GuiEmailQuenMatKhau(string email, string hoVaTen, string otp, bool isResend = false)
        {
            try
            {
                var emailService = new MailKitEmailService();
                string subject = isResend ? "Mã OTP mới - Đặt lại mật khẩu Serene Horizon" : "Đặt lại mật khẩu - Serene Horizon";
                string tieuDe = isResend ? "Mã OTP Mới" : "Đặt Lại Mật Khẩu";
                string noiDung = isResend ? "Bạn đã yêu cầu gửi lại mã xác thực. Vui lòng sử dụng mã mới dưới đây:" : "Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã dưới đây để xác thực:";

                string body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{subject}</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f4f4f4;'>
 <div style='max-width: 600px; margin: 20px auto; padding: 20px; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05); border-top: 5px solid #ff6b6b;'>
    <h2 style='color: #ff6b6b; border-bottom: 2px solid #eee; padding-bottom: 10px;'>{tieuDe}</h2>
    <p><strong>Chào {hoVaTen},</strong></p>
    <p>{noiDung}</p>
    <div style='text-align: center; margin: 30px 0; padding: 15px; background-color: #fff5f5; border: 1px dashed #ff6b6b; border-radius: 4px;'>
     <p style='font-size: 1.1rem; color: #555; margin-bottom: 5px;'>Mã xác thực của bạn (OTP):</p>
    <p style='font-size: 2.5rem; font-weight: bold; color: #d9534f; margin: 0;'>{otp}</p>
 </div>
<p style='color: #888; font-size: 0.9rem;'>Mã này chỉ có hiệu lực trong vòng <strong>5 phút</strong>. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
    <p>Trân trọng,<br/><strong>Serene Horizon Team</strong></p>
 <div style='margin-top: 20px; padding-top: 10px; border-top: 1px solid #eee; text-align: center;'>
 <p style='font-size: 0.8rem; color: #aaa;'>&copy; {DateTime.Now.Year} Serene Horizon. All rights reserved.</p>
    </div>
 </div>
</body>
</html>";

                bool emailSent = await emailService.SendEmailAsync(email, subject, body);

                TempData[emailSent ? "EmailSuccess" : "EmailError"] = emailSent
                    ? "Mã xác thực đã được gửi đến email của bạn!"
                    : "Không thể gửi email. Vui lòng kiểm tra lại email hoặc liên hệ quản trị viên.";

                return emailSent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR - QUENMATKHAU] {ex.Message}");
                TempData["EmailError"] = $"Lỗi khi gửi email: {ex.Message}";
                return false;
            }
        }

        private async Task<bool> GuiEmailThongBaoDatLaiMatKhauThanhCong(string email, string hoVaTen)
        {
            try
            {
                var emailService = new MailKitEmailService();
                string subject = "Đặt lại mật khẩu thành công - Serene Horizon";

                string body = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
  <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{subject}</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f4f4f4;'>
 <div style='max-width: 600px; margin: 20px auto; padding: 20px; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05); border-top: 5px solid #28a745;'>
        <h2 style='color: #28a745; border-bottom: 2px solid #eee; padding-bottom: 10px;'>
  <i style='font-size: 1.2em;'>✓</i> Đặt Lại Mật Khẩu Thành Công
        </h2>
        <p><strong>Chào {hoVaTen},</strong></p>
  <p>Mật khẩu của bạn đã được đặt lại thành công vào lúc <strong>{DateTime.Now:HH:mm:ss dd/MM/yyyy}</strong>.</p>
  
        <div style='margin: 30px 0; padding: 20px; background-color: #d4edda; border-left: 4px solid #28a745; border-radius: 4px;'>
            <p style='margin: 0; color: #155724; font-size: 1rem;'>
        <strong>✓ Thông báo bảo mật:</strong><br/>
       Tài khoản của bạn đã được cập nhật mật khẩu mới. Bạn có thể đăng nhập ngay bây giờ với mật khẩu mới.
       </p>
 </div>

        <div style='margin: 20px 0; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;'>
   <p style='margin: 0; color: #856404; font-size: 0.95rem;'>
   <strong>⚠ Lưu ý:</strong><br/>
  Nếu bạn <strong>KHÔNG</strong> thực hiện thay đổi này, vui lòng liên hệ với chúng tôi ngay lập tức để bảo vệ tài khoản của bạn.
            </p>
        </div>

   <div style='text-align: center; margin: 30px 0;'>
            <p style='font-size: 0.9rem; color: #666; margin-bottom: 15px;'>Bạn có thể đăng nhập tại:</p>
  <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #28a745; color: #ffffff; text-decoration: none; border-radius: 5px; font-weight: bold;'>
Đăng Nhập Ngay
</a>
        </div>

        <p style='color: #666; font-size: 0.9rem; margin-top: 30px;'>
   <strong>Mẹo bảo mật:</strong>
        </p>
        <ul style='color: #666; font-size: 0.9rem; line-height: 1.8;'>
            <li>Không chia sẻ mật khẩu với bất kỳ ai</li>
            <li>Sử dụng mật khẩu mạnh (chữ hoa, chữ thường, số, ký tự đặc biệt)</li>
            <li>Thay đổi mật khẩu định kỳ</li>
   <li>Không sử dụng cùng một mật khẩu cho nhiều tài khoản</li>
  </ul>

        <p style='margin-top: 30px;'>Trân trọng,<br/><strong>Serene Horizon Team</strong></p>
        
        <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; text-align: center;'>
            <p style='font-size: 0.8rem; color: #aaa; margin: 5px 0;'>
             Email: support@serenehorizon.com | Hotline: 1900-xxxx
            </p>
            <p style='font-size: 0.8rem; color: #aaa; margin: 5px 0;'>
           &copy; {DateTime.Now.Year} Serene Horizon. All rights reserved.
            </p>
        </div>
    </div>
</body>
</html>";

                bool emailSent = await emailService.SendEmailAsync(email, subject, body);

                if (emailSent)
                {
                    System.Diagnostics.Debug.WriteLine($"[EMAIL SUCCESS] Đã gửi email thông báo đặt lại mật khẩu thành công đến: {email}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[EMAIL WARNING] Không thể gửi email thông báo đến: {email}");
                }

                return emailSent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR - THONGBAO] {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}