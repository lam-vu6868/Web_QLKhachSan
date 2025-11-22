using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý khuyến mãi cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class KhuyenMaiController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

 private bool CheckRole()
        {
     string vaiTro = Session["VaiTro"]?.ToString();
          return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
      }

        // GET: Index - Danh sách khuyến mãi
        public ActionResult Index(KhuyenMaiFilterViewModel filter = null)
        {
       try
  {
         if (!CheckRole())
  {
       TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
  return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
  }

    // ✅ FIX: Đảm bảo filter luôn có giá trị
    if (filter == null)
    {
        filter = new KhuyenMaiFilterViewModel();
    }

  var viewModel = new KhuyenMaiListViewModel();
    viewModel.Filter = filter;

  // ✅ DEBUG: Kiểm tra tổng số khuyến mãi trong database
    var totalInDb = db.KhuyenMais.Count();
    System.Diagnostics.Debug.WriteLine($"[DEBUG] Tổng số khuyến mãi trong DB: {totalInDb}");

    var query = db.KhuyenMais.AsQueryable();

  // Filter: Tìm kiếm
 if (!string.IsNullOrWhiteSpace(filter.TimKiem))
    {
   string searchTerm = filter.TimKiem.Trim().ToLower();
 query = query.Where(km =>
     km.MaKhuyenMai.ToLower().Contains(searchTerm) ||
        km.TenKhuyenMai.ToLower().Contains(searchTerm));
         System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter TimKiem: {searchTerm}");
}

       // Filter: Trạng thái hoạt động
  if (filter.DaHoatDong.HasValue)
    {
 query = query.Where(km => km.DaHoatDong == filter.DaHoatDong.Value);
     System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter DaHoatDong: {filter.DaHoatDong.Value}");
     }

   // Filter: Theo thời gian
 if (filter.TuNgay.HasValue)
        {
        query = query.Where(km => km.NgayKetThuc >= filter.TuNgay.Value);
   System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter TuNgay: {filter.TuNgay.Value}");
        }

     if (filter.DenNgay.HasValue)
  {
  query = query.Where(km => km.NgayBatDau <= filter.DenNgay.Value);
  System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter DenNgay: {filter.DenNgay.Value}");
 }

     // Filter: Hiệu lực
   var today = DateTime.Now.Date;
    if (!string.IsNullOrEmpty(filter.HieuLuc))
     {
      switch (filter.HieuLuc)
        {
      case "active": // Đang áp dụng
   query = query.Where(km =>
 km.NgayBatDau <= today &&
  km.NgayKetThuc >= today &&
km.DaHoatDong);
     break;
         case "upcoming": // Chưa bắt đầu
        query = query.Where(km => km.NgayBatDau > today);
       break;
 case "expired": // Đã hết hạn
      query = query.Where(km => km.NgayKetThuc < today);
      break;
}
         System.Diagnostics.Debug.WriteLine($"[DEBUG] Filter HieuLuc: {filter.HieuLuc}");
  }

    // ✅ DEBUG: Kiểm tra số lượng sau khi lọc
var countAfterFilter = query.Count();
    System.Diagnostics.Debug.WriteLine($"[DEBUG] Số lượng sau khi lọc: {countAfterFilter}");

    // Thống kê
  var sapHetHanDate = today.AddDays(7); // ✅ Tính toán trước, bên ngoài query

    viewModel.ThongKe = new KhuyenMaiThongKeViewModel
      {
  TongKhuyenMai = db.KhuyenMais.Count(),
      DangHoatDong = db.KhuyenMais.Count(km => km.DaHoatDong),
      DangApDung = db.KhuyenMais.Count(km =>
    km.DaHoatDong &&
   km.NgayBatDau <= today &&
   km.NgayKetThuc >= today),
SapHetHan = db.KhuyenMais.Count(km =>
 km.DaHoatDong &&
    km.NgayBatDau <= today &&
 km.NgayKetThuc >= today &&
  km.NgayKetThuc <= sapHetHanDate), // ✅ Sử dụng biến đã tính sẵn
DaHetHan = db.KhuyenMais.Count(km => km.NgayKetThuc < today),
    ChuaKichHoat = db.KhuyenMais.Count(km => !km.DaHoatDong),
  DaDung = db.DatPhongs.Count(dp => dp.MaKhuyenMai.HasValue)
};

    System.Diagnostics.Debug.WriteLine($"[DEBUG] Thống kê - Tổng: {viewModel.ThongKe.TongKhuyenMai}, Hoạt động: {viewModel.ThongKe.DangHoatDong}");

     // Pagination
int totalRecords = query.Count();
 int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
 int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;
      int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        viewModel.TotalRecords = totalRecords;
   viewModel.PageSize = pageSize;
  viewModel.CurrentPage = currentPage;
      viewModel.TotalPages = totalPages;

  // Lấy dữ liệu
      var khuyenMais = query
  .OrderByDescending(km => km.NgayBatDau)
     .Skip((currentPage - 1) * pageSize)
  .Take(pageSize)
      .ToList();

    System.Diagnostics.Debug.WriteLine($"[DEBUG] Số bản ghi hiển thị trang {currentPage}: {khuyenMais.Count}");

    // ✅ Map dữ liệu sang ViewModel
    viewModel.DanhSachKhuyenMai = khuyenMais.Select(km => MapToKhuyenMaiItemViewModel(km)).ToList();

 System.Diagnostics.Debug.WriteLine($"[DEBUG] DanhSachKhuyenMai.Count: {viewModel.DanhSachKhuyenMai.Count}");

      return View(viewModel);
          }
     catch (Exception ex)
{
   System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Index] {ex.Message}");
  System.Diagnostics.Debug.WriteLine($"[ERROR - StackTrace] {ex.StackTrace}");
TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách khuyến mãi: " + ex.Message;
            return View(new KhuyenMaiListViewModel());
      }
   }

        // GET: Create - Form tạo khuyến mãi
        public ActionResult Create()
        {
 try
            {
   if (!CheckRole())
    {
   TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
 return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
      }

     var viewModel = new KhuyenMaiFormViewModel();
           return View(viewModel);
      }
            catch (Exception ex)
       {
     System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Create GET] {ex.Message}");
          TempData["ErrorMessage"] = "Có lỗi xảy ra!";
     return RedirectToAction("Index");
            }
        }

    // POST: Create - Xử lý tạo khuyến mãi
        [HttpPost]
    [ValidateAntiForgeryToken]
        public ActionResult Create(KhuyenMaiFormViewModel model)
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
     return View(model);
    }

            // Validation
        if (!model.IsNgayHopLe())
       {
          ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu!");
  return View(model);
           }

       // Tạo mã tự động nếu chưa có
      if (string.IsNullOrWhiteSpace(model.MaKhuyenMai))
            {
           model.MaKhuyenMai = GenerateMaKhuyenMai();
        }

             // Kiểm tra trùng mã
           if (db.KhuyenMais.Any(km => km.MaKhuyenMai == model.MaKhuyenMai))
             {
ModelState.AddModelError("MaKhuyenMai", "Mã khuyến mãi đã tồn tại!");
         return View(model);
         }

           var khuyenMai = new KhuyenMai
      {
  MaKhuyenMai = model.MaKhuyenMai.Trim(),
        TenKhuyenMai = model.TenKhuyenMai.Trim(),
 GiaTri = model.GiaTri,
      NgayBatDau = model.NgayBatDau,
     NgayKetThuc = model.NgayKetThuc,
             DieuKienApDung = model.DieuKienApDung?.Trim(),
        SoLanSuDungToiDa = model.SoLanSuDungToiDa,
             DaHoatDong = model.DaHoatDong
     };

     db.KhuyenMais.Add(khuyenMai);
       db.SaveChanges();

     TempData["SuccessMessage"] = $"Tạo khuyến mãi {khuyenMai.MaKhuyenMai} thành công!";
  return RedirectToAction("Index");
    }
        catch (Exception ex)
         {
      System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Create POST] {ex.Message}");
         TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo khuyến mãi!";
       return View(model);
      }
        }

   // GET: Edit - Form sửa khuyến mãi
        public ActionResult Edit(int id)
        {
  try
            {
        if (!CheckRole())
        {
        TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
        return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
       }

       var khuyenMai = db.KhuyenMais.Find(id);
       if (khuyenMai == null)
             {
         TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi!";
   return RedirectToAction("Index");
      }

     var viewModel = new KhuyenMaiFormViewModel
      {
       KhuyenMaiId = khuyenMai.KhuyenMaiId,
   MaKhuyenMai = khuyenMai.MaKhuyenMai,
          TenKhuyenMai = khuyenMai.TenKhuyenMai,
     GiaTri = khuyenMai.GiaTri ?? 0,
     NgayBatDau = khuyenMai.NgayBatDau ?? DateTime.Now,
     NgayKetThuc = khuyenMai.NgayKetThuc ?? DateTime.Now.AddDays(30),
              DieuKienApDung = khuyenMai.DieuKienApDung,
  SoLanSuDungToiDa = khuyenMai.SoLanSuDungToiDa,
       DaHoatDong = khuyenMai.DaHoatDong
             };

   return View(viewModel);
   }
            catch (Exception ex)
   {
   System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Edit GET] {ex.Message}");
         TempData["ErrorMessage"] = "Có lỗi xảy ra!";
     return RedirectToAction("Index");
            }
        }

 // POST: Edit - Xử lý sửa khuyến mãi
        [HttpPost]
  [ValidateAntiForgeryToken]
        public ActionResult Edit(KhuyenMaiFormViewModel model)
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
            return View(model);
        }

                if (!model.IsNgayHopLe())
                {
  ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau ngày bắt đầu!");
            return View(model);
           }

                var khuyenMai = db.KhuyenMais.Find(model.KhuyenMaiId);
            if (khuyenMai == null)
         {
  TempData["ErrorMessage"] = "Không tìm thấy khuyến mãi!";
             return RedirectToAction("Index");
       }

    // Update
        khuyenMai.TenKhuyenMai = model.TenKhuyenMai.Trim();
                khuyenMai.GiaTri = model.GiaTri;
        khuyenMai.NgayBatDau = model.NgayBatDau;
    khuyenMai.NgayKetThuc = model.NgayKetThuc;
     khuyenMai.DieuKienApDung = model.DieuKienApDung?.Trim();
      khuyenMai.SoLanSuDungToiDa = model.SoLanSuDungToiDa;
            khuyenMai.DaHoatDong = model.DaHoatDong;

    db.Entry(khuyenMai).State = EntityState.Modified;
                db.SaveChanges();

           TempData["SuccessMessage"] = $"Cập nhật khuyến mãi {khuyenMai.MaKhuyenMai} thành công!";
        return RedirectToAction("Index");
      }
