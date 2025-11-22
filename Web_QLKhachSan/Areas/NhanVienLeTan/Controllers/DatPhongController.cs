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
         .Include(dp => dp.NhanVien)
           .Include(dp => dp.KhuyenMai)
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
       LoadKhuyenMaiForCreate(viewModel);
       
   // ✅ Kiểm tra đã load thành công chưa
            if (viewModel.DanhSachPhongDat == null || viewModel.DanhSachPhongDat.Count == 0)
     {
    TempData["ErrorMessage"] = "Không thể tải danh sách loại phòng!";
      return RedirectToAction("Index");
       }
        
        return View(viewModel);
    }
         catch (Exception ex)
       {
     System.Diagnostics.Debug.WriteLine($"[ERROR - DatPhong/Create GET] {ex.Message}");
           System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
  TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
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
         LoadKhuyenMaiForCreate(model);
  return View(model);
                }

       if (!model.IsValidDateRange())
{
       ModelState.AddModelError("NgayTra", "Ngày trả phải sau ngày nhận!");
                    LoadLoaiPhongForCreate(model);
 LoadKhuyenMaiForCreate(model);
              return View(model);
              }

    if (!model.HasRooms())
          {
        ModelState.AddModelError("", "Vui lòng chọn ít nhất một phòng!");
        LoadLoaiPhongForCreate(model);
      LoadKhuyenMaiForCreate(model);
        return View(model);
   }

    var khachHang = FindOrCreateKhachHang(model);

    // ✅ FIX: Tính tổng tiền từ ChiTietDatPhong thực tế
     decimal tongTienPhong = 0;
    decimal tongGiamGiaPhong = 0;

    // Tính tổng từ các phòng đã chọn
            foreach (var phongItem in model.DanhSachPhongDat.Where(p => p.SoLuong > 0))
       {
 decimal tienPhong = phongItem.DonGia * phongItem.SoLuong * model.SoDem;
   tongTienPhong += tienPhong;
         tongGiamGiaPhong += tienPhong * (phongItem.GiamGia / 100);
   }

    decimal tongTien = tongTienPhong - tongGiamGiaPhong;
    decimal giamGiaKM = 0;

         if (model.MaKhuyenMaiId.HasValue)
  {
         var khuyenMai = db.KhuyenMais.Find(model.MaKhuyenMaiId.Value);
     if (khuyenMai != null && khuyenMai.DaHoatDong)
         {
 var today = DateTime.Now.Date;
         // Kiểm tra còn hiệu lực
      if (khuyenMai.NgayBatDau <= today && khuyenMai.NgayKetThuc >= today)
 {
   giamGiaKM = tongTien * (khuyenMai.GiaTri.GetValueOrDefault() / 100);
tongTien = tongTien - giamGiaKM;
  }
     }
   }

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
      TrangThaiThanhToan = 0,
           TongTien = tongTien, // ✅ Tổng tiền đã tính đúng
      HinhThucThanhToan = model.HinhThucThanhToan,
        MaKhuyenMai = model.MaKhuyenMaiId,
         GhiChu = BuildGhiChu(model),
  NgayTao = DateTime.Now,
            
    // ✅ SET NULL cho các cột online payment (đặt trực tiếp tại quầy)
            PaymentRefId = null,
            PaymentMethod = null,
  OnlinePaymentStatus = null,
            TotalAmount = null,
        CustomerName = null
 };

  db.DatPhongs.Add(datPhong);
  db.SaveChanges();

   foreach (var phongItem in model.DanhSachPhongDat.Where(p => p.SoLuong > 0))
       {
     // ✅ Tính toán lại chính xác từ dữ liệu thực
  decimal donGia = phongItem.DonGia;
           int soLuong = phongItem.SoLuong;
 int soDem = model.SoDem;
decimal phanTramGiam = phongItem.GiamGia;

   decimal tongTienLoaiPhong = donGia * soLuong * soDem;
    decimal tienGiam = tongTienLoaiPhong * (phanTramGiam / 100);
    decimal thanhTien = tongTienLoaiPhong - tienGiam;

     var chiTiet = new ChiTietDatPhong
     {
       DatPhongId = datPhong.DatPhongId,
           LoaiPhongId = phongItem.LoaiPhongId,
       PhongId = null,
       DonGia = donGia,
      SoLuong = soLuong,
          NgayDen = model.NgayNhan,
  NgayDi = model.NgayTra,
        GiamGia = tienGiam, // ✅ Số tiền giảm (VNĐ)
     ThanhTien = thanhTien, // ✅ Tổng tiền đã trừ giảm giá
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
         LoadKhuyenMaiForCreate(model);
   return View(model);
  }
 }

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

        /// <summary>
        /// Lấy thông tin mã khuyến mãi (AJAX)
        /// </summary>
        [HttpPost]
        public JsonResult GetKhuyenMai(int? khuyenMaiId)
        {
            try
         {
   if (!CheckRole())
       {
 return Json(new { success = false, message = "Bạn không có quyền!" });
    }

     if (!khuyenMaiId.HasValue)
         {
 return Json(new { success = true, data = (object)null });
             }

          var khuyenMai = db.KhuyenMais
      .Where(km => km.KhuyenMaiId == khuyenMaiId.Value && km.DaHoatDong)
           .Select(km => new
   {
   km.KhuyenMaiId,
        km.MaKhuyenMai,
              km.TenKhuyenMai,
       km.GiaTri,
            km.NgayBatDau,
    km.NgayKetThuc,
           km.DieuKienApDung
       })
      .FirstOrDefault();

             if (khuyenMai != null)
        {
        var today = DateTime.Now.Date;

           // Kiểm tra còn hiệu lực không
       if (khuyenMai.NgayBatDau > today)
 {
    return Json(new
          {
   success = false,
        message = $"Mã khuyến mãi chưa có hiệu lực! Bắt đầu từ {khuyenMai.NgayBatDau:dd/MM/yyyy}"
       });
        }

         if (khuyenMai.NgayKetThuc < today)
       {
              return Json(new
          {
       success = false,
     message = $"Mã khuyến mãi đã hết hạn! Hết hạn ngày {khuyenMai.NgayKetThuc:dd/MM/yyyy}"
   });
     }

         return Json(new { success = true, data = khuyenMai });
     }
   else
                {
             return Json(new { success = false, message = "Không tìm thấy mã khuyến mãi!" });
       }
         }
            catch (Exception ex)
 {
           System.Diagnostics.Debug.WriteLine($"[ERROR - GetKhuyenMai] {ex.Message}");
      return Json(new { success = false, message = "Có lỗi xảy ra!" });
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

        private void LoadKhuyenMaiForCreate(DatPhongCreateViewModel model)
        {
   var today = DateTime.Now.Date;

        // ✅ FIX: Lấy dữ liệu từ DB trước, sau đó format ở memory
      var khuyenMais = db.KhuyenMais
           .Where(km => km.DaHoatDong && 
    km.NgayBatDau <= today && 
      km.NgayKetThuc >= today)
  .OrderBy(km => km.TenKhuyenMai)
         .ToList(); // ✅ QUAN TRỌNG: Execute query trước

         // ✅ Format text ở memory (không phải trong LINQ to Entities)
            ViewBag.DanhSachKhuyenMai = khuyenMais
   .Select(km => new SelectListItem
        {
      Value = km.KhuyenMaiId.ToString(),
 Text = $"{km.TenKhuyenMai} - Giảm {km.GiaTri}%"
             })
.ToList();
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
                db.Entry(khachHang).State = EntityState.Modified;
db.SaveChanges();
            }

            return khachHang;
        }

    private string GenerateMaDatPhong()
     {
var lastDatPhong = db.DatPhongs.OrderByDescending(dp => dp.DatPhongId).FirstOrDefault();
            int newId = 1;

   if (lastDatPhong != null && !string.IsNullOrEmpty(lastDatPhong.MaDatPhong))
            {
            int lastId;
     if (int.TryParse(lastDatPhong.MaDatPhong.Replace("DP", ""), out lastId))
                {
 newId = lastId + 1;
     }
            }

     return "DP" + newId.ToString("D4");
        }

        private string BuildGhiChu(DatPhongCreateViewModel model)
      {
       string ghiChu = "";
     if (!string.IsNullOrWhiteSpace(model.GhiChu))
     {
       ghiChu = model.GhiChu.Trim();
   }

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
                NgayNhan = datPhong.NgayNhan,
       NgayTra = datPhong.NgayTra,
       SoDem = datPhong.SoDem,
     SoLuongKhach = datPhong.SoLuongKhach,
                
 // Khách hàng
  KhachHang = datPhong.KhachHang != null ? new KhachHangItemViewModel
    {
       MaKhachHang = datPhong.KhachHang.MaKhachHang,
     HoVaTen = datPhong.KhachHang.HoVaTen,
    SoDienThoai = datPhong.KhachHang.SoDienThoai,
    Email = datPhong.KhachHang.Email,
              DiaChi = datPhong.KhachHang.DiaChi,
         GioiTinh = datPhong.KhachHang.GioiTinh
     } : null,

     // Nhân viên
        NhanVien = datPhong.NhanVien != null ? new NhanVienItemViewModel
       {
     NhanVienId = datPhong.NhanVien.NhanVienId,
        HoVaTen = datPhong.NhanVien.HoVaTen,
     SoDienThoai = datPhong.NhanVien.SoDienThoai,
 Email = datPhong.NhanVien.Email
 } : null,

                // Khuyến mãi
         KhuyenMai = datPhong.KhuyenMai != null ? new KhuyenMaiItemViewModel
      {
            KhuyenMaiId = datPhong.KhuyenMai.KhuyenMaiId,
         MaKhuyenMai = datPhong.KhuyenMai.MaKhuyenMai,
         TenKhuyenMai = datPhong.KhuyenMai.TenKhuyenMai,
         GiaTri = datPhong.KhuyenMai.GiaTri,
        NgayBatDau = datPhong.KhuyenMai.NgayBatDau,
      NgayKetThuc = datPhong.KhuyenMai.NgayKetThuc
        } : null,

    // Thông tin phòng
           SoLuongPhong = chiTietPhongs.Sum(ct => ct.SoLuong),
              DanhSachLoaiPhong = string.Join(", ", chiTietPhongs
      .GroupBy(ct => ct.LoaiPhong?.TenLoai)
   .Select(g => $"{g.Key} x{g.Sum(ct => ct.SoLuong)}")),
 DanhSachPhong = chiTietPhongs.Any(ct => ct.PhongId.HasValue)
        ? string.Join(", ", chiTietPhongs.Where(ct => ct.Phong != null).Select(ct => ct.Phong.MaPhong))
            : "Chưa gán",

                // Trạng thái
    TrangThaiDatPhong = datPhong.TrangThaiDatPhong,
                TrangThaiDatPhongText = GetTrangThaiDatPhongText(datPhong.TrangThaiDatPhong),
    TrangThaiDatPhongColor = GetTrangThaiDatPhongColor(datPhong.TrangThaiDatPhong),
           TrangThaiThanhToan = datPhong.TrangThaiThanhToan,
    TrangThaiThanhToanText = GetTrangThaiThanhToanText(datPhong.TrangThaiThanhToan),
        TrangThaiThanhToanColor = GetTrangThaiThanhToanColor(datPhong.TrangThaiThanhToan),

     // Tiền
                TongTien = datPhong.TongTien,
    HinhThucThanhToan = datPhong.HinhThucThanhToan,

                // Online payment
                PaymentRefId = datPhong.PaymentRefId,
     PaymentMethod = datPhong.PaymentMethod,
         OnlinePaymentStatus = datPhong.OnlinePaymentStatus,
         TotalAmount = datPhong.TotalAmount,

    // Khác
    GhiChu = datPhong.GhiChu,
    NgayTao = datPhong.NgayTao,
             NgayCapNhat = datPhong.NgayCapNhat
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
    case 0: return "#ffc107"; // warning - vàng
    case 1: return "#17a2b8"; // info - xanh dương
             case 2: return "#28a745"; // success - xanh lá
     case 3: return "#6c757d"; // secondary - xám
                case 4: return "#dc3545"; // danger - đỏ
    default: return "#343a40"; // dark
          }
        }

        private string GetTrangThaiThanhToanText(byte trangThai)
        {
    switch (trangThai)
            {
     case 0: return "Chưa thanh toán";
case 2: return "Đã thanh toán đầy đủ";
 default: return "Không xác định";
         }
      }

        private string GetTrangThaiThanhToanColor(byte trangThai)
        {
    switch (trangThai)
 {
      case 0: return "#dc3545"; // danger - đỏ
      case 2: return "#28a745"; // success - xanh lá
default: return "#6c757d"; // secondary - xám
}
        }

        /// <summary>
