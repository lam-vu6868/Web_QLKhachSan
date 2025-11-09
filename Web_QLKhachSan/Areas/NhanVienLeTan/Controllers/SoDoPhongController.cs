using System;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels;
using Web_QLKhachSan.Filters; // ✅ THÊM DÒNG NÀY

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý Sơ đồ Phòng cho Nhân viên Lễ Tân
    /// Hiển thị trạng thái phòng real-time, cho phép cập nhật trạng thái
    /// </summary>
    [NhanVienAuthorize] // ✅ THÊM ATTRIBUTE NÀY
    public class SoDoPhongController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        /// <summary>
        /// Kiểm tra authorization - vai trò
        /// </summary>
     private bool CheckRole()
        {
        // Kiểm tra vai trò (chỉ Lễ Tân, Admin mới được truy cập)
          string vaiTro = Session["VaiTro"]?.ToString();
      return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        // GET: NhanVienLeTan/SoDoPhong
        /// <summary>
 /// Hiển thị sơ đồ phòng với filters
        /// </summary>
        public ActionResult Index(int? locTheoTang, byte? locTheoTrangThai, int? locTheoLoaiPhong, string timKiem)
        {
         try
            {
 // ✅ Chỉ cần kiểm tra Role, không cần kiểm tra Auth nữa (Filter đã làm)
        if (!CheckRole())
     {
      TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này!";
   return RedirectToAction("Index", "Home");
        }

                var viewModel = new SoDoPhongViewModel();

      // Lấy tất cả phòng đang hoạt động
        var phongQuery = db.Phongs.Where(p => p.DaHoatDong);

        // Apply filters
  if (locTheoTang.HasValue && locTheoTang.Value > 0)
        {
            phongQuery = phongQuery.Where(p => p.Tang == locTheoTang.Value);
        }

        if (locTheoTrangThai.HasValue)
        {
            phongQuery = phongQuery.Where(p => p.TrangThaiPhong == locTheoTrangThai.Value);
   }

        if (locTheoLoaiPhong.HasValue && locTheoLoaiPhong.Value > 0)
        {
 phongQuery = phongQuery.Where(p => p.LoaiPhongId == locTheoLoaiPhong.Value);
     }

        if (!string.IsNullOrWhiteSpace(timKiem))
  {
      timKiem = timKiem.Trim().ToLower();
            phongQuery = phongQuery.Where(p =>
       p.MaPhong.ToLower().Contains(timKiem) ||
     p.TenPhong.ToLower().Contains(timKiem)
      );
        }

     // Lấy danh sách phòng và map sang ViewModel
        var danhSachPhong = phongQuery
            .OrderBy(p => p.Tang)
          .ThenBy(p => p.MaPhong)
            .ToList();

        viewModel.DanhSachPhong = danhSachPhong.Select(p => MapPhongToViewModel(p)).ToList();

        // Lấy danh sách loại phòng cho dropdown filter
        viewModel.DanhSachLoaiPhong = db.LoaiPhongs
            .Where(lp => lp.Phongs.Any(p => p.DaHoatDong))
         .Select(lp => new LoaiPhongDropdownViewModel
     {
      LoaiPhongId = lp.LoaiPhongId,
 TenLoai = lp.TenLoai,
      SoPhong = lp.Phongs.Count(p => p.DaHoatDong)
            })
  .ToList();

        // Giữ lại filters
        viewModel.LocTheoTang = locTheoTang;
        viewModel.LocTheoTrangThai = locTheoTrangThai;
        viewModel.LocTheoLoaiPhong = locTheoLoaiPhong;
        viewModel.TimKiem = timKiem;

        return View(viewModel);
  }
            catch (Exception ex)
       {
     // Log error
    System.Diagnostics.Debug.WriteLine($"[ERROR - SoDoPhong/Index] {ex.Message}");
      System.Diagnostics.Debug.WriteLine($"[ERROR - StackTrace] {ex.StackTrace}");

                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải sơ đồ phòng. Vui lòng thử lại!";
   
      // Return empty view model
   return View(new SoDoPhongViewModel 
  { 
    DanhSachPhong = new System.Collections.Generic.List<PhongStatusViewModel>(),
        DanhSachLoaiPhong = new System.Collections.Generic.List<LoaiPhongDropdownViewModel>()
        });
     }
        }

 /// <summary>
        /// Map từ Entity Phong sang PhongStatusViewModel
     /// </summary>
    private PhongStatusViewModel MapPhongToViewModel(Phong phong)
        {
  if (phong == null)
return null;

   var viewModel = new PhongStatusViewModel
   {
                PhongId = phong.PhongId,
      MaPhong = phong.MaPhong ?? "N/A",
    TenPhong = phong.TenPhong ?? "N/A",
                Tang = phong.Tang ?? 1,
    TrangThaiPhong = phong.TrangThaiPhong,
        LoaiPhongId = phong.LoaiPhongId,
         TenLoaiPhong = phong.LoaiPhong?.TenLoai ?? "Chưa xác định",
    GiaPhong = phong.Gia,
        SoNguoiToiDa = phong.LoaiPhong?.SoNguoiToiDa
            };

    // Nếu phòng đang được sử dụng hoặc đã đặt, lấy thông tin khách
        if (phong.TrangThaiPhong == 1 || phong.TrangThaiPhong == 2) // Đã đặt hoặc Đang ở
            {
         try
    {
       // Tìm đơn đặt phòng hiện tại
          var chiTietDatPhong = phong.ChiTietDatPhongs
  .Where(ct => ct.DatPhong != null &&
            (ct.DatPhong.TrangThaiDatPhong == 1 || ct.DatPhong.TrangThaiDatPhong == 2)) // Đã xác nhận hoặc Đã check-in
  .OrderByDescending(ct => ct.DatPhong.NgayTao)
     .FirstOrDefault();

              if (chiTietDatPhong != null && chiTietDatPhong.DatPhong != null)
       {
             var datPhong = chiTietDatPhong.DatPhong;
  var khachHang = datPhong.KhachHang;

         viewModel.DatPhongId = datPhong.DatPhongId;
   viewModel.MaKhachHang = datPhong.MaKhachHang;
     viewModel.TenKhach = khachHang?.HoVaTen;
         viewModel.SoDienThoaiKhach = khachHang?.SoDienThoai;
     viewModel.NgayCheckIn = chiTietDatPhong.NgayDen ?? datPhong.NgayNhan;
                 viewModel.NgayCheckOut = chiTietDatPhong.NgayDi ?? datPhong.NgayTra;
      viewModel.SoNguoi = datPhong.SoLuongKhach;
         viewModel.GhiChu = datPhong.GhiChu;
       }
            }
     catch (Exception ex)
          {
    System.Diagnostics.Debug.WriteLine($"[WARNING - MapPhongToViewModel] Error loading guest info for room {phong.PhongId}: {ex.Message}");
     }
    }

            return viewModel;
        }

        // POST: NhanVienLeTan/SoDoPhong/CapNhatTrangThai
/// <summary>
        /// Cập nhật trạng thái phòng (AJAX)
/// </summary>
        [HttpPost]
        public JsonResult CapNhatTrangThai(int phongId, byte trangThaiMoi)
        {
            try
            {
   // ✅ Kiểm tra Role
     if (!CheckRole())
        {
    return Json(new { success = false, message = "Bạn không có quyền thực hiện thao tác này!" });
        }

// Validate input
        if (phongId <= 0)
     {
            return Json(new { success = false, message = "Mã phòng không hợp lệ!" });
        }

        if (trangThaiMoi > 4)
  {
 return Json(new { success = false, message = "Trạng thái không hợp lệ!" });
   }

        // Tìm phòng
     var phong = db.Phongs.Find(phongId);
        if (phong == null)
   {
        return Json(new { success = false, message = "Không tìm thấy phòng!" });
        }

 // Lưu trạng thái cũ để log
        byte trangThaiCu = phong.TrangThaiPhong;

        // Kiểm tra logic chuyển trạng thái
     var errors = ValidateChuyenTrangThai(phong, trangThaiMoi);
   if (errors.Any())
        {
   return Json(new { success = false, message = string.Join("<br/>", errors) });
        }

        // Cập nhật trạng thái
        phong.TrangThaiPhong = trangThaiMoi;
        phong.NgayCapNhat = DateTime.Now;
    db.SaveChanges();

    // Log thay đổi
        string nhanVienId = Session["NhanVienId"]?.ToString() ?? "Unknown";
        string tenNhanVien = Session["HoVaTen"]?.ToString() ?? "Unknown";
        System.Diagnostics.Debug.WriteLine($"[INFO] Room {phong.MaPhong} status changed from {trangThaiCu} to {trangThaiMoi} by {tenNhanVien} (ID: {nhanVienId})");

      return Json(new
    {
   success = true,
   message = $"Đã cập nhật trạng thái phòng {phong.MaPhong} thành công!",
            trangThaiMoi = trangThaiMoi,
            trangThaiCu = trangThaiCu
        });
    }
    catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
    {
   System.Diagnostics.Debug.WriteLine($"[ERROR - CapNhatTrangThai] Database error: {dbEx.Message}");
    return Json(new { success = false, message = "Lỗi cơ sở dữ liệu. Vui lòng thử lại!" });
  }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR - CapNhatTrangThai] {ex.Message}");
        return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
    }
        }

    /// <summary>
        /// Validate logic chuyển trạng thái phòng
        /// </summary>
        private System.Collections.Generic.List<string> ValidateChuyenTrangThai(Phong phong, byte trangThaiMoi)
     {
            var errors = new System.Collections.Generic.List<string>();

        // Rule 1: Không cho phép chuyển phòng đang ở (2) về trống (0) trực tiếp
    if (phong.TrangThaiPhong == 2 && trangThaiMoi == 0)
 {
                errors.Add("❌ Phòng đang có khách. Vui lòng Check-out trước!");
}

  // Rule 2: Không cho phép chuyển phòng đã đặt (1) về trống (0) trực tiếp
      if (phong.TrangThaiPhong == 1 && trangThaiMoi == 0)
  {
       errors.Add("❌ Phòng đã có đặt chỗ. Vui lòng hủy đặt phòng trước!");
     }

            // Rule 3: Không cho phép chuyển trống (0) thành đang ở (2) trực tiếp
 if (phong.TrangThaiPhong == 0 && trangThaiMoi == 2)
     {
           errors.Add("❌ Không thể chuyển trực tiếp từ Trống sang Đang ở. Vui lòng sử dụng chức năng Check-in!");
    }

 // Rule 4: Cho phép chuyển từ đang dọn (3) về trống (0) - OK
  // Rule 5: Cho phép chuyển từ bảo trì (4) về trống (0) - OK

       // Rule 6: Không cho phép chuyển từ đang ở (2) sang bảo trì (4) trực tiếp
            if (phong.TrangThaiPhong == 2 && trangThaiMoi == 4)
  {
       errors.Add("❌ Phòng đang có khách. Không thể chuyển sang Bảo trì!");
        }

            // Rule 7: Cảnh báo nếu chuyển trạng thái giống nhau
      if (phong.TrangThaiPhong == trangThaiMoi)
 {
        errors.Add("ℹ️ Phòng đã ở trạng thái này rồi!");
        }

         return errors;
        }

