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
        public ActionResult LichSuSuDung(int? id, int CurrentPage = 1, int PageSize = 10)
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

    // Lấy danh sách chi tiết sử dụng dịch vụ
            var query = db.ChiTietDatDichVus
    .Include(c => c.DatPhong)
        .Include(c => c.DatPhong.KhachHang)
      .Where(c => c.DichVuId == id)
.OrderByDescending(c => c.NgaySuDung ?? c.DatPhong.NgayDat);

var totalRecords = query.Count();
            var danhSach = query
                .Skip((CurrentPage - 1) * PageSize)
    .Take(PageSize)
            .ToList();

   ViewBag.DichVu = dichVu;
            ViewBag.CurrentPage = CurrentPage;
         ViewBag.PageSize = PageSize;
            ViewBag.TotalRecords = totalRecords;
    ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            // Thống kê
   var allRecords = db.ChiTietDatDichVus.Where(c => c.DichVuId == id).ToList();
          ViewBag.TongSoLanSuDung = allRecords.Count;
     ViewBag.TongDoanhThu = allRecords.Sum(c => c.ThanhTien ?? 0);
     ViewBag.SoLuongTrungBinh = allRecords.Any() ? allRecords.Average(c => c.SoLuong) : 0;

     return View(danhSach);
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
    }
}
