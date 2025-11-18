using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý đặt phòng cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class DatPhongController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        private bool CheckRole()
        {
    string vaiTro = Session["VaiTro"]?.ToString();
      return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
    }

 // GET: Index
        public ActionResult Index(DatPhongFilterViewModel filter)
        {
  try
   {
    if (!CheckRole())
                {
     TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
 return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
      }

   var viewModel = new DatPhongListViewModel();
    viewModel.Filter = filter ?? new DatPhongFilterViewModel();

    var query = db.DatPhongs.AsQueryable();

 // Filters
     if (!string.IsNullOrWhiteSpace(filter.TimKiem))
      {
       string searchTerm = filter.TimKiem.Trim().ToLower();
        query = query.Where(dp =>
        dp.MaDatPhong.ToLower().Contains(searchTerm) ||
  dp.KhachHang.HoVaTen.ToLower().Contains(searchTerm) ||
 dp.KhachHang.SoDienThoai.Contains(searchTerm));
  }

         if (filter.TrangThaiDatPhong.HasValue)
        query = query.Where(dp => dp.TrangThaiDatPhong == filter.TrangThaiDatPhong.Value);

if (filter.TrangThaiThanhToan.HasValue)
    query = query.Where(dp => dp.TrangThaiThanhToan == filter.TrangThaiThanhToan.Value);

         if (filter.TuNgay.HasValue)
query = query.Where(dp => dp.NgayNhan >= filter.TuNgay.Value);

      if (filter.DenNgay.HasValue)
   query = query.Where(dp => dp.NgayNhan <= filter.DenNgay.Value);

      if (filter.LoaiPhongId.HasValue)
           query = query.Where(dp => dp.ChiTietDatPhongs.Any(ct => ct.LoaiPhongId == filter.LoaiPhongId.Value));

    // Statistics
    viewModel.ThongKe = new DatPhongStatViewModel
    {
  TongDonDat = db.DatPhongs.Count(),
    ChoXacNhan = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 0),
       DaXacNhan = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 1),
        DaCheckIn = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 2),
        DaCheckOut = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 3),
       DaHuy = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 4),
DoanhThuDuKien = db.DatPhongs.Where(dp => dp.TrangThaiDatPhong < 3).Sum(dp => (decimal?)dp.TongTien) ?? 0,
             DaThanhToan = db.DatPhongs.Where(dp => dp.TrangThaiThanhToan == 2).Sum(dp => (decimal?)dp.TongTien) ?? 0
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

     var datPhongs = query
 .Include(dp => dp.KhachHang)
     .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
     .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
     .Include(dp => dp.NhanVien)    // ✅ THÊM MỚI: Load thông tin nhân viên
     .Include(dp => dp.KhuyenMai)   // ✅ THÊM MỚI: Load thông tin khuyến mãi
    .OrderByDescending(dp => dp.NgayTao)
       .Skip((currentPage - 1) * pageSize)
    .Take(pageSize)
         .ToList();

   viewModel.DanhSachDatPhong = datPhongs.Select(dp => MapToDatPhongItemViewModel(dp)).ToList();

     return View(viewModel);
   }
        catch (Exception ex)
      {
         System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Index] {ex.Message}");
     TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đơn đặt phòng!";
return View(new DatPhongListViewModel());
            }
  }

        // GET: Create
        public ActionResult Create()
        {
      try
 {
  if (!CheckRole())
          {
  TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
         return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
        }

 var viewModel = new DatPhongCreateViewModel
       {
    NgayNhan = DateTime.Now.AddDays(1),
  NgayTra = DateTime.Now.AddDays(2),
         SoLuongKhach = 1,
        HinhThucThanhToan = 0
        };

                LoadLoaiPhongForCreate(viewModel);
       return View(viewModel);
            }
       catch (Exception ex)
            {
         System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Create GET] {ex.Message}");
    TempData["ErrorMessage"] = " Có lỗi xảy ra!";
                return RedirectToAction("Index");
    }
        }

     // POST: Create
  [HttpPost]
        [ValidateAntiForgeryToken]
