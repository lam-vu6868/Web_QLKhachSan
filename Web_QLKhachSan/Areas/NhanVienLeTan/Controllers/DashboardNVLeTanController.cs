using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    public class DashboardNVLeTanController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: NhanVienLeTan/DashboardNVLeTan
        public ActionResult Index()
        {
            return View();
        }

        // API: Thống kê tổng quan
        public JsonResult GetThongKeTongQuan()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                // Đếm đặt phòng hôm nay
                var datPhongHomNay = db.DatPhongs
                .Count(d => DbFunctions.TruncateTime(d.NgayTao) == today);

                // Đếm check-in hôm nay
                var checkInHomNay = db.DatPhongs
                .Count(d => d.NgayNhan.HasValue &&
                                DbFunctions.TruncateTime(d.NgayNhan.Value) == today &&
                        d.TrangThaiDatPhong == 1);

                // Đếm check-out hôm nay
                var checkOutHomNay = db.DatPhongs
                    .Count(d => d.NgayTra.HasValue &&
                                DbFunctions.TruncateTime(d.NgayTra.Value) == today &&
                                d.ChiTietDatPhongs.Any(ct => ct.TrangThaiPhong == 1));

                // Thống kê phòng
                var tongPhong = db.Phongs.Count(p => p.DaHoatDong);
                var phongTrong = db.Phongs.Count(p => p.DaHoatDong && p.TrangThaiPhong == 0);
                var phongDangO = db.Phongs.Count(p => p.DaHoatDong && p.TrangThaiPhong == 1);
                var phongDangDon = db.Phongs.Count(p => p.DaHoatDong && p.TrangThaiPhong == 2);

                // Doanh thu hôm nay
                var doanhThuHomNay = db.ThanhToans
                    .Where(t => DbFunctions.TruncateTime(t.NgayThanhToan) == today &&
                                t.TrangThaiThanhToan == 1)
                    .Sum(t => (decimal?)t.SoTien) ?? 0;

                var result = new
                {
                    datPhongHomNay = datPhongHomNay,
                    checkInHomNay = checkInHomNay,
                    checkOutHomNay = checkOutHomNay,
                    tongPhong = tongPhong,
                    phongTrong = phongTrong,
                    phongDangO = phongDangO,
                    phongDangDon = phongDangDon,
                    tyLePhong = tongPhong > 0 ? (phongDangO * 100.0 / tongPhong) : 0,
                    doanhThuHomNay = doanhThuHomNay
                };

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Check-in sắp tới
        public JsonResult GetCheckInSapToi()
        {
            try
            {
                var today = DateTime.Today;
                var datPhongs = db.DatPhongs
                    .Include(d => d.KhachHang)
                    .Include(d => d.ChiTietDatPhongs.Select(ct => ct.Phong))
                    .Where(d => d.TrangThaiDatPhong == 1 &&
                                d.NgayNhan.HasValue &&
                                DbFunctions.TruncateTime(d.NgayNhan.Value) == today)
                    .OrderBy(d => d.NgayNhan)
                    .Take(5)
                    .ToList()
                    .Select(d => new
                    {
                        maDatPhong = d.MaDatPhong,
                        tenKhachHang = d.KhachHang?.HoVaTen ?? "Khách lẻ",
                        soDienThoai = d.KhachHang?.SoDienThoai,
                        gioNhan = d.NgayNhan?.ToString("HH:mm"),
                        soPhong = d.ChiTietDatPhongs.Count,
                        danhSachPhong = string.Join(", ", d.ChiTietDatPhongs.Select(ct => ct.Phong?.TenPhong ?? "Chưa chọn"))
                    })
                    .ToList();

                return Json(new { success = true, data = datPhongs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Check-out sắp tới
        public JsonResult GetCheckOutSapToi()
        {
            try
            {
                var today = DateTime.Today;
                var datPhongs = db.DatPhongs
                    .Include(d => d.KhachHang)
                    .Include(d => d.ChiTietDatPhongs.Select(ct => ct.Phong))
                    .Where(d => d.TrangThaiDatPhong == 1 &&
                                d.NgayTra.HasValue &&
                                DbFunctions.TruncateTime(d.NgayTra.Value) == today &&
                                d.ChiTietDatPhongs.Any(ct => ct.TrangThaiPhong == 1))
                    .OrderBy(d => d.NgayTra)
                    .Take(5)
                    .ToList()
                    .Select(d => new
                    {
                        maDatPhong = d.MaDatPhong,
                        tenKhachHang = d.KhachHang?.HoVaTen ?? "Khách lẻ",
                        soDienThoai = d.KhachHang?.SoDienThoai,
                        gioTra = d.NgayTra?.ToString("HH:mm"),
                        soPhong = d.ChiTietDatPhongs.Count,
                        danhSachPhong = string.Join(", ", d.ChiTietDatPhongs.Select(ct => ct.Phong?.TenPhong)),
                        daThanhToan = d.TrangThaiThanhToan == 1
                    })
                    .ToList();

                return Json(new { success = true, data = datPhongs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // API: Hoạt động gần đây
        public JsonResult GetHoatDongGanDay()
        {
            try
            {
                var today = DateTime.Today;

                // Lấy check-in gần đây
                var checkIns = db.DatPhongs
                    .Include(d => d.KhachHang)
                    .Where(d => d.NgayCapNhat.HasValue &&
                                 d.NgayCapNhat.Value >= today &&
                                 d.GhiChu != null && d.GhiChu.Contains("Check-in"))
                    .OrderByDescending(d => d.NgayCapNhat)
                    .Take(5)
                    .ToList()
                    .Select(d => new
                    {
                        loai = "check-in",
                        icon = "fa-sign-in-alt",
                        mau = "success",
                        tieuDe = "Check-in thành công",
                        noiDung = $"{d.KhachHang?.HoVaTen ?? "Khách"} - {d.MaDatPhong}",
                        thoiGian = d.NgayCapNhat?.ToString("HH:mm"),
                        sortTime = d.NgayCapNhat ?? DateTime.MinValue
                    })
                    .ToList();

                // Lấy thanh toán gần đây
                var payments = db.ThanhToans
                   .Include(t => t.HoaDon)
                    .Include(t => t.HoaDon.KhachHang)
                    .Where(t => DbFunctions.TruncateTime(t.NgayThanhToan) == today)
                  .OrderByDescending(t => t.NgayThanhToan)
                   .Take(5)
                    .ToList()
          .Select(t => new
                    {
 loai = "payment",
                  icon = "fa-credit-card",
               mau = "primary",
                  tieuDe = "Thanh toán hoàn tất",
                 noiDung = $"{t.HoaDon?.KhachHang?.HoVaTen ?? "Khách"} - {FormatCurrency(t.SoTien)}",
              thoiGian = t.NgayThanhToan.ToString("HH:mm"),
                sortTime = t.NgayThanhToan
             })
      .ToList();

                // Kết hợp và sắp xếp
                var result = checkIns.Concat(payments)
   .OrderByDescending(a => a.sortTime)
            .Take(10)
.Select(a => new
            {
       loai = a.loai,
 icon = a.icon,
  mau = a.mau,
          tieuDe = a.tieuDe,
    noiDung = a.noiDung,
             thoiGian = a.thoiGian
      })
   .ToList();

                return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Helper method
        private string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0") + " VNĐ";
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