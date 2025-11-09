using BCrypt.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.DangNhapNV.ViewModels;

namespace Web_QLKhachSan.Areas.DangNhapNV.Controllers
{
    /// <summary>
    /// Controller xử lý đăng nhập/đăng xuất cho Nhân viên
    /// </summary>
    public class DangNhapNVController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: DangNhapNV/DangNhapNV/DangNhap
        public ActionResult DangNhap(string returnUrl)
 {
            // ✅ Nếu đã có Session, redirect về trang tương ứng
    if (Session["NhanVienId"] != null)
    {
        return RedirectToNhanVienArea();
    }

    // ✅ KIỂM TRA REMEMBER ME COOKIE
    var rememberCookie = Request.Cookies["NhanVienRemember"];
    if (rememberCookie != null && !string.IsNullOrEmpty(rememberCookie.Values["Token"]))
    {
        string cookieToken = rememberCookie.Values["Token"];
     
      try
        {
          // Tìm token trong database
         var tokenRecord = db.TokenLogins
    .FirstOrDefault(t => t.token == cookieToken && t.NgayHetHan > DateTime.Now);

 if (tokenRecord != null && tokenRecord.MaTaiKhoan > 0)
            {
      // Lấy thông tin TaiKhoan
        var taiKhoan = db.TaiKhoans.Find(tokenRecord.MaTaiKhoan);
       
       if (taiKhoan != null && taiKhoan.MaNhanVien.HasValue)
         {
                    // Lấy thông tin Nhân viên
           var nhanVien = db.NhanViens.Find(taiKhoan.MaNhanVien.Value);
            
   if (nhanVien != null && nhanVien.DaHoatDong && nhanVien.TrangThaiLamViec == 1)
        {
    // ✅ TỰ ĐỘNG ĐĂNG NHẬP
      Session["NhanVienId"] = nhanVien.NhanVienId;
    Session["MaNV"] = nhanVien.MaNV;
   Session["HoVaTen"] = nhanVien.HoVaTen;
 Session["Email"] = nhanVien.Email;
       Session["VaiTro"] = nhanVien.VaiTro;
     Session["HinhAnh"] = nhanVien.HinhAnh;
     Session["DangNhapLuc"] = DateTime.Now;

       System.Diagnostics.Debug.WriteLine($"[AUTO LOGIN] {nhanVien.HoVaTen} - Remember Me");

   // Redirect về trang đã lưu hoặc trang chính
     if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
     {
        return Redirect(returnUrl);
  }
   return RedirectToNhanVienArea();
        }
           }
}
        else
          {
          // Token không hợp lệ hoặc hết hạn, xóa cookie
        Response.Cookies["NhanVienRemember"].Expires = DateTime.Now.AddDays(-1);
   }
     }
  catch (Exception ex)
    {
            System.Diagnostics.Debug.WriteLine($"[ERROR - Remember Me] {ex.Message}");
          // Xóa cookie nếu có lỗi
Response.Cookies["NhanVienRemember"].Expires = DateTime.Now.AddDays(-1);
   }
    }

    ViewBag.ReturnUrl = returnUrl;
    return View();
}

// POST: DangNhapNV/DangNhapNV/DangNhap
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult DangNhap(DangNhapNVViewModel model, string returnUrl)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return View(model);
  }

        // Tìm nhân viên theo Email hoặc MaNV