// GET: NhanVienLeTan/SoDoPhong/ChiTietPhong
      /// <summary>
        /// Lấy chi tiết phòng (AJAX) - Dùng cho modal popup
        /// </summary>
      [HttpGet]
public JsonResult ChiTietPhong(int phongId)
    {
 try
          {
    // ✅ Kiểm tra Role
        if (!CheckRole())
        {
 return Json(new { success = false, message = "Bạn không có quyền xem thông tin này!" }, JsonRequestBehavior.AllowGet);
        }

        // Validate input
        if (phongId <= 0)
    {
 return Json(new { success = false, message = "Mã phòng không hợp lệ!" }, JsonRequestBehavior.AllowGet);
        }

        // Tìm phòng
        var phong = db.Phongs.Find(phongId);
      if (phong == null)
  {
     return Json(new { success = false, message = "Không tìm thấy phòng!" }, JsonRequestBehavior.AllowGet);
}

        // Map sang ViewModel
        var viewModel = MapPhongToViewModel(phong);

        if (viewModel == null)
        {
      return Json(new { success = false, message = "Không thể tải thông tin phòng!" }, JsonRequestBehavior.AllowGet);
   }

  return Json(new
        {
       success = true,
      data = viewModel
        }, JsonRequestBehavior.AllowGet);
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[ERROR - ChiTietPhong] {ex.Message}");
        return Json(new { success = false, message = "Có lỗi xảy ra khi tải thông tin phòng!" }, JsonRequestBehavior.AllowGet);
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
