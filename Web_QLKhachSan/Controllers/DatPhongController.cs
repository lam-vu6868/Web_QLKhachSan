using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Configuration;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Utils;

namespace Web_QLKhachSan.Controllers
{
    /// <summary>
    /// Controller đặt phòng cho Khách hàng tự đặt online
    /// ✅ ĐÃ CẬP NHẬT: Sử dụng bảng DatPhong đã gộp thay vì Booking
    /// </summary>
    public class DatPhongController : Controller
    {
    private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: DatPhong/ThongTinKhachHang
        public ActionResult ThongTinKhachHang(int? phongId)
        {
      // Kiểm tra đăng nhập
         if (Session["MaKhachHang"] == null)
        {
         TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
  return RedirectToAction("DangNhap", "Login");
            }

       // Lấy thông tin khách hàng từ session
            int maKhachHang = Convert.ToInt32(Session["MaKhachHang"]);
       var khachHang = db.KhachHangs.Find(maKhachHang);

      if (khachHang == null)
            {
TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
    return RedirectToAction("Index", "Home");
            }

            // Kiểm tra nếu đã có thông tin đặt phòng trong Session
          var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
    ThongTinDatPhongViewModel model;

// Nếu có phongId trong URL và khác với session hiện tại, tức là đặt phòng mới -> RESET session
   bool isNewBooking = phongId.HasValue && (existingModel == null || phongId.Value != existingModel.PhongId);

            if (existingModel != null && !isNewBooking)
          {
     // Sử dụng model đã có trong session để giữ nguyên thông tin đã nhập (cùng phòng)
  model = existingModel;
   
        // Chỉ cập nhật thông tin khách hàng nếu cần thiết
                if (model.HoVaTen != khachHang.HoVaTen || 
            model.Email != khachHang.Email || 
 model.SoDienThoai != khachHang.SoDienThoai)
        {
        model.HoVaTen = khachHang.HoVaTen;
  model.Email = khachHang.Email;
         model.SoDienThoai = khachHang.SoDienThoai;
      }
   
          // Đảm bảo DichVuDaChon không bị null
   if (model.DichVuDaChon == null)
      {
    model.DichVuDaChon = new List<DichVuDaChon>();
       }
            }
     else
    {
       // Tạo ViewModel mới (đặt phòng mới hoặc chưa có session)
  model = new ThongTinDatPhongViewModel
           {
 HoVaTen = khachHang.HoVaTen,
         Email = khachHang.Email,
         SoDienThoai = khachHang.SoDienThoai,
      NgayNhan = DateTime.Now.AddDays(1), // Mặc định ngày mai
    NgayTra = DateTime.Now.AddDays(2),   // Mặc định 1 đêm
            DichVuDaChon = new List<DichVuDaChon>() // Reset dịch vụ cho đặt phòng mới
    };
            }

    // Lấy thông tin phòng nếu có phongId (cho cả model mới và model đã có khi đặt phòng mới)
       if (phongId.HasValue || model.PhongId.HasValue)
 {
   var roomId = phongId ?? model.PhongId;
                var phong = db.Phongs
        .Include(p => p.LoaiPhong)
         .Include(p => p.PhongAnhs)
         .FirstOrDefault(p => p.PhongId == roomId.Value && p.DaHoatDong);
       
   if (phong == null)
    {
          TempData["ErrorMessage"] = "Phòng không tồn tại hoặc không khả dụng!";
   return RedirectToAction("Index", "PhongNghi");
           }

        // Gán thông tin phòng vào model
        model.PhongId = phong.PhongId;
     model.TenPhong = phong.TenPhong;
       model.LoaiPhong = phong.LoaiPhong.TenLoai;
             model.GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0;
 model.SoNguoiToiDa = phong.LoaiPhong.SoNguoiToiDa ?? 2;
    if (model.SoNguoi == 0) // Chỉ set default nếu chưa có giá trị
   {
   model.SoNguoi = model.SoNguoiToiDa;
   }
           model.HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : "");
        }

    return View(model);
      }

