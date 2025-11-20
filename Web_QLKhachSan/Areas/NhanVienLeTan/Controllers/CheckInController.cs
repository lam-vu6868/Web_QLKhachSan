using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckIn;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller qu?n lý Check-in cho Nhân viên L? Tân
    /// </summary>
    [NhanVienAuthorize]
    public class CheckInController : Controller
    {
      private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        private bool CheckRole()
        {
         string vaiTro = Session["VaiTro"]?.ToString();
return vaiTro == "LeTan" || vaiTro == "L? Tân" || vaiTro == "Admin" || vaiTro == "Qu?n lý";
        }

        /// <summary>
/// GET: CheckIn/Index
/// Hi?n th? danh sách ??n ??t phòng c?n check-in
        /// </summary>
        public ActionResult Index(CheckInFilterViewModel filter)
 {
            try
            {
  if (!CheckRole())
          {
  TempData["ErrorMessage"] = "B?n không có quy?n truy c?p!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
             }

  var viewModel = new CheckInListViewModel();
              viewModel.Filter = filter ?? new CheckInFilterViewModel();

                // Query ??n ??t phòng tr?ng thái = 1 (?ã xác nh?n, ch?a check-in)
                var query = db.DatPhongs
           .Where(dp => dp.TrangThaiDatPhong == 1); // ?ã xác nh?n, ch? check-in

  // Filter theo tìm ki?m
 if (!string.IsNullOrWhiteSpace(filter.TimKiem))
     {
        string searchTerm = filter.TimKiem.Trim().ToLower();
           query = query.Where(dp =>
     dp.MaDatPhong.ToLower().Contains(searchTerm) ||
         dp.KhachHang.HoVaTen.ToLower().Contains(searchTerm) ||
       dp.KhachHang.SoDienThoai.Contains(searchTerm));
                }

     // Filter theo ngày
   if (filter.NgayCheckIn.HasValue)
       {
var ngay = filter.NgayCheckIn.Value.Date;
         query = query.Where(dp => dp.NgayNhan.HasValue &&
         DbFunctions.TruncateTime(dp.NgayNhan) == ngay);
   }
        else
        {
  // M?c ??nh hi?n th? check-in hôm nay và ngày mai
      var homNay = DateTime.Now.Date;
          var ngayMai = homNay.AddDays(1);
       query = query.Where(dp => dp.NgayNhan.HasValue &&
       (DbFunctions.TruncateTime(dp.NgayNhan) == homNay ||
            DbFunctions.TruncateTime(dp.NgayNhan) == ngayMai));
      }

         // Th?ng kê
    var homNayDate = DateTime.Now.Date;
  viewModel.ThongKe = new CheckInStatViewModel
 {
CanCheckInHomNay = db.DatPhongs.Count(dp =>
           dp.TrangThaiDatPhong == 1 &&
          dp.NgayNhan.HasValue &&
       DbFunctions.TruncateTime(dp.NgayNhan) == homNayDate),
      DaCheckInHomNay = db.DatPhongs.Count(dp =>
  dp.TrangThaiDatPhong == 2 &&
  dp.NgayNhan.HasValue &&
  DbFunctions.TruncateTime(dp.NgayNhan) == homNayDate),
          QuaHan = db.DatPhongs.Count(dp =>
      dp.TrangThaiDatPhong == 1 &&
         dp.NgayNhan.HasValue &&
      DbFunctions.TruncateTime(dp.NgayNhan) < homNayDate),
  TongDonCho = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 1)
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

                // L?y danh sách ??n ??t phòng
                var datPhongs = query
            .Include(dp => dp.KhachHang)
         .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
      .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
     .OrderBy(dp => dp.NgayNhan)
         .ThenByDescending(dp => dp.NgayTao)
     .Skip((currentPage - 1) * pageSize)
           .Take(pageSize)
        .ToList();

  viewModel.DanhSachCheckIn = datPhongs.Select(dp => MapToCheckInItemViewModel(dp)).ToList();

          return View(viewModel);
   }
            catch (Exception ex)
   {
      System.Diagnostics.Debug.WriteLine($"[ERROR - CheckIn/Index] {ex.Message}");
        TempData["ErrorMessage"] = "Có l?i x?y ra khi t?i danh sách check-in!";
         return View(new CheckInListViewModel());
     }
        }

        /// <summary>
        /// POST: CheckIn/DoCheckIn
        /// Th?c hi?n check-in cho ??n ??t phòng
    /// </summary>
    [HttpPost]
        public JsonResult DoCheckIn(int datPhongId, int soNguoiThucTe, string ghiChu)
        {
    try
 {
              if (!CheckRole())
           {
    return Json(new { success = false, message = "B?n không có quy?n!" });
             }

          var datPhong = db.DatPhongs
  .Include(dp => dp.ChiTietDatPhongs)
   .Include(dp => dp.KhachHang)
        .FirstOrDefault(dp => dp.DatPhongId == datPhongId);

  if (datPhong == null)
          {
         return Json(new { success = false, message = "Không tìm th?y ??n ??t phòng!" });
     }

      // Ki?m tra tr?ng thái
                if (datPhong.TrangThaiDatPhong != 1)
    {
             return Json(new { success = false, message = "Ch? check-in ???c ??n ?ã xác nh?n!" });
          }

                // Ki?m tra ?ã gán phòng ch?a
       if (!datPhong.ChiTietDatPhongs.Any(ct => ct.PhongId.HasValue))
      {
      return Json(new { success = false, message = "Ch?a gán phòng cho ??n này!" });
           }

          // C?p nh?t s? ng??i th?c t?
                if (soNguoiThucTe > 0 && soNguoiThucTe != datPhong.SoLuongKhach)
           {
      datPhong.SoLuongKhach = soNguoiThucTe;
            }

          // Thêm ghi chú check-in
           if (!string.IsNullOrWhiteSpace(ghiChu))
    {
      datPhong.GhiChu = (datPhong.GhiChu ?? "") + 
 $"\n[CHECK-IN] {DateTime.Now:dd/MM/yyyy HH:mm} - {ghiChu}";
            }

        // C?p nh?t ngày check-in th?c t?
        datPhong.NgayNhan = DateTime.Now;

         // C?p nh?t tr?ng thái phòng sang "?ang ?"
         foreach (var chiTiet in datPhong.ChiTietDatPhongs.Where(ct => ct.PhongId.HasValue))
  {
        var phong = db.Phongs.Find(chiTiet.PhongId.Value);
  if (phong != null)
    {
     phong.TrangThaiPhong = 2; // ?ang ?
        phong.NgayCapNhat = DateTime.Now;
         }

           chiTiet.TrangThaiPhong = 2;
     chiTiet.NgayDen = DateTime.Now;
      chiTiet.NgayCapNhat = DateTime.Now;
      }

              // C?p nh?t tr?ng thái ??n
     datPhong.TrangThaiDatPhong = 2; // ?ã check-in
    datPhong.NgayCapNhat = DateTime.Now;

             db.SaveChanges();

     var danhSachPhong = string.Join(", ", datPhong.ChiTietDatPhongs
          .Where(ct => ct.Phong != null)
            .Select(ct => ct.Phong.MaPhong));

  return Json(new
                {
   success = true,
       message = $"Check-in thành công cho khách {datPhong.KhachHang.HoVaTen}! Phòng: {danhSachPhong}",
         checkInTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
    });
  }
         catch (Exception ex)
            {
    System.Diagnostics.Debug.WriteLine($"[ERROR - DoCheckIn] {ex.Message}");
    return Json(new { success = false, message = "Có l?i x?y ra: " + ex.Message });
            }
     }

        /// <summary>
        /// GET: CheckIn/Details/{id}
     /// Xem chi ti?t ??n ??t phòng tr??c khi check-in
        /// </summary>
        public ActionResult Details(int id)
        {
       try
  {
             if (!CheckRole())
             {
      TempData["ErrorMessage"] = "B?n không có quy?n truy c?p!";
       return RedirectToAction("Index");
                }

  var datPhong = db.DatPhongs
  .Include(dp => dp.KhachHang)
    .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
           .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
          .Include(dp => dp.NhanVien)
          .FirstOrDefault(dp => dp.DatPhongId == id);

   if (datPhong == null)
       {
    TempData["ErrorMessage"] = "Không tìm th?y ??n ??t phòng!";
        return RedirectToAction("Index");
     }

          var viewModel = MapToCheckInItemViewModel(datPhong);

return View(viewModel);
    }
     catch (Exception ex)
            {
         System.Diagnostics.Debug.WriteLine($"[ERROR - CheckIn/Details] {ex.Message}");
       TempData["ErrorMessage"] = "Có l?i x?y ra!";
        return RedirectToAction("Index");
            }
    }

        // Helper Methods
        private CheckInItemViewModel MapToCheckInItemViewModel(DatPhong datPhong)
      {
          var chiTietPhongs = datPhong.ChiTietDatPhongs.ToList();

 return new CheckInItemViewModel
   {
        DatPhongId = datPhong.DatPhongId,
     MaDatPhong = datPhong.MaDatPhong,
    NgayDat = datPhong.NgayDat,
    NgayNhan = datPhong.NgayNhan,
             NgayTra = datPhong.NgayTra,
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
  : "Ch?a gán",
    
                // Tr?ng thái
      TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
     TongTien = datPhong.TongTien,
       GhiChu = datPhong.GhiChu,
        
     // Online payment
   LaDonOnline = !string.IsNullOrEmpty(datPhong.PaymentRefId),
OnlinePaymentStatus = datPhong.OnlinePaymentStatus,
       
     // Nhân viên
    NhanVienDatPhong = datPhong.NhanVien?.HoVaTen
        };
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
