using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            // 1. KIỂM TRA TỔNG: Người dùng có được xác thực không?
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new DB_QLKhachSanEntities())
                {
                    var tenDangNhap = User.Identity.Name;

                    // 2. LẤY THÔNG TIN CƠ BẢN
                    var tendn = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == tenDangNhap);
                    if (tendn == null)
                    {
                        FormsAuthentication.SignOut();
                        return RedirectToAction("Index", "Home");
                    }

                    // SỬA LỖI DB: Dùng tendn.MaKhachHang (giống như trong LoginController)
                    var khachHang = db.KhachHangs.FirstOrDefault(x => x.MaKhachHang == tendn.MaKhachHang);
                    if (khachHang == null)
                    {
                        FormsAuthentication.SignOut();
                        return RedirectToAction("Index", "Home");
                    }

                    // 3. XỬ LÝ LOGIC "REMEMBER ME"
                    string cokieValue = Request.Cookies["RememberMeToken"]?.Value;
                    var dbToken = db.TokenLogins.FirstOrDefault(x => x.MaTaiKhoan == tendn.TaiKhoanId);

                    // Trường hợp 1: Có "Remember Me" (có cookie VÀ có token trong DB)
                    if (!string.IsNullOrEmpty(cokieValue) && dbToken != null)
                    {
                        if (cokieValue == dbToken.token)
                        {
                            if (dbToken.NgayHetHan > DateTime.Now)
                            {
                                // === TOKEN HỢP LỆ, CÒN HẠN ===
                                // 1. Làm mới token
                                var newToken = Guid.NewGuid().ToString("N"); // Dùng "N" cho khớp
                                dbToken.token = newToken;
                                dbToken.NgayHetHan = DateTime.Now.AddDays(30);
                                db.SaveChanges();

                                // 2. Gửi cookie mới về trình duyệt
                                Response.Cookies["RememberMeToken"].Value = newToken;
                                Response.Cookies["RememberMeToken"].Expires = DateTime.Now.AddDays(30);
                                Response.Cookies["RememberMeToken"].HttpOnly = true; // Luôn thêm HttpOnly

                                // 3. SỬA LỖI SESSION: Nạp lại ĐẦY ĐỦ Session
                                HttpContext.Session["MaKhachHang"] = khachHang.MaKhachHang;
                                HttpContext.Session["HoVaTen"] = khachHang.HoVaTen;
                                HttpContext.Session["Email"] = khachHang.Email;
                                HttpContext.Session["TenDangNhap"] = khachHang.TenDangNhap;

                                // kiểm tra người dùng có phải là admin không nếu phải thì chuyển hướng đến trang admin
                                if (tendn.LoaiTaiKhoan == 1)
                                {
                                    return RedirectToAction("Index", "Admin/Dashboard");
                                }
                                else if (tendn.LoaiTaiKhoan == 2)
                                {
                                    return RedirectToAction("Index", "NhanVien/Dashboard");
                                }
                            }
                            else
                            {
                                // === TOKEN ĐÃ HẾT HẠN ===
                                db.TokenLogins.Remove(dbToken);
                                db.SaveChanges();
                                Response.Cookies["RememberMeToken"].Expires = DateTime.Now.AddDays(-1);
                                FormsAuthentication.SignOut();
                            }
                        }
                        else
                        {
                            // Token không khớp (bị giả mạo) -> Đăng xuất
                            db.TokenLogins.Remove(dbToken);
                            db.SaveChanges();
                            Response.Cookies["RememberMeToken"].Expires = DateTime.Now.AddDays(-1);
                            FormsAuthentication.SignOut();
                        }
                    }
                    // Trường hợp 2: Đăng nhập bình thường (hoặc không có token)
                    else if (Session["MaKhachHang"] == null) // Chỉ nạp nếu Session chưa có
                    {
                        // Nạp lại ĐẦY ĐỦ Session
                        HttpContext.Session["MaKhachHang"] = khachHang.MaKhachHang;
                        HttpContext.Session["HoVaTen"] = khachHang.HoVaTen;
                        HttpContext.Session["Email"] = khachHang.Email;
                        HttpContext.Session["TenDangNhap"] = khachHang.TenDangNhap;
                    }
                }
            }

            // Về trang chủ
            return RedirectToAction("Index", "Home");
        }
    }
}