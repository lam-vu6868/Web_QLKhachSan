using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckOut;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý Check-out cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class CheckOutController : Controller
    {
    private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        private bool CheckRole()
        {
      string vaiTro = Session["VaiTro"]?.ToString();
          return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        /// <summary>
        /// GET: CheckOut/Index
        /// Hiển thị danh sách đơn đặt phòng cần check-out
  /// </summary>
        public ActionResult Index(CheckOutFilterViewModel filter)
    {
            try
            {
 if (!CheckRole())
     {
                 TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
           return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
      }

          var viewModel = new CheckOutListViewModel();
     viewModel.Filter = filter ?? new CheckOutFilterViewModel();

     // Query đơn đặt phòng trạng thái = 2 (Đã check-in, chưa check-out)
      var query = db.DatPhongs
       .Where(dp => dp.TrangThaiDatPhong == 2); // Đã check-in, đang ở

      // Filter theo tìm kiếm
      if (!string.IsNullOrWhiteSpace(filter.TimKiem))
     {
     string searchTerm = filter.TimKiem.Trim().ToLower();
             query = query.Where(dp =>
dp.MaDatPhong.ToLower().Contains(searchTerm) ||
 dp.KhachHang.HoVaTen.ToLower().Contains(searchTerm) ||
   dp.KhachHang.SoDienThoai.Contains(searchTerm));
         }

     // Filter theo ngày check-out
           if (filter.NgayCheckOut.HasValue)
     {
   var ngay = filter.NgayCheckOut.Value.Date;
      query = query.Where(dp => dp.NgayTra.HasValue &&
           DbFunctions.TruncateTime(dp.NgayTra) == ngay);
       }
    else
         {
  // Mặc định hiển thị check-out hôm nay và ngày mai
           var homNay = DateTime.Now.Date;
               var ngayMai = homNay.AddDays(1);
      query = query.Where(dp => dp.NgayTra.HasValue &&
 (DbFunctions.TruncateTime(dp.NgayTra) == homNay ||
  DbFunctions.TruncateTime(dp.NgayTra) == ngayMai));
      }

             // Thống kê
 var homNayDate = DateTime.Now.Date;
         viewModel.ThongKe = new CheckOutStatViewModel
    {
 CanCheckOutHomNay = db.DatPhongs.Count(dp =>
        dp.TrangThaiDatPhong == 2 &&
       dp.NgayTra.HasValue &&
        DbFunctions.TruncateTime(dp.NgayTra) == homNayDate),
  DaCheckOutHomNay = db.DatPhongs.Count(dp =>
                 dp.TrangThaiDatPhong == 3 &&
 dp.NgayTra.HasValue &&
       DbFunctions.TruncateTime(dp.NgayTra) == homNayDate),
  QuaHan = db.DatPhongs.Count(dp =>
     dp.TrangThaiDatPhong == 2 &&
            dp.NgayTra.HasValue &&
      DbFunctions.TruncateTime(dp.NgayTra) < homNayDate),
     TongDonDangO = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 2)
        };

          // Pagination
          int totalRecords = query.Count();
    int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
       int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;
     int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

    viewModel.TotalRecords = totalRecords;
                viewModel.PageSize = pageSize;
    viewModel.CurrentPage = currentPage;
            viewModel.TotalPages = totalPages;

       // Lấy danh sách đơn đặt phòng
       var datPhongs = query
 .Include(dp => dp.KhachHang)
         .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
     .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
          .OrderBy(dp => dp.NgayTra)
           .ThenByDescending(dp => dp.NgayTao)
  .Skip((currentPage - 1) * pageSize)
          .Take(pageSize)
  .ToList();

     viewModel.DanhSachCheckOut = datPhongs.Select(dp => MapToCheckOutItemViewModel(dp)).ToList();

        return View(viewModel);
       }
        catch (Exception ex)
{
   System.Diagnostics.Debug.WriteLine($"[ERROR - CheckOut/Index] {ex.Message}");
    TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách check-out!";
         return View(new CheckOutListViewModel());
    }
     }

        /// <summary>
        /// POST: CheckOut/DoCheckOut
        /// Thực hiện check-out cho đơn đặt phòng
        /// </summary>
      [HttpPost]
        public JsonResult DoCheckOut(int datPhongId, string ghiChuCheckOut)
     {
       try
            {
   if (!CheckRole())
  {
      return Json(new { success = false, message = "Bạn không có quyền!" });
   }

    var datPhong = db.DatPhongs
  .Include(dp => dp.ChiTietDatPhongs)
  .Include(dp => dp.ChiTietDatDichVus)
         .Include(dp => dp.KhachHang)
.FirstOrDefault(dp => dp.DatPhongId == datPhongId);

  if (datPhong == null)
      {
   return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });
      }

   // Kiểm tra trạng thái
  if (datPhong.TrangThaiDatPhong != 2)
        {
   return Json(new { success = false, message = "Chỉ check-out được đơn đã check-in!" });
   }

     // ===== TÍNH TỔNG TIỀN =====
      // ✅ FIX: Tính lại CHÍNH XÁC từ ChiTietDatPhong + ChiTietDatDichVu
      // KHÔNG dùng DatPhong.TongTien để tránh lỗi cộng x2
      
      // 1. Tính tổng tiền phòng từ ChiTietDatPhong (đã trừ giảm giá)
    decimal tongTienPhong = datPhong.ChiTietDatPhongs?.Sum(ct => ct.ThanhTien ?? 0) ?? 0;
      
    // 2. Tính tổng tiền dịch vụ từ ChiTietDatDichVu
      decimal tongTienDichVu = datPhong.ChiTietDatDichVus?.Sum(dv => dv.ThanhTien ?? 0) ?? 0;
      
      // 3. Tổng tiền hóa đơn = Tiền phòng + Tiền dịch vụ
decimal tongTien = tongTienPhong + tongTienDichVu;

          // ===== KIỂM TRA ĐÃ CÓ HÓA ĐƠN CHƯA =====
            var hoaDonCu = db.HoaDons.FirstOrDefault(hd => hd.DatPhongId == datPhong.DatPhongId);
       if (hoaDonCu != null)
{
 return Json(new { 
         success = false, 
         message = "Đơn này đã có hóa đơn rồi!",
     hoaDonId = hoaDonCu.HoaDonId,
                maHoaDon = hoaDonCu.MaHoaDon
       });
 }

      // ===== TẠO HÓA ĐƠN MỚI =====
  var hoaDon = new HoaDon
{
         MaHoaDon = GenerateMaHoaDon(),
   DatPhongId = datPhong.DatPhongId,
             MaKhachHang = datPhong.MaKhachHang,
        NgayLap = DateTime.Now,
  TongTien = tongTien,
 TrangThaiHoaDon = 0, // Chưa thanh toán
      NguoiLapHoaDon = Session["HoVaTen"]?.ToString(),
                  NgayTao = DateTime.Now
     };
           db.HoaDons.Add(hoaDon);

          // Thêm ghi chú check-out
        if (!string.IsNullOrWhiteSpace(ghiChuCheckOut))
                {
          datPhong.GhiChu = (datPhong.GhiChu ?? "") +
     $"\n[CHECK-OUT] {DateTime.Now:dd/MM/yyyy HH:mm} - {ghiChuCheckOut}";
          }

         // Cập nhật ngày check-out thực tế
  datPhong.NgayTra = DateTime.Now;

    // Cập nhật trạng thái phòng sang "Đang dọn"
 foreach (var chiTiet in datPhong.ChiTietDatPhongs.Where(ct => ct.PhongId.HasValue))
        {
    var phong = db.Phongs.Find(chiTiet.PhongId.Value);
        if (phong != null)
{
      phong.TrangThaiPhong = 3; // Đang dọn
 phong.NgayCapNhat = DateTime.Now;
  }

     chiTiet.TrangThaiPhong = 3;
     chiTiet.NgayDi = DateTime.Now;
    chiTiet.NgayCapNhat = DateTime.Now;
           }

      // Cập nhật trạng thái đơn
        datPhong.TrangThaiDatPhong = 3; // Đã check-out
     datPhong.NgayCapNhat = DateTime.Now;

    db.SaveChanges();

     var danhSachPhong = string.Join(", ", datPhong.ChiTietDatPhongs
          .Where(ct => ct.Phong != null)
      .Select(ct => ct.Phong.MaPhong));

  return Json(new
        {
     success = true,
        message = $"Check-out thành công cho khách {datPhong.KhachHang.HoVaTen}! Phòng: {danhSachPhong}. Hóa đơn: {hoaDon.MaHoaDon}",
        checkOutTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
        tongTien = tongTien,
        hoaDonId = hoaDon.HoaDonId,
        maHoaDon = hoaDon.MaHoaDon
       });
  }
          catch (Exception ex)
      {
     System.Diagnostics.Debug.WriteLine($"[ERROR - DoCheckOut] {ex.Message}");
    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
  }
   }

  // Helper Methods
        private CheckOutItemViewModel MapToCheckOutItemViewModel(DatPhong datPhong)
   {
   var chiTietPhongs = datPhong.ChiTietDatPhongs.ToList();

    return new CheckOutItemViewModel
            {
      DatPhongId = datPhong.DatPhongId,
    MaDatPhong = datPhong.MaDatPhong,
    NgayCheckIn = datPhong.NgayNhan,
        NgayCheckOut = datPhong.NgayTra,
                SoDem = datPhong.SoDem,
        SoLuongKhach = datPhong.SoLuongKhach,

       // Khách hàng
        TenKhachHang = datPhong.KhachHang?.HoVaTen,
   SoDienThoai = datPhong.KhachHang?.SoDienThoai,
        Email = datPhong.KhachHang?.Email,

                // Phòng
      SoLuongPhong = chiTietPhongs.Sum(ct => ct.SoLuong),
          DanhSachLoaiPhong = string.Join(", ", chiTietPhongs
  .GroupBy(ct => ct.LoaiPhong?.TenLoai)
  .Select(g => $"{g.Key} x{g.Sum(ct => ct.SoLuong)}")),
  DanhSachPhong = chiTietPhongs.Any(ct => ct.PhongId.HasValue)
      ? string.Join(", ", chiTietPhongs.Where(ct => ct.Phong != null).Select(ct => ct.Phong.MaPhong))
: "Chưa gán",

          // Trạng thái
 TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
   TongTien = datPhong.TongTien,
    GhiChu = datPhong.GhiChu,

       // Online payment
                DaThanhToanOnline = !string.IsNullOrEmpty(datPhong.PaymentRefId),
   OnlinePaymentStatus = datPhong.OnlinePaymentStatus
  };
  }

     /// <summary>
      /// Tự động tạo mã hóa đơn theo format: HD000001, HD000002, ...
        /// </summary>
      private string GenerateMaHoaDon()
     {
            var lastHoaDon = db.HoaDons.OrderByDescending(hd => hd.HoaDonId).FirstOrDefault();
            int newId = 1;

       if (lastHoaDon != null && !string.IsNullOrEmpty(lastHoaDon.MaHoaDon))
            {
          int lastId;
if (int.TryParse(lastHoaDon.MaHoaDon.Replace("HD", ""), out lastId))
     {
        newId = lastId + 1;
        }
            }

            return "HD" + newId.ToString("D6");
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