public ActionResult Create(DatPhongCreateViewModel model)
        {
  try
  {
      if (!CheckRole())
         {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
            return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
     }

        if (!ModelState.IsValid)
       {
 LoadLoaiPhongForCreate(model);
                    return View(model);
 }

        if (!model.IsValidDateRange())
     {
        ModelState.AddModelError("NgayTra", "Ngày trả phải sau ngày nhận!");
        LoadLoaiPhongForCreate(model);
         return View(model);
        }

          if (!model.HasRooms())
        {
      ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng!");
         LoadLoaiPhongForCreate(model);
             return View(model);
   }

                var khachHang = FindOrCreateKhachHang(model);

      var datPhong = new DatPhong
    {
       MaDatPhong = GenerateMaDatPhong(),
     MaKhachHang = khachHang.MaKhachHang,
    NhanVienId = Session["NhanVienId"] != null ? (int?)Convert.ToInt32(Session["NhanVienId"]) : null,
          NgayDat = DateTime.Now,
        NgayNhan = model.NgayNhan,
     NgayTra = model.NgayTra,
    SoDem = model.SoDem,
   SoLuongKhach = model.SoLuongKhach,
     TrangThaiDatPhong = 0,
    TrangThaiThanhToan = 0, // Chưa thanh toán
      TongTien = model.TongCong,
          HinhThucThanhToan = model.HinhThucThanhToan,
  MaKhuyenMai = model.MaKhuyenMaiId,
      GhiChu = BuildGhiChu(model),
    NgayTao = DateTime.Now
      };

 db.DatPhongs.Add(datPhong);
     db.SaveChanges();

    foreach (var phongItem in model.DanhSachPhongDat.Where(p => p.SoLuong > 0))
    {
    var chiTiet = new ChiTietDatPhong
           {
   DatPhongId = datPhong.DatPhongId,
          LoaiPhongId = phongItem.LoaiPhongId,
     PhongId = null,
        DonGia = phongItem.DonGia,
      SoLuong = phongItem.SoLuong,
     NgayDen = model.NgayNhan,
       NgayDi = model.NgayTra,
          GiamGia = phongItem.ThanhTien * (phongItem.GiamGia / 100),
             ThanhTien = phongItem.ThanhTien,
  TrangThaiPhong = 1,
     NgayCapNhat = DateTime.Now
      };
 db.ChiTietDatPhongs.Add(chiTiet);
                }

 db.SaveChanges();

    TempData["SuccessMessage"] = $"Tạo đơn đặt phòng {datPhong.MaDatPhong} thành công!";
        return RedirectToAction("Index");
            }
   catch (Exception ex)
       {
       System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Create POST] {ex.Message}");
        TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo đơn đặt phòng!";
     LoadLoaiPhongForCreate(model);
           return View(model);
            }
        }

        // Helper Methods
    private void LoadLoaiPhongForCreate(DatPhongCreateViewModel model)
        {
   var loaiPhongs = db.LoaiPhongs.OrderBy(lp => lp.LoaiPhongId).ToList();

            model.DanhSachPhongDat = loaiPhongs.Select(lp => new PhongDatItemViewModel
    {
    LoaiPhongId = lp.LoaiPhongId,
                TenLoaiPhong = lp.TenLoai,
      MoTa = lp.MoTa,
           SoNguoiToiDa = lp.SoNguoiToiDa,
        DienTich = lp.DienTich,
        DonGia = lp.GiaCoBan ?? 0,
    SoLuong = 0,
      GiamGia = 0,
      SoDem = model.SoDem,
                SoPhongTrong = db.Phongs.Count(p => p.LoaiPhongId == lp.LoaiPhongId && p.TrangThaiPhong == 0 && p.DaHoatDong)
   }).ToList();
        }

      private KhachHang FindOrCreateKhachHang(DatPhongCreateViewModel model)
      {
  var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.SoDienThoai == model.SoDienThoai);

        if (khachHang == null)
     {
 khachHang = new KhachHang
     {
    HoVaTen = model.HoVaTen,
   SoDienThoai = model.SoDienThoai,
        Email = model.Email,
  DiaChi = model.DiaChi,
   GioiTinh = model.GioiTinh,
        DaDongYDieuKhoan = true,
          NgayTao = DateTime.Now
    };
    db.KhachHangs.Add(khachHang);
            db.SaveChanges();
  }
         else
        {
                khachHang.HoVaTen = model.HoVaTen;
                khachHang.Email = model.Email ?? khachHang.Email;
       khachHang.DiaChi = model.DiaChi ?? khachHang.DiaChi;
     khachHang.GioiTinh = model.GioiTinh ?? khachHang.GioiTinh;
      khachHang.NgayCapNhat = DateTime.Now;
        db.SaveChanges();
}

         return khachHang;
   }

     private string GenerateMaDatPhong()
        {
            var lastMa = db.DatPhongs.OrderByDescending(dp => dp.DatPhongId).Select(dp => dp.MaDatPhong).FirstOrDefault();
   if (string.IsNullOrEmpty(lastMa)) return "DP0001";
          var number = int.Parse(lastMa.Substring(2)) + 1;
        return $"DP{number:D4}";
        }

        private string BuildGhiChu(DatPhongCreateViewModel model)
        {
            var ghiChu = model.GhiChu ?? "";
            var tangInfo = new List<string>();

        foreach (var phong in model.DanhSachPhongDat.Where(p => p.SoLuong > 0))
        {
      if (phong.Tang.HasValue)
      tangInfo.Add($"{phong.TenLoaiPhong} - Tầng {phong.Tang.Value}");
            }

            if (tangInfo.Any())
     ghiChu += (string.IsNullOrEmpty(ghiChu) ? "" : "\n") + "[Ưu tiên tầng]\n" + string.Join("\n", tangInfo);
            else if (model.TangUuTien.HasValue)
    ghiChu += (string.IsNullOrEmpty(ghiChu) ? "" : "\n") + $"[Ưu tiên tầng {model.TangUuTien.Value}]";

         return ghiChu;
        }

        private DatPhongItemViewModel MapToDatPhongItemViewModel(DatPhong datPhong)
        {
            var chiTietPhongs = datPhong.ChiTietDatPhongs.ToList();

            return new DatPhongItemViewModel
      {
          DatPhongId = datPhong.DatPhongId,
    MaDatPhong = datPhong.MaDatPhong,
     NgayDat = datPhong.NgayDat,
        NgayNhan = datPhong.NgayNhan ?? DateTime.Now,
     NgayTra = datPhong.NgayTra ?? DateTime.Now.AddDays(1),
  SoDem = datPhong.SoDem ?? 0,
    MaKhachHang = datPhong.MaKhachHang,
    TenKhachHang = datPhong.KhachHang?.HoVaTen ?? "N/A",
     SoDienThoai = datPhong.KhachHang?.SoDienThoai,
     Email = datPhong.KhachHang?.Email,
         CustomerName = datPhong.CustomerName,
     SoLuongPhong = chiTietPhongs.Sum(ct => ct.SoLuong),
      DanhSachLoaiPhong = string.Join(", ", chiTietPhongs.GroupBy(ct => ct.LoaiPhong?.TenLoai).Select(g => $"{g.Key} x{g.Sum(ct => ct.SoLuong)}")),
    DanhSachPhong = chiTietPhongs.Any(ct => ct.PhongId.HasValue) ? string.Join(", ", chiTietPhongs.Where(ct => ct.Phong != null).Select(ct => ct.Phong.MaPhong)) : "Chưa gán",
    SoLuongKhach = datPhong.SoLuongKhach,
                TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
             TrangThaiDatPhongText = GetTrangThaiDatPhongText(datPhong.TrangThaiDatPhong),
      TrangThaiDatPhongColor = GetTrangThaiDatPhongColor(datPhong.TrangThaiDatPhong),
          TrangThaiThanhToan = datPhong.TrangThaiThanhToan,
      TrangThaiThanhToanText = GetTrangThaiThanhToanText(datPhong.TrangThaiThanhToan),
                TrangThaiThanhToanColor = GetTrangThaiThanhToanColor(datPhong.TrangThaiThanhToan),
         TongTien = datPhong.TongTien,
HinhThucThanhToan = datPhong.HinhThucThanhToan,
MaKhuyenMai = datPhong.MaKhuyenMai,
         PaymentRefId = datPhong.PaymentRefId,
           PaymentMethod = datPhong.PaymentMethod,
         OnlinePaymentStatus = datPhong.OnlinePaymentStatus,
      TotalAmount = datPhong.TotalAmount,
GhiChu = datPhong.GhiChu,
  NgayTao = datPhong.NgayTao,
    NgayCapNhat = datPhong.NgayCapNhat,
        NhanVienId = datPhong.NhanVienId,
        // ✅ THÊM MỚI: Sử dụng Navigation Property
        TenNhanVien = datPhong.NhanVien?.HoVaTen,
 TenKhuyenMai = datPhong.KhuyenMai?.TenKhuyenMai,
        MaKhuyenMaiCode = datPhong.KhuyenMai?.MaKhuyenMai,
        GiaTriKhuyenMai = datPhong.KhuyenMai?.GiaTri
};
        }

        private string GetTrangThaiDatPhongText(byte trangThai)
        {
          switch (trangThai)
   {
            case 0: return "Chờ xác nhận";
   case 1: return "Đã xác nhận";
      case 2: return "Đã check-in";
         case 3: return "Đã check-out";
         case 4: return "Đã hủy";
      default: return "Không xác định";
        }
        }

      private string GetTrangThaiDatPhongColor(byte trangThai)
    {
            switch (trangThai)
        {
    case 0: return "#ffc107";
 case 1: return "#17a2b8";
     case 2: return "#28a745";
          case 3: return "#6c757d";
case 4: return "#dc3545";
                default: return "#343a40";
            }
 }

      private string GetTrangThaiThanhToanText(byte trangThai)
      {
    switch (trangThai)
            {
                case 0: return "Chưa thanh toán";
     case 1: return "Đã thanh toán một phần";
             case 2: return "Đã thanh toán đầy đủ";
     default: return "Không xác định";
         }
        }

        private string GetTrangThaiThanhToanColor(byte trangThai)
        {
            switch (trangThai)
     {
  case 0: return "#dc3545";
      case 1: return "#ffc107";
         case 2: return "#28a745";
    default: return "#6c757d";
      }
        }

        // ===== AJAX ACTIONS =====
        
        /// <summary>
