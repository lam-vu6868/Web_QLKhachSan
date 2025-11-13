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

        /// <summary>
        /// Kiểm tra quyền truy cập
    /// </summary>
        private bool CheckRole()
        {
 string vaiTro = Session["VaiTro"]?.ToString();
      return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        // ===== GET: Index - Danh sách đơn đặt phòng =====
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

       // Lấy danh sách đơn đặt phòng
                var query = db.DatPhongs.AsQueryable();

          // ===== ÁP DỤNG BỘ LỌC =====
          // 1. Tìm kiếm theo mã đơn, tên khách, SĐT
                if (!string.IsNullOrWhiteSpace(filter.TimKiem))
    {
        string searchTerm = filter.TimKiem.Trim().ToLower();
    query = query.Where(dp =>
        dp.MaDatPhong.ToLower().Contains(searchTerm) ||
        dp.KhachHang.HoVaTen.ToLower().Contains(searchTerm) ||
     dp.KhachHang.SoDienThoai.Contains(searchTerm)
            );
        }

              // 2. Lọc theo trạng thái đặt phòng
          if (filter.TrangThaiDatPhong.HasValue)
             {
              query = query.Where(dp => dp.TrangThaiDatPhong == filter.TrangThaiDatPhong.Value);
            }

    // 3. Lọc theo trạng thái thanh toán
                if (filter.TrangThaiThanhToan.HasValue)
{
           query = query.Where(dp => dp.TrangThaiThanhToan == filter.TrangThaiThanhToan.Value);
    }

         // 4. Lọc theo khoảng ngày
         if (filter.TuNgay.HasValue)
       {
    query = query.Where(dp => dp.NgayNhan >= filter.TuNgay.Value);
                }
       if (filter.DenNgay.HasValue)
       {
          query = query.Where(dp => dp.NgayNhan <= filter.DenNgay.Value);
    }

  // 5. Lọc theo loại phòng
      if (filter.LoaiPhongId.HasValue)
     {
      query = query.Where(dp => dp.ChiTietDatPhongs.Any(ct => ct.LoaiPhongId == filter.LoaiPhongId.Value));
      }

     // ===== THỐNG KÊ NHANH =====
     viewModel.ThongKe = new DatPhongStatViewModel
       {
          TongDonDat = db.DatPhongs.Count(),
 ChoXacNhan = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 0),
    DaXacNhan = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 1),
  DaCheckIn = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 2),
DaCheckOut = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 3),
         DaHuy = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 4),
          DoanhThuDuKien = db.DatPhongs
            .Where(dp => dp.TrangThaiDatPhong < 3)
 .Sum(dp => (decimal?)dp.TongTien) ?? 0,
           DaThanhToan = db.DatPhongs
    .Where(dp => dp.TrangThaiThanhToan == 2)
        .Sum(dp => (decimal?)dp.TongTien) ?? 0
             };

        // ===== PAGINATION =====
     int totalRecords = query.Count();
       int pageSize = filter.PageSize > 0 ? filter.PageSize : 10; // Đổi từ 20 → 10
                int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;
           int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

           viewModel.TotalRecords = totalRecords;
        viewModel.PageSize = pageSize;
  viewModel.CurrentPage = currentPage;
             viewModel.TotalPages = totalPages;

     // ===== LẤY DATA CHO TRANG HIỆN TẠI =====
       var datPhongs = query
           .Include(dp => dp.KhachHang)
       .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
     .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
    .OrderByDescending(dp => dp.NgayTao)
          .Skip((currentPage - 1) * pageSize)
     .Take(pageSize)
     .ToList();

          // ===== MAP SANG VIEWMODEL =====
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

        // ===== GET: Create - Form tạo đơn mới =====
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
          NgayNhan = DateTime.Now.AddDays(1), // Mặc định ngày mai
         NgayTra = DateTime.Now.AddDays(2)   // Mặc định 1 đêm
      };

  // Load danh sách loại phòng
         LoadLoaiPhongForCreate(viewModel);

      return View(viewModel);
   }
            catch (Exception ex)
            {
System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Create GET] {ex.Message}");
  TempData["ErrorMessage"] = "Có lỗi xảy ra!";
     return RedirectToAction("Index");
     }
    }

        // ===== POST: Create - Xử lý tạo đơn =====
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

     // ===== VALIDATION =====
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

        // ===== TÌM HOẶC TẠO KHÁCH HÀNG =====
  var khachHang = FindOrCreateKhachHang(model);

      // ===== TẠO ĐƠN ĐẶT PHÒNG =====
       var datPhong = new Models.DatPhong
      {
      MaDatPhong = GenerateMaDatPhong(),
                 MaKhachHang = khachHang.MaKhachHang,
      NhanVienId = Session["NhanVienId"] != null ? (int?)Convert.ToInt32(Session["NhanVienId"]) : null,
          NgayDat = DateTime.Now,
      NgayNhan = model.NgayNhan,
  NgayTra = model.NgayTra,
         SoDem = model.SoDem,
SoLuongKhach = model.SoLuongKhach,
        TrangThaiDatPhong = 0, // Chờ xác nhận
     TrangThaiThanhToan = model.TienDatCoc.HasValue && model.TienDatCoc.Value > 0 ? (byte)1 : (byte)0,
   TongTien = model.TongCong,
         HinhThucThanhToan = model.HinhThucThanhToan,
 MaKhuyenMai = model.MaKhuyenMaiId,
          GhiChu = model.GhiChu,
        NgayTao = DateTime.Now
 };

      db.DatPhongs.Add(datPhong);
   db.SaveChanges();

        // ===== TẠO CHI TIẾT ĐẶT PHÒNG =====