        // POST: DatPhong/ThongTinKhachHang
     [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinKhachHang(ThongTinDatPhongViewModel model)
        {
          // Kiểm tra đăng nhập
         if (Session["MaKhachHang"] == null)
        {
    TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
return RedirectToAction("DangNhap", "Login");
       }

            // CRITICAL: Preserve services from existing session
            var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
  if (existingModel != null && existingModel.DichVuDaChon != null)
    {
         // Preserve services từ session
   model.DichVuDaChon = existingModel.DichVuDaChon;
            }
      else
          {
                // Khởi tạo nếu chưa có
            model.DichVuDaChon = new List<DichVuDaChon>();
      }

            // Validation tùy chỉnh
            string errorMessage;
 if (!model.IsValid(out errorMessage))
       {
           ModelState.AddModelError("", errorMessage);
            }

     if (!ModelState.IsValid)
      {
                // Nếu có lỗi, load lại thông tin phòng
              if (model.PhongId.HasValue)
 {
     var phong = db.Phongs
  .Include(p => p.LoaiPhong)
                .Include(p => p.PhongAnhs)
      .FirstOrDefault(p => p.PhongId == model.PhongId.Value);
      
      if (phong != null)
          {
           model.TenPhong = phong.TenPhong;
           model.LoaiPhong = phong.LoaiPhong.TenLoai;
    model.GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0;
  model.SoNguoiToiDa = phong.LoaiPhong.SoNguoiToiDa ?? 2;
                   model.HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : "");
           }
           }
        return View(model);
     }

            // Lưu thông tin vào Session để dùng cho bước tiếp theo
            // Đảm bảo DichVuDaChon được khởi tạo
     if (model.DichVuDaChon == null)
          {
  model.DichVuDaChon = new List<DichVuDaChon>();
    }
    Session["ThongTinDatPhong"] = model;

            // Chuyển sang trang dịch vụ đặt thêm
            return RedirectToAction("DichVuDatThem");
        }

        // GET: DatPhong/DichVuDatThem
        public ActionResult DichVuDatThem(int? phongId)
        {
     // Ngăn browser cache trang này
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
       Response.Cache.SetNoStore();
   Response.Cache.SetExpires(DateTime.MinValue);
            
        // Kiểm tra thông tin đặt phòng trong Session
       var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            
    // Kiểm tra xem có phải đặt phòng mới không
            bool isNewBooking = phongId.HasValue && 
       (existingModel == null || existingModel.PhongId != phongId.Value);
        
            // QUAN TRỌNG: Reset hoàn toàn session khi đặt phòng mới
       if (isNewBooking)
    {
        Session.Remove("ThongTinDatPhong");
                existingModel = null;
        }
   
 if (existingModel == null)
     {
            if (phongId.HasValue)
            {
  // Kiểm tra đăng nhập
             if (Session["MaKhachHang"] == null)
           {
           TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                  return RedirectToAction("DangNhap", "Login");
        }

        int maKhachHang = Convert.ToInt32(Session["MaKhachHang"]);
                var khachHang = db.KhachHangs.Find(maKhachHang);
   if (khachHang == null)
          {
     TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
    return RedirectToAction("Index", "Home");
              }

     var phong = db.Phongs
      .Include(p => p.LoaiPhong)
    .Include(p => p.PhongAnhs)
  .FirstOrDefault(p => p.PhongId == phongId.Value && p.DaHoatDong);

          if (phong == null)
        {
    TempData["ErrorMessage"] = "Phòng không tồn tại hoặc không khả dụng!";
              return RedirectToAction("Index", "PhongNghi");
        }

var autoModel = new ThongTinDatPhongViewModel
    {
 HoVaTen = khachHang.HoVaTen,
   Email = khachHang.Email,
         SoDienThoai = khachHang.SoDienThoai,
    NgayNhan = DateTime.Now.AddDays(1),
         NgayTra = DateTime.Now.AddDays(2),
              PhongId = phong.PhongId,
             TenPhong = phong.TenPhong,
      LoaiPhong = phong.LoaiPhong?.TenLoai,
GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0,
             SoNguoiToiDa = phong.LoaiPhong?.SoNguoiToiDa ?? 2,
     SoNguoi = phong.LoaiPhong?.SoNguoiToiDa ?? 2,
 HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : ""),
        DichVuDaChon = new List<DichVuDaChon>() // Khởi tạo danh sách trống cho đặt phòng mới
      };

         // Lưu vào Session để các bước sau dùng (hoặc thay thế session cũ)
       Session["ThongTinDatPhong"] = autoModel;
           }
       else
{
            TempData["ErrorMessage"] = "Vui lòng nhập thông tin khách hàng trước!";
   return RedirectToAction("ThongTinKhachHang");
     }
        }

            // Lấy thông tin đặt phòng từ Session (sẵn có hoặc mới tạo)
        var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
       