/// Tìm khách hàng theo số điện thoại (AJAX)
        /// </summary>
  [HttpPost]
        public JsonResult TimKhachHang(string sdt)
        {
       try
  {
    if (!CheckRole())
{
    return Json(new { success = false, message = "Bạn không có quyền!" });
  }

     if (string.IsNullOrWhiteSpace(sdt))
      {
     return Json(new { success = false, message = "Vui lòng nhập số điện thoại!" });
       }

  var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.SoDienThoai == sdt.Trim());

 if (khachHang != null)
      {
             return Json(new
       {
    success = true,
         data = new
       {
              khachHang.MaKhachHang,
    khachHang.HoVaTen,
      khachHang.Email,
        khachHang.DiaChi,
            khachHang.GioiTinh
    }
       });
                }
         else
                {
       return Json(new { success = false, message = "Không tìm thấy khách hàng" });
  }
       }
            catch (Exception ex)
 {
  System.Diagnostics.Debug.WriteLine($"[ERROR - TimKhachHang] {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra!" });
         }
}

        /// <summary>
/// Lấy danh sách tầng còn phòng trống theo loại phòng (AJAX)
        /// </summary>
  [HttpPost]
        public JsonResult GetAvailableFloors(int loaiPhongId, DateTime ngayNhan, DateTime ngayTra)
   {
   try
            {
     if (!CheckRole())
    {
                  return Json(new { success = false, message = "Bạn không có quyền!" });
   }

         // Lấy các phòng trống theo loại trong khoảng thời gian
    var phongTrongTheoTang = db.Phongs
         .Where(p => p.LoaiPhongId == loaiPhongId && p.DaHoatDong)
   .Where(p => !p.ChiTietDatPhongs.Any(ct =>
                ct.DatPhong.TrangThaiDatPhong != 4 && // Không bị hủy
    (
         (ct.NgayDen <= ngayNhan && ct.NgayDi > ngayNhan) || // Overlap check-in
       (ct.NgayDen < ngayTra && ct.NgayDi >= ngayTra) ||   // Overlap check-out
        (ct.NgayDen >= ngayNhan && ct.NgayDi <= ngayTra)    // Inside range
  )
               ))
       .GroupBy(p => p.Tang)
         .Select(g => new
     {
          Tang = g.Key,
        SoPhongTrong = g.Count()
        })
            .OrderBy(x => x.Tang)
          .ToList();

       if (phongTrongTheoTang.Any())
  {
       return Json(new { success = true, data = phongTrongTheoTang });
 }
     else
     {
    return Json(new { success = false, message = "Không có phòng trống!" });
 }
            }
            catch (Exception ex)
            {
            System.Diagnostics.Debug.WriteLine($"[ERROR - GetAvailableFloors] {ex.Message}");
          return Json(new { success = false, message = "Có lỗi xảy ra!" });
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