/// Xác nhận đơn đặt phòng và gán phòng tự động
        /// </summary>
        [HttpPost]
  public JsonResult XacNhan(int id)
        {
        try
          {
   if (!CheckRole())
     {
    return Json(new { success = false, message = "Bạn không có quyền!" });
                }

       var datPhong = db.DatPhongs
       .Include(dp => dp.ChiTietDatPhongs.Select(ct => ct.LoaiPhong))
  .FirstOrDefault(dp => dp.DatPhongId == id);

        if (datPhong == null)
{
     return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });
        }

        // Kiểm tra trạng thái
    if (datPhong.TrangThaiDatPhong != 0)
   {
        return Json(new { success = false, message = "Chỉ xác nhận được đơn đang chờ xác nhận!" });
         }

            // ✅ BỎ kiểm tra thanh toán - Vì thanh toán khi check-out

    // Danh sách phòng đã gán
        var danhSachPhongGan = new List<string>();
     var coLoiGanPhong = false;
                var loiMessage = "";

      // Gán phòng cho từng chi tiết đặt phòng
          foreach (var chiTiet in datPhong.ChiTietDatPhongs)
    {
        if (chiTiet.PhongId != null) continue; // Đã gán rồi thì bỏ qua

     // Tìm phòng trống theo loại phòng
    var phongTrong = db.Phongs
      .Where(p =>
         p.LoaiPhongId == chiTiet.LoaiPhongId &&
p.TrangThaiPhong == 0 && // Trống
       p.DaHoatDong &&
   // Kiểm tra không bị đặt trùng trong khoảng thời gian
      !p.ChiTietDatPhongs.Any(ct =>
     ct.DatPhong.TrangThaiDatPhong != 4 && // Không bị hủy
  ct.DatPhong.DatPhongId != datPhong.DatPhongId && // Không phải đơn hiện tại
  (
     (ct.NgayDen <= datPhong.NgayNhan && ct.NgayDi > datPhong.NgayNhan) ||
     (ct.NgayDen < datPhong.NgayTra && ct.NgayDi >= datPhong.NgayTra) ||
     (ct.NgayDen >= datPhong.NgayNhan && ct.NgayDi <= datPhong.NgayTra)
      )
           )
         )
      .Take(chiTiet.SoLuong)
    .ToList();

         if (phongTrong.Count < chiTiet.SoLuong)
        {
      coLoiGanPhong = true;
    loiMessage = $"Không đủ phòng {chiTiet.LoaiPhong?.TenLoai ?? ""}! Cần {chiTiet.SoLuong} phòng nhưng chỉ còn {phongTrong.Count} phòng trống.";
        break;
        }

         // Nếu SoLuong > 1, cần tạo nhiều ChiTietDatPhong
     if (chiTiet.SoLuong > 1)
     {
       // Xóa chi tiết gộp cũ
 db.ChiTietDatPhongs.Remove(chiTiet);

        // Tạo chi tiết riêng cho từng phòng
       foreach (var phong in phongTrong)
             {
 var chiTietMoi = new ChiTietDatPhong
      {
     DatPhongId = datPhong.DatPhongId,
      LoaiPhongId = chiTiet.LoaiPhongId,
 PhongId = phong.PhongId,
    DonGia = chiTiet.DonGia,
     SoLuong = 1 // Mỗi chi tiết 1 phòng
    };
         db.ChiTietDatPhongs.Add(chiTietMoi);

           // Cập nhật trạng thái phòng
   phong.TrangThaiPhong = 1; // Đã đặt
 phong.NgayCapNhat = DateTime.Now;

   danhSachPhongGan.Add(phong.MaPhong);
    }
     }
      else
    {
 // SoLuong = 1, gán phòng trực tiếp
    var phong = phongTrong.First();
   chiTiet.PhongId = phong.PhongId;
  chiTiet.TrangThaiPhong = 1;
        chiTiet.NgayCapNhat = DateTime.Now;

  // Cập nhật trạng thái phòng
         phong.TrangThaiPhong = 1;
   phong.NgayCapNhat = DateTime.Now;

      danhSachPhongGan.Add(phong.MaPhong);
       }
    }

      if (coLoiGanPhong)
    {
          return Json(new { success = false, message = loiMessage });
     }

    // Cập nhật trạng thái đơn đặt phòng
   datPhong.TrangThaiDatPhong = 1; // Đã xác nhận
 datPhong.NgayCapNhat = DateTime.Now;
            // ✅ TrangThaiThanhToan giữ nguyên = 0 (chưa thanh toán)
            // Sẽ thanh toán khi check-out

       db.SaveChanges();

         return Json(new
    {
   success = true,
     message = $"Xác nhận đơn thành công! Đã gán phòng: {string.Join(", ", danhSachPhongGan)}"
});
          }
    catch (Exception ex)
   {
    System.Diagnostics.Debug.WriteLine($"[ERROR - XacNhan] {ex.Message}");
    System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
        return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
      }
        }

   /// <summary>
        /// Hủy đơn đặt phòng
        /// </summary>
      [HttpPost]
        public JsonResult Huy(int id, string lyDo)
  {
  try
            {
      if (!CheckRole())
       {
    return Json(new { success = false, message = "Bạn không có quyền!" });
                }

    var datPhong = db.DatPhongs
 .Include(dp => dp.ChiTietDatPhongs)
        .FirstOrDefault(dp => dp.DatPhongId == id);

      if (datPhong == null)
      {
return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });
     }

   // ✅ Chỉ hủy được đơn chưa check-in
    if (datPhong.TrangThaiDatPhong >= 2)
                {
      return Json(new { success = false, message = "Không thể hủy đơn đã check-in!" });
    }

                // Giải phóng phòng đã gán
       foreach (var chiTiet in datPhong.ChiTietDatPhongs)
 {
         if (chiTiet.PhongId.HasValue)
        {
      var phong = db.Phongs.Find(chiTiet.PhongId.Value);
    if (phong != null)
          {
       phong.TrangThaiPhong = 0; // Trống
 phong.NgayCapNhat = DateTime.Now;
   }
 }
          }

    // Cập nhật trạng thái đơn
      datPhong.TrangThaiDatPhong = 4; // Đã hủy
  datPhong.GhiChu = (datPhong.GhiChu ?? "") + $"\n[HỦY] {DateTime.Now:dd/MM/yyyy HH:mm} - Lý do: {lyDo}";
   datPhong.NgayCapNhat = DateTime.Now;

       db.SaveChanges();

         return Json(new { success = true, message = "Hủy đơn thành công!" });
          }
            catch (Exception ex)
  {
System.Diagnostics.Debug.WriteLine($"[ERROR - Huy] {ex.Message}");
       return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
   }
    }

        /// <summary>
        /// Check-in cho khách
        /// </summary>
      [HttpPost]
 public JsonResult CheckIn(int id)
     {
      try
     {
     if (!CheckRole())
   {
       return Json(new { success = false, message = "Bạn không có quyền!" });
     }

           var datPhong = db.DatPhongs
       .Include(dp => dp.ChiTietDatPhongs)
   .FirstOrDefault(dp => dp.DatPhongId == id);

          if (datPhong == null)
      {
     return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng!" });
       }

    // Kiểm tra trạng thái
      if (datPhong.TrangThaiDatPhong != 1)
    {
     return Json(new { success = false, message = "Chỉ check-in được đơn đã xác nhận!" });
     }

        // Kiểm tra đã gán phòng chưa
        if (!datPhong.ChiTietDatPhongs.Any(ct => ct.PhongId.HasValue))
    {
     return Json(new { success = false, message = "Chưa gán phòng cho đơn này!" });
        }

    // ✅ CẬP NHẬT NGÀY CHECK-IN THỰC TẾ
    datPhong.NgayNhan = DateTime.Now;

    // Cập nhật trạng thái phòng sang "Đang ở"
   foreach (var chiTiet in datPhong.ChiTietDatPhongs.Where(ct => ct.PhongId.HasValue))
        {
     var phong = db.Phongs.Find(chiTiet.PhongId.Value);
 if (phong != null)
   {
             phong.TrangThaiPhong = 2; // Đang ở
   phong.NgayCapNhat = DateTime.Now;
      }

    chiTiet.TrangThaiPhong = 2;
        // ✅ CẬP NHẬT NGÀY ĐẾN THỰC TẾ CHO CHI TIẾT
        chiTiet.NgayDen = DateTime.Now;
        chiTiet.NgayCapNhat = DateTime.Now;
     }

           // Cập nhật trạng thái đơn
     datPhong.TrangThaiDatPhong = 2; // Đã check-in
       datPhong.NgayCapNhat = DateTime.Now;

    db.SaveChanges();

     var danhSachPhong = string.Join(", ", datPhong.ChiTietDatPhongs
      .Where(ct => ct.Phong != null)
              .Select(ct => ct.Phong.MaPhong));

     return Json(new { 
       success = true, 
      message = $"Check-in thành công lúc {DateTime.Now:HH:mm dd/MM/yyyy}! Phòng: {danhSachPhong}" 
    });
  }
   catch (Exception ex)
       {
       System.Diagnostics.Debug.WriteLine($"[ERROR - CheckIn] {ex.Message}");
          return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
   }
        }

        /// <summary>
        /// Check-out - Tạo hóa đơn (chưa thanh toán)
    /// </summary>
        [HttpPost]
     public JsonResult CheckOut(int id)
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
 .FirstOrDefault(dp => dp.DatPhongId == id);

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

          // Kiểm tra đã có hóa đơn chưa
    var hoaDonCu = db.HoaDons.FirstOrDefault(hd => hd.DatPhongId == datPhong.DatPhongId);
    if (hoaDonCu != null)
      {
    return Json(new { 
     success = false, 
message = "Đơn này đã có hóa đơn rồi!",
hoaDonId = hoaDonCu.HoaDonId
     });
        }

    // Tạo hóa đơn MỚI
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

 // ✅ CẬP NHẬT NGÀY CHECK-OUT THỰC TẾ
       datPhong.NgayTra = DateTime.Now;
    
  // Cập nhật trạng thái đơn đặt phòng
           datPhong.TrangThaiDatPhong = 3; // Đã check-out
  datPhong.NgayCapNhat = DateTime.Now;
     // ⚠️ KHÔNG cập nhật TrangThaiThanhToan - sẽ cập nhật ở bước thanh toán

