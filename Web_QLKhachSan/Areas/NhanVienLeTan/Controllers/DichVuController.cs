using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu;
using Web_QLKhachSan.Filters;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    [NhanVienAuthorize]
    public class DichVuController : Controller
    {
     private readonly DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

  // GET: NhanVienLeTan/DichVu
        public ActionResult Index(
            string TimKiem,
            int? LoaiDichVuId,
  bool? DaHoatDong,
   decimal? TuGia,
decimal? DenGia,
            int CurrentPage = 1,
        int PageSize = 10)
    {
            try
    {
                // Khởi tạo ViewModel
         var viewModel = new DichVuListViewModel
             {
         CurrentPage = CurrentPage,
         PageSize = PageSize,
 Filter = new DichVuFilterViewModel
           {
   TimKiem = TimKiem,
      LoaiDichVuId = LoaiDichVuId,
              DaHoatDong = DaHoatDong,
        TuGia = TuGia,
       DenGia = DenGia
     }
    };

    // Query dịch vụ
         var query = db.DichVus.Include(d => d.LoaiDichVu).AsQueryable();

      // Áp dụng bộ lọc
    if (!string.IsNullOrWhiteSpace(TimKiem))
 {
              var keyword = TimKiem.Trim().ToLower();
         query = query.Where(d =>
         d.MaDichVu.ToLower().Contains(keyword) ||
   d.TenDichVu.ToLower().Contains(keyword) ||
     d.MoTa.ToLower().Contains(keyword)
         );
      }

        if (LoaiDichVuId.HasValue)
    {
      query = query.Where(d => d.LoaiDichVuId == LoaiDichVuId.Value);
     }

     if (DaHoatDong.HasValue)
                {
        query = query.Where(d => d.DaHoatDong == DaHoatDong.Value);
    }

      if (TuGia.HasValue)
    {
        query = query.Where(d => d.Gia >= TuGia.Value);
     }

    if (DenGia.HasValue)
     {
    query = query.Where(d => d.Gia <= DenGia.Value);
 }

                // Thống kê
   var allDichVus = query.ToList();
          viewModel.ThongKe = new DichVuThongKeViewModel
              {
       TongDichVu = allDichVus.Count,
    DangHoatDong = allDichVus.Count(d => d.DaHoatDong),
          NgungHoatDong = allDichVus.Count(d => !d.DaHoatDong),
           DangGiamGia = allDichVus.Count(d => d.GiaUuDai.HasValue && d.GiaUuDai < d.Gia),
        DoanhThuDuKien = allDichVus.Where(d => d.DaHoatDong).Sum(d => d.Gia ?? 0)
                };

         // Tổng số bản ghi
         viewModel.TotalRecords = allDichVus.Count;

        // Phân trang
  var danhSach = query
      .OrderByDescending(d => d.NgayTao)
     .Skip((CurrentPage - 1) * PageSize)
         .Take(PageSize)
 .ToList();

            // Map sang ViewModel
 viewModel.DanhSachDichVu = danhSach.Select(d => new DichVuItemViewModel
      {
        DichVuId = d.DichVuId,
      MaDichVu = d.MaDichVu,
       TenDichVu = d.TenDichVu,
 MoTa = d.MoTa,
                Gia = d.Gia,
   GiaUuDai = d.GiaUuDai,
DaHoatDong = d.DaHoatDong,
          NgayTao = d.NgayTao,
        NgayCapNhat = d.NgayCapNhat,
              TenLoaiDichVu = d.LoaiDichVu?.TenLoai,
    LoaiDichVuId = d.LoaiDichVuId,
        Icon = d.Icon
           }).ToList();

   // Danh sách loại dịch vụ cho dropdown
 viewModel.DanhSachLoaiDichVu = db.LoaiDichVus
       .Select(l => new LoaiDichVuItemViewModel
 {
      LoaiDichVuId = l.LoaiDichVuId,
 TenLoai = l.TenLoai,
        MoTa = l.MoTa,
         Icon = l.Icon,
             SoLuongDichVu = l.DichVus.Count
  })
     .ToList();

       return View(viewModel);
}
catch (Exception ex)
            {
   TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
       return View(new DichVuListViewModel());
            }
 }

        // GET: NhanVienLeTan/DichVu/Details/5
        public ActionResult Details(int? id)
        {
  if (id == null)
      {
         TempData["ErrorMessage"] = "Không tìm thấy mã dịch vụ!";
     return RedirectToAction("Index");
            }

            var dichVu = db.DichVus
             .Include(d => d.LoaiDichVu)
    .Include(d => d.ChiTietDatDichVus)
         .FirstOrDefault(d => d.DichVuId == id);

            if (dichVu == null)
            {
       TempData["ErrorMessage"] = "Không tìm thấy dịch vụ!";
  return RedirectToAction("Index");
            }

            var viewModel = new DichVuItemViewModel
       {
   DichVuId = dichVu.DichVuId,
         MaDichVu = dichVu.MaDichVu,
    TenDichVu = dichVu.TenDichVu,
      MoTa = dichVu.MoTa,
            Gia = dichVu.Gia,
            GiaUuDai = dichVu.GiaUuDai,
       DaHoatDong = dichVu.DaHoatDong,
           NgayTao = dichVu.NgayTao,
      NgayCapNhat = dichVu.NgayCapNhat,
    TenLoaiDichVu = dichVu.LoaiDichVu?.TenLoai,
   LoaiDichVuId = dichVu.LoaiDichVuId,
           Icon = dichVu.Icon
        };

    return View(viewModel);
        }

        // GET: NhanVienLeTan/DichVu/LichSuSuDung/5
        public ActionResult LichSuSuDung(
    int? id,
    DateTime? TuNgay,
    DateTime? DenNgay,
    string TimKiem,
    string SortBy,
    int CurrentPage = 1,
    int PageSize = 10)
{
    if (id == null)
    {
        TempData["ErrorMessage"] = "Không tìm thấy mã dịch vụ!";
  return RedirectToAction("Index");
    }

    var dichVu = db.DichVus.Find(id);
    if (dichVu == null)
    {
 TempData["ErrorMessage"] = "Không tìm thấy dịch vụ!";
        return RedirectToAction("Index");
    }

    // Khởi tạo ViewModel
    var viewModel = new LichSuSuDungViewModel
    {
   // Thông tin dịch vụ
     DichVuId = dichVu.DichVuId,
        MaDichVu = dichVu.MaDichVu,
        TenDichVu = dichVu.TenDichVu,
     MoTa = dichVu.MoTa,
        Gia = dichVu.Gia,
GiaUuDai = dichVu.GiaUuDai,
        Icon = dichVu.Icon,

        // Phân trang
        CurrentPage = CurrentPage,
  PageSize = PageSize,

   // Bộ lọc
        TuNgay = TuNgay,
        DenNgay = DenNgay,
        TimKiem = TimKiem,
        SortBy = SortBy
    };

    // ===== QUERY DỮ LIỆU =====
    var query = db.ChiTietDatDichVus
     .Include(c => c.DatPhong)
        .Include(c => c.DatPhong.KhachHang)
        .Where(c => c.DichVuId == id);

    // ===== ÁP DỤNG BỘ LỌC =====

 // Lọc theo khoảng thời gian
    if (TuNgay.HasValue)
    {
      query = query.Where(c =>
            (c.NgaySuDung.HasValue && c.NgaySuDung.Value >= TuNgay.Value) ||
     (!c.NgaySuDung.HasValue && c.DatPhong.NgayDat >= TuNgay.Value)
        );
    }

    if (DenNgay.HasValue)
    {
    var denNgayEnd = DenNgay.Value.AddDays(1).AddSeconds(-1); // Cuối ngày
        query = query.Where(c =>
     (c.NgaySuDung.HasValue && c.NgaySuDung.Value <= denNgayEnd) ||
      (!c.NgaySuDung.HasValue && c.DatPhong.NgayDat <= denNgayEnd)
        );
    }

    // Tìm kiếm theo tên hoặc SĐT khách hàng
    if (!string.IsNullOrWhiteSpace(TimKiem))
    {
        var keyword = TimKiem.Trim().ToLower();
        query = query.Where(c =>
   c.DatPhong.KhachHang.HoVaTen.ToLower().Contains(keyword) ||
         c.DatPhong.KhachHang.SoDienThoai.Contains(keyword) ||
            c.DatPhong.MaDatPhong.ToLower().Contains(keyword)
    );
    }

    // ===== SẮP XẾP =====
    switch (SortBy)
    {
   case "date_asc":
  query = query.OrderBy(c => c.NgaySuDung ?? c.DatPhong.NgayDat);
       break;
        case "amount_desc":
   query = query.OrderByDescending(c => c.ThanhTien);
            break;
        case "amount_asc":
          query = query.OrderBy(c => c.ThanhTien);
   break;
        case "quantity_desc":
          query = query.OrderByDescending(c => c.SoLuong);
       break;
case "date_desc":
        default:
query = query.OrderByDescending(c => c.NgaySuDung ?? c.DatPhong.NgayDat);
            break;
    }

    // ===== THỐNG KÊ TỔNG QUAN (TOÀN BỘ DỮ LIỆU - KHÔNG LỌC) =====
    var allRecords = db.ChiTietDatDichVus.Where(c => c.DichVuId == id).ToList();

    viewModel.TongSoLanSuDung = allRecords.Count;
    viewModel.TongDoanhThu = allRecords.Sum(c => c.ThanhTien ?? 0);
 viewModel.SoLuongTrungBinh = allRecords.Any() ? allRecords.Average(c => c.SoLuong) : 0;

    // ===== THỐNG KÊ THEO THÁNG (CHO BIỂU ĐỒ) =====
    viewModel.ThongKeTheoThang = allRecords
        .Where(c => c.NgaySuDung.HasValue)
        .GroupBy(c => new
        {
        Year = c.NgaySuDung.Value.Year,
     Month = c.NgaySuDung.Value.Month
     })
        .Select(g => new ThongKeThangItem
        {
       Thang = $"{g.Key.Month}/{g.Key.Year}",
            SoLan = g.Count(),
      DoanhThu = g.Sum(c => c.ThanhTien ?? 0)
 })
        .OrderByDescending(x => x.Thang)
   .Take(12) // Lấy 12 tháng gần nhất
    .OrderBy(x => x.Thang) // Sắp xếp lại theo thứ tự tăng dần
        .ToList();

    // ===== PHÂN TRANG (SAU KHI LỌC) =====
    var totalRecords = query.Count();
    viewModel.TotalRecords = totalRecords;

    var danhSach = query
        .Skip((CurrentPage - 1) * PageSize)
        .Take(PageSize)
        .ToList();

    // ===== MAP SANG VIEWMODEL =====
    viewModel.DanhSachChiTiet = danhSach.Select(c => new ChiTietSuDungItemViewModel
    {
     ChiTietDatDichVuId = c.ChiTietDatDichVuId,
        DatPhongId = c.DatPhongId,
      MaDatPhong = c.DatPhong.MaDatPhong,

 // Thông tin khách hàng
        TenKhachHang = c.DatPhong.KhachHang?.HoVaTen ?? "N/A",
        SoDienThoaiKhachHang = c.DatPhong.KhachHang?.SoDienThoai ?? "",

        // Thông tin sử dụng
   NgaySuDung = c.NgaySuDung,
        NgayDatPhong = c.DatPhong.NgayDat,
        SoLuong = c.SoLuong,
   DonGia = c.DonGia,
        ThanhTien = c.ThanhTien,
        GhiChu = c.GhiChu
    }).ToList();

    return View(viewModel);
}

     // POST: NhanVienLeTan/DichVu/ToggleStatus
     [HttpPost]
 [ValidateAntiForgeryToken]
   public JsonResult ToggleStatus(int id)
        {
        try
   {
  var dichVu = db.DichVus.Find(id);
         if (dichVu == null)
   {
     return Json(new { success = false, message = "Không tìm thấy dịch vụ!" });
            }

            dichVu.DaHoatDong = !dichVu.DaHoatDong;
      dichVu.NgayCapNhat = DateTime.Now;
  db.SaveChanges();

       var trangThai = dichVu.DaHoatDong ? "kích hoạt" : "ngừng hoạt động";
      return Json(new
   {
  success = true,
          message = $"Đã {trangThai} dịch vụ \"{dichVu.TenDichVu}\" thành công!",
         daHoatDong = dichVu.DaHoatDong,
        tenDichVu = dichVu.TenDichVu
 });
}
    catch (Exception ex)
 {
     return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
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

        // ===== HELPER METHODS =====

        /// <summary>
        /// Tự động tạo mã dịch vụ theo format: DV001, DV002, ...
   /// </summary>
        private string GenerateMaDichVu()
        {
      var lastDichVu = db.DichVus
    .OrderByDescending(d => d.DichVuId)
      .FirstOrDefault();

   if (lastDichVu == null)
        {
    return "DV001";
            }

  // Lấy số thứ tự từ mã cuối cùng
            int nextNumber = lastDichVu.DichVuId + 1;
      return $"DV{nextNumber:D3}"; // D3 = 3 chữ số (001, 002, ...)
        }

  /// <summary>
        /// Load danh sách loại dịch vụ cho dropdown
      /// </summary>
    private List<SelectListItem> LoadLoaiDichVu(int? selectedId = null)
        {
          var loaiDichVus = db.LoaiDichVus
    .OrderBy(l => l.TenLoai)
       .Select(l => new SelectListItem
        {
           Value = l.LoaiDichVuId.ToString(),
         Text = l.TenLoai,
                  Selected = selectedId.HasValue && l.LoaiDichVuId == selectedId.Value
    })
                .ToList();

    return loaiDichVus;
        }

        /// <summary>
  /// Load danh sách icon FontAwesome phổ biến
        /// </summary>
        private List<SelectListItem> LoadDanhSachIcon(string selectedIcon = null)
        {
     var icons = new List<SelectListItem>
            {
           new SelectListItem { Value = "", Text = "-- Chọn icon --" },
       new SelectListItem { Value = "fas fa-concierge-bell", Text = "🛎️ Concierge Bell" },
            new SelectListItem { Value = "fas fa-utensils", Text = "🍴 Đồ ăn" },
 new SelectListItem { Value = "fas fa-coffee", Text = "☕ Cà phê" },
      new SelectListItem { Value = "fas fa-cocktail", Text = "🍹 Cocktail" },
      new SelectListItem { Value = "fas fa-spa", Text = "💆 Spa" },
   new SelectListItem { Value = "fas fa-swimming-pool", Text = "🏊 Bể bơi" },
   new SelectListItem { Value = "fas fa-dumbbell", Text = "🏋️ Gym" },
 new SelectListItem { Value = "fas fa-wifi", Text = "📶 Wifi" },
                new SelectListItem { Value = "fas fa-car", Text = "🚗 Xe" },
                new SelectListItem { Value = "fas fa-shuttle-van", Text = "🚐 Đưa đón" },
                new SelectListItem { Value = "fas fa-broom", Text = "🧹 Dọn phòng" },
          new SelectListItem { Value = "fas fa-tshirt", Text = "👕 Giặt ủi" },
      new SelectListItem { Value = "fas fa-newspaper", Text = "📰 Báo" },
           new SelectListItem { Value = "fas fa-tv", Text = "📺 TV" },
      new SelectListItem { Value = "fas fa-phone", Text = "📞 Điện thoại" },
        new SelectListItem { Value = "fas fa-bed", Text = "🛏️ Giường" },
            new SelectListItem { Value = "fas fa-bath", Text = "🛁 Tắm" },
                new SelectListItem { Value = "fas fa-wine-glass-alt", Text = "🍷 Rượu" },
     new SelectListItem { Value = "fas fa-birthday-cake", Text = "🎂 Bánh sinh nhật" }
            };

  if (!string.IsNullOrEmpty(selectedIcon))
            {
var selectedItem = icons.FirstOrDefault(i => i.Value == selectedIcon);
         if (selectedItem != null)
           {
        selectedItem.Selected = true;
       }
     }

    return icons;
      }

        // GET: NhanVienLeTan/DichVu/Create
      public ActionResult Create()
        {
  var viewModel = new DichVuCreateViewModel
            {
      // Tự động tạo mã dịch vụ
        MaDichVu = GenerateMaDichVu(),

     // Load dropdown data
                DanhSachLoaiDichVu = LoadLoaiDichVu(),
                DanhSachIcon = LoadDanhSachIcon(),

  // Giá trị mặc định
       DaHoatDong = true
};

    return View(viewModel);
        }

      // POST: NhanVienLeTan/DichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DichVuCreateViewModel model)
        {
   try
      {
       // ===== VALIDATION =====

    // 1. Validate ModelState
         if (!ModelState.IsValid)
       {
         // Reload dropdown data nếu có lỗi
        model.DanhSachLoaiDichVu = LoadLoaiDichVu(model.LoaiDichVuId);
        model.DanhSachIcon = LoadDanhSachIcon(model.Icon);
return View(model);
     }

      // 2. Validate giá ưu đãi phải nhỏ hơn giá gốc
if (model.GiaUuDai.HasValue && model.GiaUuDai.Value >= model.Gia)
         {
     ModelState.AddModelError("GiaUuDai", "Giá ưu đãi phải nhỏ hơn giá gốc!");
      model.DanhSachLoaiDichVu = LoadLoaiDichVu(model.LoaiDichVuId);
     model.DanhSachIcon = LoadDanhSachIcon(model.Icon);
   return View(model);
    }

                // 3. Kiểm tra mã dịch vụ đã tồn tại chưa
     if (!string.IsNullOrWhiteSpace(model.MaDichVu))
       {
      var existingDichVu = db.DichVus.FirstOrDefault(d => d.MaDichVu == model.MaDichVu);
    if (existingDichVu != null)
  {
   ModelState.AddModelError("MaDichVu", "Mã dịch vụ đã tồn tại!");
        model.DanhSachLoaiDichVu = LoadLoaiDichVu(model.LoaiDichVuId);
      model.DanhSachIcon = LoadDanhSachIcon(model.Icon);
     return View(model);
            }
                }
else
       {
    // Tự động tạo mã nếu để trống
         model.MaDichVu = GenerateMaDichVu();
   }

          // ===== TẠO MỚI DỊCH VỤ =====

    var dichVu = new Models.DichVu
              {
     MaDichVu = model.MaDichVu,
 TenDichVu = model.TenDichVu,
   MoTa = model.MoTa,
    Gia = model.Gia,
   GiaUuDai = model.GiaUuDai,
            LoaiDichVuId = model.LoaiDichVuId,
        DaHoatDong = model.DaHoatDong,
     Icon = model.Icon,
   NgayTao = DateTime.Now,
  NgayCapNhat = null
     };

           db.DichVus.Add(dichVu);
            db.SaveChanges();

           // ===== THÔNG BÁO THÀNH CÔNG =====

      TempData["SuccessMessage"] = $"Đã thêm dịch vụ \"{dichVu.TenDichVu}\" thành công!";
        return RedirectToAction("Index");
       }
  catch (Exception ex)
            {
                // ===== XỬ LÝ LỖI =====

         TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;

       // Reload dropdown data
   model.DanhSachLoaiDichVu = LoadLoaiDichVu(model.LoaiDichVuId);
                model.DanhSachIcon = LoadDanhSachIcon(model.Icon);

    return View(model);
         }
        }
    }
}
