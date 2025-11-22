using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý hóa đơn cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class HoaDonController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

     private bool CheckRole()
 {
     string vaiTro = Session["VaiTro"]?.ToString();
        return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

// GET: NhanVienLeTan/HoaDon/Index
     public ActionResult Index(HoaDonFilterViewModel filter)
        {
        try
            {
          if (!CheckRole())
       {
        TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
       return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
         }

      var viewModel = new HoaDonListViewModel();
         viewModel.Filter = filter ?? new HoaDonFilterViewModel();

       var query = db.HoaDons.AsQueryable();

                // ===== BỘ LỌC =====
                
    // Tìm kiếm theo mã hóa đơn, tên khách hàng, số điện thoại
          if (!string.IsNullOrWhiteSpace(filter.TimKiem))
      {
              string searchTerm = filter.TimKiem.Trim().ToLower();
       query = query.Where(hd =>
         hd.MaHoaDon.ToLower().Contains(searchTerm) ||
          hd.KhachHang.HoVaTen.ToLower().Contains(searchTerm) ||
                 hd.KhachHang.SoDienThoai.Contains(searchTerm) ||
     hd.DatPhong.MaDatPhong.ToLower().Contains(searchTerm));
    }

     // Lọc theo trạng thái
                if (filter.TrangThaiHoaDon.HasValue)
   {
    query = query.Where(hd => hd.TrangThaiHoaDon == filter.TrangThaiHoaDon.Value);
       }

              // Lọc theo phương thức thanh toán
     if (!string.IsNullOrWhiteSpace(filter.PhuongThucThanhToan))
             {
      query = query.Where(hd => hd.PhuongThucThanhToan == filter.PhuongThucThanhToan);
  }

          // Lọc theo ngày lập
        if (filter.TuNgay.HasValue)
   {
        query = query.Where(hd => DbFunctions.TruncateTime(hd.NgayLap) >= DbFunctions.TruncateTime(filter.TuNgay.Value));
     }

     if (filter.DenNgay.HasValue)
 {
 query = query.Where(hd => DbFunctions.TruncateTime(hd.NgayLap) <= DbFunctions.TruncateTime(filter.DenNgay.Value));
              }

    // Lọc theo số tiền
     if (filter.TuSoTien.HasValue)
     {
                    query = query.Where(hd => hd.TongTien >= filter.TuSoTien.Value);
   }

        if (filter.DenSoTien.HasValue)
    {
         query = query.Where(hd => hd.TongTien <= filter.DenSoTien.Value);
              }

    // ===== THỐNG KÊ =====
              var today = DateTime.Now.Date;
           var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

                viewModel.ThongKe = new HoaDonThongKeViewModel
  {
             TongHoaDon = db.HoaDons.Count(),
           ChuaThanhToan = db.HoaDons.Count(hd => hd.TrangThaiHoaDon == 0),
           DaThanhToan = db.HoaDons.Count(hd => hd.TrangThaiHoaDon == 1),
           TongDoanhThu = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1).Sum(hd => (decimal?)hd.TongTien) ?? 0,
           DoanhThuHomNay = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1 && DbFunctions.TruncateTime(hd.NgayThanhToan) == today).Sum(hd => (decimal?)hd.TongTien) ?? 0,
  DoanhThuThangNay = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1 && hd.NgayThanhToan >= firstDayOfMonth).Sum(hd => (decimal?)hd.TongTien) ?? 0,
      ThanhToanTienMat = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1 && hd.PhuongThucThanhToan == "Tiền mặt").Sum(hd => (decimal?)hd.TongTien) ?? 0,
     ThanhToanChuyenKhoan = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1 && hd.PhuongThucThanhToan == "Chuyển khoản").Sum(hd => (decimal?)hd.TongTien) ?? 0,
               ThanhToanOnline = db.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1 && hd.PhuongThucThanhToan == "Online").Sum(hd => (decimal?)hd.TongTien) ?? 0
     };

      // ===== PHÂN TRANG =====
  int totalRecords = query.Count();
      int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
      int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;
                // int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

          viewModel.TotalRecords = totalRecords;
   viewModel.PageSize = pageSize;
      viewModel.CurrentPage = currentPage;
      // TotalPages là computed property, không cần gán

       // ===== LẤY DANH SÁCH HÓA ĐƠN =====
                var hoaDons = query
    .Include(hd => hd.KhachHang)
              .Include(hd => hd.DatPhong)
         .Include(hd => hd.ThanhToans)
    .OrderByDescending(hd => hd.NgayLap)
            .Skip((currentPage - 1) * pageSize)
        .Take(pageSize)
   .ToList();

          viewModel.DanhSachHoaDon = hoaDons.Select(hd => MapToHoaDonItemViewModel(hd)).ToList();

       return View(viewModel);
   }
      catch (Exception ex)
            {
     System.Diagnostics.Debug.WriteLine($"[ERROR - HoaDon/Index] {ex.Message}");
          TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách hóa đơn!";
       return View(new HoaDonListViewModel());
 }
        }

        // GET: NhanVienLeTan/HoaDon/Details/5
    public ActionResult Details(int? id)
        {
            try
       {
          if (!CheckRole())
           {
         TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
         return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

        if (!id.HasValue)
   {
        TempData["ErrorMessage"] = "Không tìm thấy hóa đơn!";
      return RedirectToAction("Index");
           }

            var hoaDon = db.HoaDons
   .Include(hd => hd.KhachHang)
          .Include(hd => hd.DatPhong)
         .Include(hd => hd.DatPhong.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
         .Include(hd => hd.DatPhong.ChiTietDatPhongs.Select(ct => ct.Phong))
      .Include(hd => hd.DatPhong.ChiTietDatDichVus.Select(dv => dv.DichVu))
         .Include(hd => hd.ThanhToans)
 .FirstOrDefault(hd => hd.HoaDonId == id.Value);

      if (hoaDon == null)
       {
        TempData["ErrorMessage"] = "Không tìm thấy hóa đơn!";
               return RedirectToAction("Index");
                }

          var viewModel = MapToHoaDonDetailsViewModel(hoaDon);

          return View(viewModel);
       }
  catch (Exception ex)
  {
  System.Diagnostics.Debug.WriteLine($"[ERROR - HoaDon/Details] {ex.Message}");
        TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết hóa đơn!";
     return RedirectToAction("Index");
        }
     }

      // POST: NhanVienLeTan/HoaDon/ThanhToan
 [HttpPost]
   public JsonResult ThanhToan(int hoaDonId, byte phuongThuc, string maGiaoDich)
   {
         try
    {
        if (!CheckRole())
   {
 return Json(new { success = false, message = "Bạn không có quyền!" });
    }

                var hoaDon = db.HoaDons
   .Include(hd => hd.DatPhong)
         .FirstOrDefault(hd => hd.HoaDonId == hoaDonId);

   if (hoaDon == null)
      {
             return Json(new { success = false, message = "Không tìm thấy hóa đơn!" });
          }

    if (hoaDon.TrangThaiHoaDon == 1)
       {
     return Json(new { success = false, message = "Hóa đơn này đã được thanh toán rồi!" });
       }

          // Validate chuyển khoản
     if (phuongThuc == 1 && string.IsNullOrWhiteSpace(maGiaoDich))
       {
      return Json(new { 
  success = false, 
  message = "Vui lòng nhập mã giao dịch chuyển khoản!" 
            });
        }

    // Tạo record ThanhToan
    var thanhToan = new ThanhToan
    {
          HoaDonId = hoaDonId,
             NgayThanhToan = DateTime.Now,
  SoTien = hoaDon.TongTien ?? 0,
   PhuongThucThanhToan = phuongThuc,
    MaGiaoDich = maGiaoDich,
  TrangThaiThanhToan = 1, // 1 = Thành công
   NguoiThucHien = Session["HoVaTen"]?.ToString(),
        GhiChu = phuongThuc == 0 
           ? "Thanh toán tiền mặt" 
            : $"Chuyển khoản - Mã GD: {maGiaoDich}"
       };
    db.ThanhToans.Add(thanhToan);

  // Cập nhật hóa đơn
      hoaDon.TrangThaiHoaDon = 1; // Đã thanh toán
                hoaDon.NgayThanhToan = DateTime.Now;
      hoaDon.PhuongThucThanhToan = phuongThuc == 0 ? "Tiền mặt" : "Chuyển khoản";

                // Cập nhật đơn đặt phòng
              if (hoaDon.DatPhong != null)
                {
           hoaDon.DatPhong.TrangThaiThanhToan = 2; // Đã thanh toán đủ
          hoaDon.DatPhong.HinhThucThanhToan = phuongThuc;
         hoaDon.DatPhong.NgayCapNhat = DateTime.Now;
        }

          db.SaveChanges();

   return Json(new { 
                 success = true, 
    message = $"Thanh toán thành công! Số tiền: {thanhToan.SoTien:N0}đ" +
        (phuongThuc == 1 ? $" (Mã GD: {maGiaoDich})" : "")
    });
       }
            catch (Exception ex)
        {
  System.Diagnostics.Debug.WriteLine($"[ERROR - HoaDon/ThanhToan] {ex.Message}");
       return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
    }
     }

        // HELPER METHODS

        private HoaDonItemViewModel MapToHoaDonItemViewModel(HoaDon hoaDon)
        {
  return new HoaDonItemViewModel
 {
                HoaDonId = hoaDon.HoaDonId,
   MaHoaDon = hoaDon.MaHoaDon,
          NgayLap = hoaDon.NgayLap,
       TongTien = hoaDon.TongTien,
    GiamGia = hoaDon.GiamGia,
                Thue = hoaDon.Thue,
                ThanhToanCuoi = hoaDon.ThanhToanCuoi,
TrangThaiHoaDon = hoaDon.TrangThaiHoaDon,
           TrangThaiHoaDonText = GetTrangThaiHoaDonText(hoaDon.TrangThaiHoaDon),
     TrangThaiHoaDonColor = GetTrangThaiHoaDonColor(hoaDon.TrangThaiHoaDon),
 PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
  NgayThanhToan = hoaDon.NgayThanhToan,
      NguoiLapHoaDon = hoaDon.NguoiLapHoaDon,
           GhiChu = hoaDon.GhiChu,
      DatPhongId = hoaDon.DatPhongId,
      MaDatPhong = hoaDon.DatPhong?.MaDatPhong,
             MaKhachHang = hoaDon.MaKhachHang,
         TenKhachHang = hoaDon.KhachHang?.HoVaTen,
SoDienThoaiKhachHang = hoaDon.KhachHang?.SoDienThoai,
              SoLanThanhToan = hoaDon.ThanhToans?.Count ?? 0,
         DanhSachThanhToan = hoaDon.ThanhToans?.Select(tt => MapToThanhToanItemViewModel(tt)).ToList(),
            NgayTao = hoaDon.NgayTao
            };
        }

        private HoaDonDetailsViewModel MapToHoaDonDetailsViewModel(HoaDon hoaDon)
     {
            var viewModel = new HoaDonDetailsViewModel
            {
 HoaDonId = hoaDon.HoaDonId,
       MaHoaDon = hoaDon.MaHoaDon,
    NgayLap = hoaDon.NgayLap,
     TongTien = hoaDon.TongTien,
        GiamGia = hoaDon.GiamGia,
      Thue = hoaDon.Thue,
        ThanhToanCuoi = hoaDon.ThanhToanCuoi,
        TrangThaiHoaDon = hoaDon.TrangThaiHoaDon,
      TrangThaiHoaDonText = GetTrangThaiHoaDonText(hoaDon.TrangThaiHoaDon),
     TrangThaiHoaDonColor = GetTrangThaiHoaDonColor(hoaDon.TrangThaiHoaDon),
   PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
       NgayThanhToan = hoaDon.NgayThanhToan,
       NguoiLapHoaDon = hoaDon.NguoiLapHoaDon,
          GhiChu = hoaDon.GhiChu,
         DatPhongId = hoaDon.DatPhongId,
           MaDatPhong = hoaDon.DatPhong?.MaDatPhong,
       NgayNhan = hoaDon.DatPhong?.NgayNhan,
     NgayTra = hoaDon.DatPhong?.NgayTra,
                SoDem = hoaDon.DatPhong?.SoDem,
   MaKhachHang = hoaDon.MaKhachHang,
            TenKhachHang = hoaDon.KhachHang?.HoVaTen,
         SoDienThoaiKhachHang = hoaDon.KhachHang?.SoDienThoai,
   EmailKhachHang = hoaDon.KhachHang?.Email,
           DiaChiKhachHang = hoaDon.KhachHang?.DiaChi,
           NgayTao = hoaDon.NgayTao
     };

 // Chi tiết phòng
            if (hoaDon.DatPhong?.ChiTietDatPhongs != null)
 {
   viewModel.DanhSachPhong = hoaDon.DatPhong.ChiTietDatPhongs.Select(ct => new ChiTietPhongHoaDonViewModel
       {
   TenLoaiPhong = ct.LoaiPhong?.TenLoai,
  MaPhong = ct.Phong?.MaPhong,
         SoLuong = ct.SoLuong,
DonGia = ct.DonGia ?? 0,
SoDem = hoaDon.DatPhong.SoDem ?? 0,
    GiamGia = ct.GiamGia ?? 0,
  ThanhTien = ct.ThanhTien ?? 0
         }).ToList();

                viewModel.TongTienPhong = viewModel.DanhSachPhong.Sum(p => p.ThanhTien);
    }

      // Chi tiết dịch vụ
            if (hoaDon.DatPhong?.ChiTietDatDichVus != null)
       {
     viewModel.DanhSachDichVu = hoaDon.DatPhong.ChiTietDatDichVus.Select(dv => new ChiTietDichVuHoaDonViewModel
        {
          TenDichVu = dv.DichVu?.TenDichVu,
  SoLuong = dv.SoLuong,
        DonGia = dv.DonGia ?? 0,
        ThanhTien = dv.ThanhTien ?? 0,
     NgaySuDung = dv.NgaySuDung
                }).ToList();

         viewModel.TongTienDichVu = viewModel.DanhSachDichVu.Sum(dv => dv.ThanhTien);
  }

            // Lịch sử thanh toán
        if (hoaDon.ThanhToans != null)
 {
      viewModel.LichSuThanhToan = hoaDon.ThanhToans
                    .OrderByDescending(tt => tt.NgayThanhToan)
  .Select(tt => MapToThanhToanItemViewModel(tt))
      .ToList();
       }

            return viewModel;
   }

        private ThanhToanItemViewModel MapToThanhToanItemViewModel(ThanhToan thanhToan)
        {
            return new ThanhToanItemViewModel
 {
                ThanhToanId = thanhToan.ThanhToanId,
          NgayThanhToan = thanhToan.NgayThanhToan,
           SoTien = thanhToan.SoTien,
            PhuongThucThanhToan = thanhToan.PhuongThucThanhToan,
  PhuongThucText = GetPhuongThucThanhToanText(thanhToan.PhuongThucThanhToan),
 MaGiaoDich = thanhToan.MaGiaoDich,
TrangThaiThanhToan = thanhToan.TrangThaiThanhToan,
      TrangThaiText = GetTrangThaiThanhToanText(thanhToan.TrangThaiThanhToan),
     TrangThaiColor = GetTrangThaiThanhToanColor(thanhToan.TrangThaiThanhToan),
     NguoiThucHien = thanhToan.NguoiThucHien,
       GhiChu = thanhToan.GhiChu,
                HinhAnhBienLai = thanhToan.HinhAnhBienLai
     };
   }

        private string GetTrangThaiHoaDonText(byte trangThai)
        {
            switch (trangThai)
            {
       case 0: return "Chưa thanh toán";
      case 1: return "Đã thanh toán";
default: return "Không xác định";
            }
      }

      private string GetTrangThaiHoaDonColor(byte trangThai)
      {
      switch (trangThai)
 {
        case 0: return "#dc3545"; // danger - đỏ
                case 1: return "#28a745"; // success - xanh lá
    default: return "#6c757d"; // secondary - xám
         }
    }

        private string GetPhuongThucThanhToanText(byte? phuongThuc)
        {
     if (!phuongThuc.HasValue) return "-";
   switch (phuongThuc.Value)
    {
      case 0: return "Tiền mặt";
        case 1: return "Chuyển khoản";
       case 2: return "Online";
     default: return "Khác";
            }
        }

   private string GetTrangThaiThanhToanText(byte trangThai)
    {
   switch (trangThai)
      {
                case 0: return "Chờ xử lý";
           case 1: return "Thành công";
            case 2: return "Thất bại";
             default: return "Không xác định";
            }
   }

        private string GetTrangThaiThanhToanColor(byte trangThai)
        {
switch (trangThai)
            {
 case 0: return "#ffc107"; // warning - vàng
      case 1: return "#28a745"; // success - xanh lá
     case 2: return "#dc3545"; // danger - đỏ
      default: return "#6c757d"; // secondary - xám
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