// Cập nhật trạng thái phòng
  foreach (var chiTiet in datPhong.ChiTietDatPhongs.Where(ct => ct.PhongId.HasValue))
    {
     var phong = db.Phongs.Find(chiTiet.PhongId.Value);
 if (phong != null)
          {
         phong.TrangThaiPhong = 3; // Đang dọn
 phong.NgayCapNhat = DateTime.Now;
   }

     chiTiet.TrangThaiPhong = 3;
      // ✅ CẬP NHẬT NGÀY ĐI THỰC TẾ CHO CHI TIẾT
              chiTiet.NgayDi = DateTime.Now;
  chiTiet.NgayCapNhat = DateTime.Now;
       }

     db.SaveChanges();

return Json(new { 
     success = true, 
 message = $"Check-out thành công lúc {DateTime.Now:HH:mm dd/MM/yyyy}! Hóa đơn: {hoaDon.MaHoaDon}. Vui lòng xác nhận thanh toán.",
          hoaDonId = hoaDon.HoaDonId,
   maHoaDon = hoaDon.MaHoaDon,
  tongTien = tongTien
     });
   }
    catch (Exception ex)
       {
     System.Diagnostics.Debug.WriteLine($"[ERROR - CheckOut] {ex.Message}");
     return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
       }
   }

     /// <summary>
        /// Thanh toán hóa đơn
        /// </summary>
        [HttpPost]
        public JsonResult ThanhToanHoaDon(int hoaDonId, byte phuongThuc, string maGiaoDich)
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
  System.Diagnostics.Debug.WriteLine($"[ERROR - ThanhToanHoaDon] {ex.Message}");
          return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
        }
        }

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