foreach (var phongItem in model.DanhSachPhongDat.Where(p => p.SoLuong > 0))
          {
     var chiTiet = new ChiTietDatPhong
           {
         DatPhongId = datPhong.DatPhongId,
       LoaiPhongId = phongItem.LoaiPhongId,
     PhongId = null, // Sẽ gán khi check-in
             DonGia = phongItem.DonGia,
           SoLuong = phongItem.SoLuong,
     NgayDen = model.NgayNhan,
   NgayDi = model.NgayTra,
       GiamGia = phongItem.ThanhTien * (phongItem.GiamGia / 100),
        ThanhTien = phongItem.ThanhTien,
    TrangThaiPhong = 1, // Đã đặt
    NgayCapNhat = DateTime.Now
            };
    db.ChiTietDatPhongs.Add(chiTiet);
        }

    db.SaveChanges();

   TempData["SuccessMessage"] = $"Tạo đơn đặt phòng {datPhong.MaDatPhong} thành công!";
                return RedirectToAction("Details", new { id = datPhong.DatPhongId });
    }
     catch (Exception ex)
      {
     System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Create POST] {ex.Message}");
     TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo đơn đặt phòng!";
     LoadLoaiPhongForCreate(model);
         return View(model);
            }
        }

        // ===== GET: Details - Xem chi tiết đơn =====
        public ActionResult Details(int id)
        {
            try
            {
          if (!CheckRole())
       {
       TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
          return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

            var datPhong = db.DatPhongs
   .Include(dp => dp.KhachHang)
            .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
    .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.Phong))
       .Include(dp => dp.ChiTietDatDichVus.Select(dv => dv.DichVu))
           .FirstOrDefault(dp => dp.DatPhongId == id);

                if (datPhong == null)
        {
       TempData["ErrorMessage"] = "Không tìm thấy đơn đặt phòng!";
  return RedirectToAction("Index");
       }

   var viewModel = MapToDatPhongDetailViewModel(datPhong);

       return View(viewModel);
            }
            catch (Exception ex)
  {
    System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Details] {ex.Message}");
 TempData["ErrorMessage"] = "Có lỗi xảy ra!";
                return RedirectToAction("Index");
  }
        }

      // ===== POST: XacNhan - Xác nhận đơn đặt =====
        [HttpPost]
        public JsonResult XacNhan(int id)
        {
      try
 {
    var datPhong = db.DatPhongs.Find(id);
       if (datPhong == null)
           return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });

        if (datPhong.TrangThaiDatPhong != 0)
     return Json(new { success = false, message = "Đơn đã được xử lý!" });

 datPhong.TrangThaiDatPhong = 1; // Đã xác nhận
    datPhong.NgayCapNhat = DateTime.Now;

           // Cập nhật trạng thái chi tiết đặt phòng
  foreach (var chiTiet in datPhong.ChiTietDatPhongs)
              {
        chiTiet.TrangThaiPhong = 1; // Đã đặt
     }

          db.SaveChanges();

  return Json(new { success = true, message = "Xác nhận đơn đặt phòng thành công!" });
         }
 catch (Exception ex)
            {
    System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/XacNhan] {ex.Message}");
      return Json(new { success = false, message = "Có lỗi xảy ra!" });
        }
   }

      // ===== POST: Huy - Hủy đơn đặt =====
        [HttpPost]
        public JsonResult Huy(int id, string lyDo)
        {
            try
            {
    var datPhong = db.DatPhongs.Find(id);
    if (datPhong == null)
      return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });

         if (datPhong.TrangThaiDatPhong >= 2)
         return Json(new { success = false, message = "Không thể hủy đơn đã check-in!" });

            datPhong.TrangThaiDatPhong = 4; // Đã hủy
     datPhong.GhiChu = (datPhong.GhiChu ?? "") + $"\n[Hủy đơn] {lyDo} - {DateTime.Now:dd/MM/yyyy HH:mm}";
    datPhong.NgayCapNhat = DateTime.Now;

          db.SaveChanges();

   return Json(new { success = true, message = "Hủy đơn đặt phòng thành công!" });
      }
 catch (Exception ex)
       {
System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Huy] {ex.Message}");
    return Json(new { success = false, message = "Có lỗi xảy ra!" });
      }
    }

 // ===== HELPER METHODS =====

        /// <summary>
        /// Map DatPhong entity sang DatPhongItemViewModel
    /// </summary>
        private DatPhongItemViewModel MapToDatPhongItemViewModel(Models.DatPhong datPhong)
        {
       var chiTietPhongs = datPhong.ChiTietDatPhongs.ToList();

     return new DatPhongItemViewModel
            {
  DatPhongId = datPhong.DatPhongId,
        MaDatPhong = datPhong.MaDatPhong,
     NgayDat = datPhong.NgayDat,
       NgayNhan = datPhong.NgayNhan,
     NgayTra = datPhong.NgayTra,
  SoDem = datPhong.SoDem,
MaKhachHang = datPhong.MaKhachHang,
    TenKhachHang = datPhong.KhachHang?.HoVaTen ?? "N/A",
  SoDienThoai = datPhong.KhachHang?.SoDienThoai,
      Email = datPhong.KhachHang?.Email,
     SoLuongPhong = chiTietPhongs.Sum(ct => ct.SoLuong),
        DanhSachLoaiPhong = string.Join(", ", chiTietPhongs
            .GroupBy(ct => ct.LoaiPhong?.TenLoai)
       .Select(g => $"{g.Key} x{g.Sum(ct => ct.SoLuong)}")),
    DanhSachPhong = chiTietPhongs.Any(ct => ct.PhongId.HasValue)
  ? string.Join(", ", chiTietPhongs.Where(ct => ct.Phong != null).Select(ct => ct.Phong.MaPhong))
   : "Chưa gán",
     TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
       TrangThaiThanhToan = datPhong.TrangThaiThanhToan,
         TongTien = datPhong.TongTien,
  SoLuongKhach = datPhong.SoLuongKhach,
            GhiChu = datPhong.GhiChu
       };
        }

        /// <summary>
        /// Map DatPhong entity sang DatPhongDetailViewModel
    /// </summary>
        private DatPhongDetailViewModel MapToDatPhongDetailViewModel(Models.DatPhong datPhong)
        {
            var viewModel = new DatPhongDetailViewModel
            {
   DatPhongId = datPhong.DatPhongId,
        MaDatPhong = datPhong.MaDatPhong,
    NgayDat = datPhong.NgayDat,
           NgayTao = datPhong.NgayTao,
           NgayCapNhat = datPhong.NgayCapNhat,
     TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
             TrangThaiThanhToan = datPhong.TrangThaiThanhToan,
      NgayNhan = datPhong.NgayNhan,
                NgayTra = datPhong.NgayTra,
          SoDem = datPhong.SoDem,
                SoLuongKhach = datPhong.SoLuongKhach,
           GhiChu = datPhong.GhiChu,
                TongTienPhong = datPhong.ChiTietDatPhongs.Sum(ct => ct.ThanhTien ?? 0),
   TongTienDichVu = datPhong.ChiTietDatDichVus.Sum(dv => dv.ThanhTien ?? 0),
    TongCong = datPhong.TongTien ?? 0,
            HinhThucThanhToan = datPhong.HinhThucThanhToan == 0 ? "Tiền mặt" : 
                 datPhong.HinhThucThanhToan == 1 ? "Chuyển khoản" : "QR Code"
        };

            // Thông tin khách hàng
            if (datPhong.KhachHang != null)
      {
   viewModel.HoVaTen = datPhong.KhachHang.HoVaTen;
          viewModel.SoDienThoai = datPhong.KhachHang.SoDienThoai;
        viewModel.Email = datPhong.KhachHang.Email;
     viewModel.DiaChi = datPhong.KhachHang.DiaChi;
     }

     // Danh sách phòng
     viewModel.DanhSachPhong = datPhong.ChiTietDatPhongs.Select(ct => new PhongDetailItemViewModel
   {
       ChiTietDatPhongId = ct.ChiTietDatPhongId,
      TenLoaiPhong = ct.LoaiPhong?.TenLoai,
                MaPhong = ct.Phong?.MaPhong ?? "Chưa gán",
        PhongId = ct.PhongId,
 NgayDen = ct.NgayDen,
         NgayDi = ct.NgayDi,
  DonGia = ct.DonGia,
     SoLuong = ct.SoLuong,
        GiamGia = ct.GiamGia,
      ThanhTien = ct.ThanhTien,
      TrangThaiPhong = ct.TrangThaiPhong,
 GhiChu = ct.GhiChu
   }).ToList();

            // Danh sách dịch vụ
      viewModel.DanhSachDichVu = datPhong.ChiTietDatDichVus.Select(dv => new DichVuDetailItemViewModel
   {
          ChiTietDatDichVuId = dv.ChiTietDatDichVuId,
     TenDichVu = dv.DichVu?.TenDichVu,
              NgaySuDung = dv.NgaySuDung,
       SoLuong = dv.SoLuong,
     DonGia = dv.DonGia,
 ThanhTien = dv.ThanhTien,
         GhiChu = dv.GhiChu
            }).ToList();

            // Set text và color cho trạng thái
            viewModel.TrangThaiDatPhongText = GetTrangThaiDatPhongText(datPhong.TrangThaiDatPhong);
        viewModel.TrangThaiDatPhongColor = GetTrangThaiDatPhongColor(datPhong.TrangThaiDatPhong);
     viewModel.TrangThaiThanhToanText = GetTrangThaiThanhToanText(datPhong.TrangThaiThanhToan);
          viewModel.TrangThaiThanhToanColor = GetTrangThaiThanhToanColor(datPhong.TrangThaiThanhToan);

    return viewModel;
        }

        /// <summary>
        /// Load danh sách loại phòng cho form tạo đơn
      /// </summary>
        private void LoadLoaiPhongForCreate(DatPhongCreateViewModel model)
        {
            // Lấy tất cả loại phòng
  var loaiPhongs = db.LoaiPhongs.ToList();

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

    /// <summary>
        /// Tìm hoặc tạo mới khách hàng
        /// </summary>
private KhachHang FindOrCreateKhachHang(DatPhongCreateViewModel model)
        {
        // Tìm khách hàng theo SĐT
       var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.SoDienThoai == model.SoDienThoai);

  if (khachHang == null)
  {
            // Tạo mới khách hàng
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
            // Cập nhật thông tin khách hàng nếu có thay đổi
      khachHang.HoVaTen = model.HoVaTen;
   khachHang.Email = model.Email ?? khachHang.Email;
     khachHang.DiaChi = model.DiaChi ?? khachHang.DiaChi;
          khachHang.NgayCapNhat = DateTime.Now;
         db.SaveChanges();
            }

    return khachHang;
        }

        /// <summary>
        /// Generate mã đặt phòng tự động
        /// </summary>
        private string GenerateMaDatPhong()
  {
        var lastMa = db.DatPhongs
       .OrderByDescending(dp => dp.DatPhongId)
     .Select(dp => dp.MaDatPhong)
      .FirstOrDefault();

       if (string.IsNullOrEmpty(lastMa))
         return "DP0001";

         var number = int.Parse(lastMa.Substring(2)) + 1;
    return $"DP{number:D4}";
        }

        // Helper methods cho text và color
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