    // Đảm bảo DichVuDaChon không null
            if (thongTinDatPhong.DichVuDaChon == null)
  {
     thongTinDatPhong.DichVuDaChon = new List<DichVuDaChon>();
            }

       // Load danh sách loại dịch vụ và dịch vụ từ database
            var loaiDichVus = db.LoaiDichVus
    .Where(ldv => ldv.DichVus.Any(dv => dv.DaHoatDong)) // Chỉ lấy loại có dịch vụ đang hoạt động
           .OrderBy(ldv => ldv.LoaiDichVuId)
         .ToList();

         // Tạo ViewModel
       var viewModel = new DichVuDatThemViewModel
{
           ThongTinDatPhong = thongTinDatPhong,
    DanhSachLoaiDichVu = loaiDichVus
        };

            return View(viewModel);
        }

      // POST: DatPhong/DichVuDatThem
     [HttpPost]
        public ActionResult DichVuDatThem(int[] services)
  {
     // Lấy thông tin đặt phòng từ Session
   var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
        if (thongTinDatPhong == null)
 {
    TempData["ErrorMessage"] = "Thông tin đặt phòng không hợp lệ!";
     return RedirectToAction("ThongTinKhachHang");
  }

     // Clear dịch vụ cũ để tránh nhân đôi
    thongTinDatPhong.DichVuDaChon = new List<DichVuDaChon>();
            
     if (services != null && services.Length > 0)
  {
  // Lấy thông tin các dịch vụ đã chọn từ database
         var dichVuList = db.DichVus
        .Where(dv => services.Contains(dv.DichVuId) && dv.DaHoatDong)
    .ToList();

          foreach (var dichVu in dichVuList)
             {
         thongTinDatPhong.DichVuDaChon.Add(new DichVuDaChon
         {
    DichVuId = dichVu.DichVuId,
 TenDichVu = dichVu.TenDichVu,
  Gia = dichVu.GiaUuDai ?? dichVu.Gia ?? 0
           });
        }
 }

     // Cập nhật lại Session
            Session["ThongTinDatPhong"] = thongTinDatPhong;

            // Chuyển sang trang thanh toán
 return RedirectToAction("ThanhToan");
        }

        /// <summary>
        /// Trang thanh toán
        /// </summary>
     public ActionResult ThanhToan()
     {
            // Lấy thông tin đặt phòng từ Session
            var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
        if (thongTinDatPhong == null)
  {
            TempData["ErrorMessage"] = "Thông tin đặt phòng không hợp lệ!";
           return RedirectToAction("ThongTinKhachHang");
    }

     // ✅ Tạo đơn đặt phòng trong bảng DatPhong (thay vì Booking)
            string paymentRefId = CreateDatPhongRecord(thongTinDatPhong);
       ViewBag.BookingRef = paymentRefId;

        // Thêm logic để kiểm tra số đêm cho tùy chọn gia hạn
   ViewBag.CanUseCustomDelay = thongTinDatPhong.SoDem >= 2;
    ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);

     return View(thongTinDatPhong);
        }

        /// <summary>
   /// ✅ Tạo đơn đặt phòng trong bảng DatPhong (thay vì Booking)
        /// </summary>
        private string CreateDatPhongRecord(ThongTinDatPhongViewModel thongTinDatPhong)
    {
       try
     {
       // Tạo Payment Reference ID unique
     string paymentRefId = "BOOKING_" + DateTime.Now.Ticks;
   
      // Lấy MaKhachHang từ session
        int? maKhachHang = Session["MaKhachHang"] != null 
                    ? (int?)Convert.ToInt32(Session["MaKhachHang"]) 
   : null;

     // ✅ Tạo đơn đặt phòng trong bảng DatPhong
         var datPhong = new DatPhong
       {
           // Thông tin online payment
                 PaymentRefId = paymentRefId,
         PaymentMethod = "VNPAY",
       OnlinePaymentStatus = "PENDING",
CustomerName = thongTinDatPhong.HoVaTen,
    TotalAmount = thongTinDatPhong.TongCong,
        
    // Thông tin đặt phòng
MaKhachHang = maKhachHang,
          NgayDat = DateTime.Now,
         NgayNhan = thongTinDatPhong.NgayNhan,
         NgayTra = thongTinDatPhong.NgayTra,
   SoDem = thongTinDatPhong.SoDem,
         SoLuongKhach = thongTinDatPhong.SoNguoi,
            
        // Trạng thái
TrangThaiDatPhong = 0, // Chờ xác nhận
     TrangThaiThanhToan = 0, // Chưa thanh toán
              
         // Tiền
          TongTien = thongTinDatPhong.TongCong,
       HinhThucThanhToan = 2, // QR Code/VNPay
     
  // Metadata
        GhiChu = "Đặt phòng online qua VNPay",
       NgayTao = DateTime.Now
      };

    db.DatPhongs.Add(datPhong);
            db.SaveChanges();

    // Lưu vào session để dùng sau
       Session["CurrentDatPhongId"] = datPhong.DatPhongId;
 Session["CurrentPaymentRefId"] = paymentRefId;

      // ✅ Tạo chi tiết đặt phòng
       if (thongTinDatPhong.PhongId.HasValue)
  {
var phong = db.Phongs
             .Include(p => p.LoaiPhong)
      .FirstOrDefault(p => p.PhongId == thongTinDatPhong.PhongId.Value);
         
  if (phong != null)
        {
     var chiTiet = new ChiTietDatPhong
           {
          DatPhongId = datPhong.DatPhongId,
   PhongId = phong.PhongId,
   LoaiPhongId = phong.LoaiPhongId,
            DonGia = thongTinDatPhong.GiaPhong,
  SoLuong = 1,
        NgayDen = thongTinDatPhong.NgayNhan,
  NgayDi = thongTinDatPhong.NgayTra,
              ThanhTien = thongTinDatPhong.TongTienPhong,
       TrangThaiPhong = 1, // Đã đặt
     NgayCapNhat = DateTime.Now
 };
      
              db.ChiTietDatPhongs.Add(chiTiet);
        }
      }

    // ✅ Tạo chi tiết dịch vụ
      if (thongTinDatPhong.DichVuDaChon != null && thongTinDatPhong.DichVuDaChon.Any())
       {
       foreach (var dv in thongTinDatPhong.DichVuDaChon)
   {
        var chiTietDV = new ChiTietDatDichVu
     {
      DatPhongId = datPhong.DatPhongId,
       DichVuId = dv.DichVuId,
      SoLuong = 1,
            DonGia = dv.Gia,
     ThanhTien = dv.Gia,
    NgaySuDung = thongTinDatPhong.NgayNhan
               };
        
   db.ChiTietDatDichVus.Add(chiTietDV);
     }
         }

              db.SaveChanges();

                // ✅ Lưu log VNPay
       var vnpayLog = new VNPAY_Transaction_Logs
         {
        DatPhongId = datPhong.DatPhongId,
 vnp_txn_ref = DateTime.Now.Ticks.ToString(),
                    vnp_amount = thongTinDatPhong.TongCong,
     vnp_response_code = "WAITING",
     vnp_transaction_status = "WAITING",
    log_details = $"Tạo đơn đặt phòng online {paymentRefId} - Khách hàng: {thongTinDatPhong.HoVaTen} - Số tiền: {thongTinDatPhong.TongCong:N0}đ",
    log_time = DateTime.Now
      };

     db.VNPAY_Transaction_Logs.Add(vnpayLog);
        db.SaveChanges();

     return paymentRefId;
   }
  catch (Exception ex)
  {
      System.Diagnostics.Debug.WriteLine($"[ERROR - CreateDatPhongRecord] {ex.Message}");
   return "ERROR_" + DateTime.Now.Ticks;
            }
        }

        /// <summary>
        /// API giả lập chuyển khoản thành công
        /// </summary>
        public ActionResult SimulatePayment(string paymentRefId)
        {
  try
  {
          // ✅ Tìm đơn đặt phòng theo PaymentRefId
     var datPhong = db.DatPhongs.FirstOrDefault(dp => dp.PaymentRefId == paymentRefId);
        if (datPhong != null)
           {
    // Cập nhật trạng thái thành công
  datPhong.OnlinePaymentStatus = "PAID";
     datPhong.TrangThaiThanhToan = 2; // Đã thanh toán đầy đủ
    datPhong.TrangThaiDatPhong = 1; // Đã xác nhận
       datPhong.NgayCapNhat = DateTime.Now;
    
      // Lưu log thành công
    var vnpayLog = new VNPAY_Transaction_Logs
       {
    DatPhongId = datPhong.DatPhongId,
          vnp_txn_ref = DateTime.Now.Ticks.ToString(),
             vnp_amount = datPhong.TotalAmount ?? datPhong.TongTien ?? 0,
            vnp_response_code = "00",
        vnp_transaction_status = "00",
       log_details = $"GIẢ LẬP chuyển khoản thành công cho đơn {paymentRefId} - Số tiền: {datPhong.TongTien:N0}đ",
        log_time = DateTime.Now
 };
   
        db.VNPAY_Transaction_Logs.Add(vnpayLog);
                    db.SaveChanges();
           
         return Json(new { success = true, message = "Chuyển khoản thành công!" }, JsonRequestBehavior.AllowGet);
                }
 
           return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" }, JsonRequestBehavior.AllowGet);
          }
       catch (Exception ex)
            {
  return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
         }
        }

        /// <summary>
        /// ✅ Xử lý thanh toán VNPay (đã cập nhật dùng DatPhong)
        /// </summary>
        private ActionResult ProcessVNPayPayment(ThongTinDatPhongViewModel thongTinDatPhong)
        {
  // Lấy config VNPay từ Web.config
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"] ?? Url.Action("VNPayReturn", "DatPhong", null, Request.Url.Scheme);
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"] ?? "DEMOTMNCODE";
    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"] ?? "DEMOHASHSECRET";

         // Tạo payment reference ID
            string paymentRefId = "BOOKING_" + DateTime.Now.Ticks;
            long vnpTxnRef = DateTime.Now.Ticks;

          // ✅ Lấy MaKhachHang từ session
         int? maKhachHang = Session["MaKhachHang"] != null 
                ? (int?)Convert.ToInt32(Session["MaKhachHang"]) 
      : null;

// ✅ Lưu đơn đặt phòng vào database (thay vì Booking)
 var datPhong = new DatPhong
  {
  PaymentRefId = paymentRefId,
           PaymentMethod = "VNPAY",
       OnlinePaymentStatus = "PENDING",
    CustomerName = thongTinDatPhong.HoVaTen,
    TotalAmount = thongTinDatPhong.TongCong,
      
          MaKhachHang = maKhachHang,
     NgayDat = DateTime.Now,
    NgayNhan = thongTinDatPhong.NgayNhan,
     NgayTra = thongTinDatPhong.NgayTra,
            SoDem = thongTinDatPhong.SoDem,
          SoLuongKhach = thongTinDatPhong.SoNguoi,
           
                TrangThaiDatPhong = 0,
         TrangThaiThanhToan = 0,
       TongTien = thongTinDatPhong.TongCong,
      HinhThucThanhToan = 2,
      
       GhiChu = "Đặt phòng online qua VNPay",
              NgayTao = DateTime.Now
            };

            db.DatPhongs.Add(datPhong);
            db.SaveChanges();

  // Lưu thông tin VNPay transaction log
            var vnpayLog = new VNPAY_Transaction_Logs
   {
                DatPhongId = datPhong.DatPhongId,
   vnp_txn_ref = vnpTxnRef.ToString(),
     vnp_amount = thongTinDatPhong.TongCong,
      vnp_response_code = "PENDING",
   vnp_transaction_status = "PENDING",
     log_details = $"Khởi tạo thanh toán cho đơn {paymentRefId} - Khách hàng: {thongTinDatPhong.HoVaTen}",
      log_time = DateTime.Now
            };

            db.VNPAY_Transaction_Logs.Add(vnpayLog);
            db.SaveChanges();

            // Lưu DatPhongId vào session để dùng khi return
   Session["CurrentDatPhongId"] = datPhong.DatPhongId;
    Session["CurrentPaymentRefId"] = paymentRefId;

     // Build URL for VNPay
            VnPayLibrary vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
     vnpay.AddRequestData("vnp_Amount", (thongTinDatPhong.TongCong * 100).ToString());
  vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
 vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VnPayUtils.GetIpAddress());
          vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan dat phong {paymentRefId} - {thongTinDatPhong.HoVaTen}");
         vnpay.AddRequestData("vnp_OrderType", "other");
         vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
  vnpay.AddRequestData("vnp_TxnRef", vnpTxnRef.ToString());

      string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        
  // Ghi log
     var successLog = new VNPAY_Transaction_Logs
    {
                DatPhongId = datPhong.DatPhongId,
      vnp_txn_ref = vnpTxnRef.ToString(),
          vnp_amount = thongTinDatPhong.TongCong,
                vnp_response_code = "REDIRECT",
      vnp_transaction_status = "REDIRECT",
            log_details = $"Redirect to VNPay URL: {paymentUrl}",
          log_time = DateTime.Now
            };
  db.VNPAY_Transaction_Logs.Add(successLog);
            db.SaveChanges();

 return Redirect(paymentUrl);
  }

        /// <summary>
        /// ✅ Callback từ VNPay (đã cập nhật dùng DatPhong)
     /// </summary>
        public ActionResult VNPayReturn()
        {
 if (Request.QueryString.Count > 0)
            {
     string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"] ?? "DEMOHASHSECRET";
   var vnpayData = Request.QueryString;
           VnPayLibrary vnpay = new VnPayLibrary();

          foreach (string s in vnpayData)
   {
   if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
  {
     vnpay.AddResponseData(s, vnpayData[s]);
               }
         }

       string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
             string vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
         string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
       string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
          string vnp_BankCode = Request.QueryString["vnp_BankCode"];
            decimal vnp_Amount = Convert.ToDecimal(vnpay.GetResponseData("vnp_Amount")) / 100;

             bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
     
        // ✅ Lấy thông tin đơn đặt phòng từ session
      string paymentRefId = Session["CurrentPaymentRefId"] as string;
                int? datPhongId = Session["CurrentDatPhongId"] as int?;

     if (checkSignature)
 {
// Log transaction result
         var vnpayLog = new VNPAY_Transaction_Logs
       {
    DatPhongId = datPhongId,
         vnp_txn_ref = vnp_TxnRef,
           vnp_amount = vnp_Amount,
    vnp_response_code = vnp_ResponseCode,
  vnp_transaction_status = vnp_TransactionStatus,
         log_details = $"VNPay return - Response: {vnp_ResponseCode}, Status: {vnp_TransactionStatus}, Bank: {vnp_BankCode}, TransNo: {vnp_TransactionNo}",
  log_time = DateTime.Now
       };
     db.VNPAY_Transaction_Logs.Add(vnpayLog);

           if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
             {
               // ✅ Thanh toán thành công - Cập nhật DatPhong
   if (datPhongId.HasValue)
       {
              var datPhong = db.DatPhongs.Find(datPhongId.Value);
    if (datPhong != null)
           {
   datPhong.OnlinePaymentStatus = "PAID";
 datPhong.TrangThaiThanhToan = 2; // Đã thanh toán đầy đủ
  datPhong.TrangThaiDatPhong = 1; // Đã xác nhận
 datPhong.NgayCapNhat = DateTime.Now;
  db.SaveChanges();
  }
     }

   TempData["PaymentSuccess"] = true;
    TempData["TransactionNo"] = vnp_TransactionNo;
            TempData["Amount"] = vnp_Amount;
    TempData["BankCode"] = vnp_BankCode;
         TempData["BookingRef"] = paymentRefId;
             }
    else
       {
    // ✅ Thanh toán thất bại
       if (datPhongId.HasValue)
       {
        var datPhong = db.DatPhongs.Find(datPhongId.Value);
       if (datPhong != null)
  {
      datPhong.OnlinePaymentStatus = "FAILED";
             datPhong.NgayCapNhat = DateTime.Now;
             db.SaveChanges();
   }
        }

   TempData["PaymentSuccess"] = false;
                  TempData["ErrorMessage"] = "Thanh toán thất bại. Mã lỗi: " + vnp_ResponseCode;
}

          db.SaveChanges();
           }
        else
      {
        // Invalid signature
        var errorLog = new VNPAY_Transaction_Logs
         {
       DatPhongId = datPhongId,
             vnp_txn_ref = vnp_TxnRef,
            vnp_amount = vnp_Amount,
         vnp_response_code = "INVALID_SIGNATURE",
                vnp_transaction_status = "INVALID_SIGNATURE",
     log_details = $"Invalid signature - Raw URL: {Request.RawUrl}",
  log_time = DateTime.Now
     };
        db.VNPAY_Transaction_Logs.Add(errorLog);
         db.SaveChanges();

    TempData["PaymentSuccess"] = false;
            TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xử lý thanh toán";
    }
            }
            else
   {
    TempData["PaymentSuccess"] = false;
                TempData["ErrorMessage"] = "Không nhận được phản hồi từ cổng thanh toán";
      }

            return RedirectToAction("XacNhanHoaDon");
   }

        public ActionResult XacNhanHoaDon()
  {
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