catch (Exception ex)
         {
      System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Edit POST] {ex.Message}");
          TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật khuyến mãi!";
    return View(model);
      }
        }

        // POST: Delete - Xóa khuyến mãi
        [HttpPost]
        public JsonResult Delete(int id)
   {
            try
     {
         if (!CheckRole())
            {
         return Json(new { success = false, message = "Bạn không có quyền!" });
     }

              var khuyenMai = db.KhuyenMais.Find(id);
      if (khuyenMai == null)
             {
      return Json(new { success = false, message = "Không tìm thấy khuyến mãi!" });
        }

      // Kiểm tra đã có đơn đặt phòng sử dụng chưa
    if (db.DatPhongs.Any(dp => dp.MaKhuyenMai == khuyenMai.KhuyenMaiId))
  {
     return Json(new { success = false, message = "Không thể xóa khuyến mãi đã được sử dụng!" });
     }

 db.KhuyenMais.Remove(khuyenMai);
      db.SaveChanges();

return Json(new { success = true, message = "Xóa khuyến mãi thành công!" });
   }
    catch (Exception ex)
    {
            System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/Delete] {ex.Message}");
           return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
       }
     }

        // POST: ToggleStatus - Bật/tắt trạng thái
      [HttpPost]
      public JsonResult ToggleStatus(int id)
        {
 try
  {
if (!CheckRole())
                {
      return Json(new { success = false, message = "Bạn không có quyền!" });
     }

  var khuyenMai = db.KhuyenMais.Find(id);
  if (khuyenMai == null)
                {
       return Json(new { success = false, message = "Không tìm thấy khuyến mãi!" });
        }

                khuyenMai.DaHoatDong = !khuyenMai.DaHoatDong;
         db.Entry(khuyenMai).State = EntityState.Modified;
     db.SaveChanges();

       string status = khuyenMai.DaHoatDong ? "kích hoạt" : "vô hiệu hóa";
     return Json(new { success = true, message = $"Đã {status} khuyến mãi thành công!", daHoatDong = khuyenMai.DaHoatDong });
            }
  catch (Exception ex)
       {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhuyenMai/ToggleStatus] {ex.Message}");
         return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
     }
        }

  // Helper Methods
        private string GenerateMaKhuyenMai()
        {
       var lastKhuyenMai = db.KhuyenMais.OrderByDescending(km => km.KhuyenMaiId).FirstOrDefault();
            int newId = 1;

        if (lastKhuyenMai != null && !string.IsNullOrEmpty(lastKhuyenMai.MaKhuyenMai))
            {
    int lastId;
        if (int.TryParse(lastKhuyenMai.MaKhuyenMai.Replace("KM", ""), out lastId))
        {
      newId = lastId + 1;
         }
    }

            return "KM" + newId.ToString("D4");
        }

   private KhuyenMaiItemViewModel MapToKhuyenMaiItemViewModel(KhuyenMai km)
        {
   return new KhuyenMaiItemViewModel
    {
   KhuyenMaiId = km.KhuyenMaiId,
   MaKhuyenMai = km.MaKhuyenMai,
  TenKhuyenMai = km.TenKhuyenMai,
   GiaTri = km.GiaTri,
        NgayBatDau = km.NgayBatDau,
        NgayKetThuc = km.NgayKetThuc,
    DieuKienApDung = km.DieuKienApDung,
    SoLanSuDungToiDa = km.SoLanSuDungToiDa,
             SoLanDaSuDung = db.DatPhongs.Count(dp => dp.MaKhuyenMai == km.KhuyenMaiId),
            DaHoatDong = km.DaHoatDong
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