var nhanVien = db.NhanViens
       .FirstOrDefault(nv =>
 (nv.Email == model.TenDangNhap || nv.MaNV == model.TenDangNhap)
     && nv.DaHoatDong == true
    );

        if (nhanVien == null)
        {
    TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
    return View(model);
 }

        // Kiểm tra trạng thái làm việc
        if (nhanVien.TrangThaiLamViec == 0)
        {
 TempData["ErrorMessage"] = "Tài khoản của bạn đã ngừng hoạt động. Vui lòng liên hệ quản trị viên!";
            return View(model);
        }

        // Xác thực mật khẩu
     bool isPasswordValid = false;

     try
  {
         // Thử verify với BCrypt (nếu đã hash)
      isPasswordValid = BCrypt.Net.BCrypt.Verify(model.MatKhau, nhanVien.MatKhau);
 }
        catch
        {
    // Nếu lỗi, có thể mật khẩu chưa hash, so sánh trực tiếp
isPasswordValid = nhanVien.MatKhau == model.MatKhau;

  // Nếu đúng, hash lại mật khẩu
    if (isPasswordValid)
            {
      nhanVien.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
        db.SaveChanges();
        }
        }

      if (!isPasswordValid)
    {
      TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View(model);
        }

        // ✅ Đăng nhập thành công - Lưu Session
        Session["NhanVienId"] = nhanVien.NhanVienId;
        Session["MaNV"] = nhanVien.MaNV;
        Session["HoVaTen"] = nhanVien.HoVaTen;
        Session["Email"] = nhanVien.Email;
        Session["VaiTro"] = nhanVien.VaiTro;
        Session["HinhAnh"] = nhanVien.HinhAnh;
        Session["DangNhapLuc"] = DateTime.Now;

        // ✅ GHI NHỚ ĐĂNG NHẬP (LƯU VÀO DATABASE)
if (model.GhiNho)
        {
    // Tìm hoặc tạo TaiKhoan cho Nhân viên
            var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.MaNhanVien == nhanVien.NhanVienId);
      
  if (taiKhoan == null)
            {
 taiKhoan = new TaiKhoan
       {
        TenDangNhap = nhanVien.MaNV,
      MatKhauHash = nhanVien.MatKhau,
             LoaiTaiKhoan = 2, // 2 = Nhân viên
       MaNhanVien = nhanVien.NhanVienId,
      Email = nhanVien.Email,
           DaKichHoat = true,
          NgayTao = DateTime.Now
       };
   db.TaiKhoans.Add(taiKhoan);
    db.SaveChanges();
}

          // Tạo token duy nhất
    string token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

// Lưu token vào database
            var tokenLogin = new TokenLogin
            {
     MaTaiKhoan = taiKhoan.TaiKhoanId,
                token = token,
    NgayTao = DateTime.Now,
     NgayHetHan = DateTime.Now.AddDays(30)
   };
 db.TokenLogins.Add(tokenLogin);
            db.SaveChanges();

  // Tạo cookie chứa token
     HttpCookie cookie = new HttpCookie("NhanVienRemember");
   cookie.Values["MaNV"] = nhanVien.MaNV;
       cookie.Values["Token"] = token;
          cookie.Expires = DateTime.Now.AddDays(30);
  cookie.HttpOnly = true; // Bảo mật: không cho JavaScript truy cập
            cookie.Secure = Request.IsSecureConnection; // Chỉ gửi qua HTTPS nếu có
   Response.Cookies.Add(cookie);

   System.Diagnostics.Debug.WriteLine($"[REMEMBER ME] Token created for {nhanVien.MaNV}");
        }

        // Log
        System.Diagnostics.Debug.WriteLine($"[LOGIN SUCCESS] {nhanVien.HoVaTen} ({nhanVien.MaNV}) - {DateTime.Now}");

        TempData["SuccessMessage"] = $"Chào mừng {nhanVien.HoVaTen}!";

        // Redirect
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
    return Redirect(returnUrl);
        }

   return RedirectToNhanVienArea();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR - DangNhap] {ex.Message}");
        TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
        return View(model);
    }
}

// GET: DangNhapNV/DangNhapNV/DangXuat
public ActionResult DangXuat()
{
    try
    {
    // Log
     string tenNhanVien = Session["HoVaTen"]?.ToString() ?? "Unknown";
     System.Diagnostics.Debug.WriteLine($"[LOGOUT] {tenNhanVien} - {DateTime.Now}");

        // ✅ XÓA TOKEN REMEMBER ME TRONG DATABASE
        var rememberCookie = Request.Cookies["NhanVienRemember"];
        if (rememberCookie != null && !string.IsNullOrEmpty(rememberCookie.Values["Token"]))
  {
     string token = rememberCookie.Values["Token"];
       var tokenRecord = db.TokenLogins.FirstOrDefault(t => t.token == token);
            if (tokenRecord != null)
            {
      db.TokenLogins.Remove(tokenRecord);
     db.SaveChanges();
           System.Diagnostics.Debug.WriteLine($"[LOGOUT] Đã xóa token: {tokenRecord.TokenId}");
     }
        }

 // Xóa Session
 Session.Clear();
      Session.Abandon();

        // Xóa Cookie
     if (Request.Cookies["NhanVienRemember"] != null)
        {
   HttpCookie cookie = new HttpCookie("NhanVienRemember");
     cookie.Expires = DateTime.Now.AddDays(-1);
Response.Cookies.Add(cookie);
      }

        TempData["SuccessMessage"] = "Đăng xuất thành công!";
        return RedirectToAction("DangNhap");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR - DangXuat] {ex.Message}");
        return RedirectToAction("DangNhap");
    }
}

// ==================== QUÊN MẬT KHẨU ====================

// GET: DangNhapNV/DangNhapNV/QuenMatKhau
   public ActionResult QuenMatKhau()
        {
  return View();
        }

        // POST: DangNhapNV/DangNhapNV/QuenMatKhau
   [HttpPost]
  [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuenMatKhau(string email)
        {
            try
       {
          if (string.IsNullOrWhiteSpace(email))
                {
            TempData["ErrorMessage"] = "Vui lòng nhập email!";
      return View();
    }

     var nhanVien = db.NhanViens.FirstOrDefault(nv => nv.Email == email && nv.DaHoatDong);

          if (nhanVien == null)
     {
       TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống!";
    return View();
        }

 // Tạo OTP 6 số
     var otp = new Random().Next(100000, 999999).ToString();

 // Lưu OTP vào Session
       Session["OTP_QuenMatKhau"] = otp;
        Session["Email_QuenMatKhau"] = email;
            Session["OTP_Expiry"] = DateTime.Now.AddMinutes(5);

                // Gửi email
           bool emailSent = await GuiEmailQuenMatKhau(email, nhanVien.HoVaTen, otp);

          if (!emailSent)
       {
          TempData["ErrorMessage"] = "Không thể gửi email. Vui lòng thử lại sau!";
             return View();
      }

    TempData["SuccessMessage"] = "Mã OTP đã được gửi đến email của bạn!";
      return RedirectToAction("XacThucOTP");
 }
      catch (Exception ex)
      {
  System.Diagnostics.Debug.WriteLine($"[ERROR - QuenMatKhau] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra. Vui lòng thử lại!";
         return View();
            }
        }

        // GET: DangNhapNV/DangNhapNV/XacThucOTP
        public ActionResult XacThucOTP()
        {
   if (Session["Email_QuenMatKhau"] == null)
            {
       TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn!";
     return RedirectToAction("QuenMatKhau");
       }

            ViewBag.Email = Session["Email_QuenMatKhau"];
   return View();
    }

        // POST: DangNhapNV/DangNhapNV/XacThucOTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XacThucOTP(string otp)
        {
            try
 {
           string savedOTP = Session["OTP_QuenMatKhau"] as string;
           DateTime? otpExpiry = Session["OTP_Expiry"] as DateTime?;
    string email = Session["Email_QuenMatKhau"] as string;

           if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(email))
        {
   TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn!";
      return RedirectToAction("QuenMatKhau");
  }

 if (otpExpiry.HasValue && DateTime.Now > otpExpiry.Value)
        {
   TempData["ErrorMessage"] = "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới!";
      ViewBag.Email = email;
               return View();
    }

        if (otp != savedOTP)
                {
        TempData["ErrorMessage"] = "Mã OTP không chính xác!";
    ViewBag.Email = email;
        return View();
    }

                // OTP đúng, chuyển sang trang đặt lại mật khẩu
              Session["OTP_Verified"] = true;
        return RedirectToAction("DatLaiMatKhau");
     }
 catch (Exception ex)
      {
    System.Diagnostics.Debug.WriteLine($"[ERROR - XacThucOTP] {ex.Message}");
      TempData["ErrorMessage"] = "Có lỗi xảy ra!";
       return View();
        }
        }

        // POST: DangNhapNV/DangNhapNV/GuiLaiOTP
     [HttpPost]
     public async Task<JsonResult> GuiLaiOTP()
        {
     try
 {
       string email = Session["Email_QuenMatKhau"] as string;

                if (string.IsNullOrEmpty(email))
          {
    return Json(new { success = false, message = "Phiên làm việc đã hết hạn!" });
      }

       var nhanVien = db.NhanViens.FirstOrDefault(nv => nv.Email == email && nv.DaHoatDong);

                if (nhanVien == null)
       {
             return Json(new { success = false, message = "Email không tồn tại!" });
         }

 // Tạo OTP mới
              var otp = new Random().Next(100000, 999999).ToString();
                Session["OTP_QuenMatKhau"] = otp;
            Session["OTP_Expiry"] = DateTime.Now.AddMinutes(5);

         // Gửi email
        bool emailSent = await GuiEmailQuenMatKhau(email, nhanVien.HoVaTen, otp, true);

        if (!emailSent)
{
         return Json(new { success = false, message = "Không thể gửi email!" });
  }

     return Json(new { success = true, message = "Mã OTP mới đã được gửi!" });
    }
            catch (Exception ex)
   {
     System.Diagnostics.Debug.WriteLine($"[ERROR - GuiLaiOTP] {ex.Message}");
        return Json(new { success = false, message = "Có lỗi xảy ra!" });
            }
        }

        // GET: DangNhapNV/DangNhapNV/DatLaiMatKhau
        public ActionResult DatLaiMatKhau()
        {
       if (Session["OTP_Verified"] == null || !(bool)Session["OTP_Verified"])
          {
     TempData["ErrorMessage"] = "Vui lòng xác thực OTP trước!";
                return RedirectToAction("QuenMatKhau");
     }

ViewBag.Email = Session["Email_QuenMatKhau"];
   return View();
  }

        // POST: DangNhapNV/DangNhapNV/DatLaiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DatLaiMatKhau(DatLaiMatKhauNVViewModel model)
      {
    try
   {
       if (!ModelState.IsValid)
     {
       ViewBag.Email = Session["Email_QuenMatKhau"];
              return View(model);
   }

                string email = Session["Email_QuenMatKhau"] as string;

          if (string.IsNullOrEmpty(email))
 {
    TempData["ErrorMessage"] = "Phiên làm việc đã hết hạn!";
             return RedirectToAction("QuenMatKhau");
        }

  var nhanVien = db.NhanViens.FirstOrDefault(nv => nv.Email == email && nv.DaHoatDong);

    if (nhanVien == null)
    {
   TempData["ErrorMessage"] = "Không tìm thấy nhân viên!";
       return RedirectToAction("QuenMatKhau");
       }

          // Cập nhật mật khẩu mới
                nhanVien.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhauMoi);
         nhanVien.NgayCapNhat = DateTime.Now;
        db.SaveChanges();

    // Gửi email thông báo
     await GuiEmailThongBaoDatLaiMatKhau(email, nhanVien.HoVaTen);

   // Xóa Session
       Session.Remove("OTP_QuenMatKhau");
  Session.Remove("Email_QuenMatKhau");
          Session.Remove("OTP_Expiry");
                Session.Remove("OTP_Verified");

   TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay.";
    return RedirectToAction("DangNhap");
            }
         catch (Exception ex)
            {
     System.Diagnostics.Debug.WriteLine($"[ERROR - DatLaiMatKhau] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra!";
   ViewBag.Email = Session["Email_QuenMatKhau"];
        return View(model);
        }
        }

  // ==================== PRIVATE METHODS ====================

     /// <summary>
        /// Redirect về trang tương ứng vai trò
        /// </summary>
        private ActionResult RedirectToNhanVienArea()
      {
            string vaiTro = Session["VaiTro"]?.ToString() ?? "";

        switch (vaiTro.ToLower())
          {
  case "admin":
    case "quản lý":
       return RedirectToAction("Index", "DashboardAdmin", new { area = "Admin" });

            case "letan":
        case "lễ tân":
   return RedirectToAction("Index", "SoDoPhong", new { area = "NhanVienLeTan" });

  case "nhanvienbuong":
      case "nhân viên buồng":
     return RedirectToAction("Index", "DashboardNVB", new { area = "NhanVienBuong" });

    default:
        return RedirectToAction("Index", "SoDoPhong", new { area = "NhanVienLeTan" });
      }
        }

        /// <summary>
        /// Tạo token remember me
        /// </summary>
        private string GenerateRememberToken(int nhanVienId)
        {
            return BCrypt.Net.BCrypt.HashPassword($"{nhanVienId}_{DateTime.Now.Ticks}");
        }

        /// <summary>
        /// Gửi email quên mật khẩu với OTP
   /// </summary>
        private async Task<bool> GuiEmailQuenMatKhau(string email, string hoVaTen, string otp, bool isResend = false)
        {
         try
            {
var emailService = new MailKitEmailService();
                string subject = isResend ? "Mã OTP mới - Đặt lại mật khẩu Nhân viên" : "Đặt lại mật khẩu - Serene Horizon";
         string tieuDe = isResend ? "Mã OTP Mới" : "Đặt Lại Mật Khẩu";
    string noiDung = isResend 
       ? "Bạn đã yêu cầu gửi lại mã OTP. Vui lòng sử dụng mã mới dưới đây:" 
         : "Bạn đã yêu cầu đặt lại mật khẩu. Vui lòng sử dụng mã OTP dưới đây để xác thực:";

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
            <p style='font-size: 1.1rem; color: #555; margin-bottom: 5px;'>Mã xác thực OTP:</p>
   <p style='font-size: 2.5rem; font-weight: bold; color: #d9534f; margin: 0;'>{otp}</p>
    </div>
 <p style='color: #888; font-size: 0.9rem;'>Mã này có hiệu lực trong vòng <strong>5 phút</strong>. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
        <p>Trân trọng,<br/><strong>Serene Horizon Team</strong></p>
        <div style='margin-top: 20px; padding-top: 10px; border-top: 1px solid #eee; text-align: center;'>
            <p style='font-size: 0.8rem; color: #aaa;'>&copy; {DateTime.Now.Year} Serene Horizon. All rights reserved.</p>
   </div>
    </div>
</body>
</html>";

 return await emailService.SendEmailAsync(email, subject, body);
         }
         catch (Exception ex)
            {
          System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR] {ex.Message}");
   return false;
         }
  }

        /// <summary>
        /// Gửi email thông báo đặt lại mật khẩu thành công
        /// </summary>
        private async Task<bool> GuiEmailThongBaoDatLaiMatKhau(string email, string hoVaTen)
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
    <h2 style='color: #28a745; border-bottom: 2px solid #eee; padding-bottom: 10px;'>✓ Đặt Lại Mật Khẩu Thành Công</h2>
 <p><strong>Chào {hoVaTen},</strong></p>
   <p>Mật khẩu của bạn đã được đặt lại thành công vào lúc <strong>{DateTime.Now:HH:mm:ss dd/MM/yyyy}</strong>.</p>
     
        <div style='margin: 30px 0; padding: 20px; background-color: #d4edda; border-left: 4px solid #28a745; border-radius: 4px;'>
       <p style='margin: 0; color: #155724; font-size: 1rem;'>
      <strong>✓ Thông báo bảo mật:</strong><br/>
                Tài khoản của bạn đã được cập nhật mật khẩu mới. Bạn có thể đăng nhập ngay bây giờ.
    </p>
        </div>

   <div style='margin: 20px 0; padding: 15px; background-color: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;'>
     <p style='margin: 0; color: #856404; font-size: 0.95rem;'>
             <strong>⚠ Lưu ý:</strong><br/>
     Nếu bạn <strong>KHÔNG</strong> thực hiện thay đổi này, vui lòng liên hệ quản trị viên ngay!
        </p>
        </div>

        <p style='margin-top: 30px;'>Trân trọng,<br/><strong>Serene Horizon Team</strong></p>
        
        <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; text-align: center;'>
            <p style='font-size: 0.8rem; color: #aaa;'>&copy; {DateTime.Now.Year} Serene Horizon. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        return await emailService.SendEmailAsync(email, subject, body);
      }
            catch (Exception ex)
       {
           System.Diagnostics.Debug.WriteLine($"[EMAIL ERROR] {ex.Message}");
        return false;
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
