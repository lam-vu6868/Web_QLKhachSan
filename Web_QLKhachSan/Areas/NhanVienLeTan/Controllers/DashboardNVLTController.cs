using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller Dashboard cho Nhân viên Lễ Tân
    /// Hiển thị tổng quan về trạng thái khách sạn
    /// </summary>
    [NhanVienAuthorize]
    public class DashboardNVLTController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        /// <summary>
        /// Kiểm tra authorization - vai trò
        /// </summary>
        private bool CheckRole()
        {
            string vaiTro = Session["VaiTro"]?.ToString();
            return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        // GET: NhanVienLeTan/DashboardNVLT
        public ActionResult Index()
        {
            try
            {
                // ✅ Kiểm tra Role
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                var viewModel = new DashboardViewModel();
                DateTime today = DateTime.Now.Date;

                // ===== 1. THỐNG KÊ PHÒNG =====
                var phongs = db.Phongs.Where(p => p.DaHoatDong).ToList();

                viewModel.ThongKePhong = new ThongKePhongViewModel
                {
                    TongSoPhong = phongs.Count,
                    SoPhongTrong = phongs.Count(p => p.TrangThaiPhong == 0),
                    SoPhongDaDat = phongs.Count(p => p.TrangThaiPhong == 1),
                    SoPhongDangO = phongs.Count(p => p.TrangThaiPhong == 2),
                    SoPhongDangDon = phongs.Count(p => p.TrangThaiPhong == 3),
                    SoPhongBaoTri = phongs.Count(p => p.TrangThaiPhong == 4)
                };

                // ===== 2. THỐNG KÊ ĐẶT PHÒNG =====
                viewModel.SoDonChoXacNhan = db.DatPhongs
                    .Count(dp => dp.TrangThaiDatPhong == 0 && dp.NgayNhan >= today);

                viewModel.TongDonDatHomNay = db.DatPhongs
                    .Count(dp => dp.NgayDat.Year == today.Year
                        && dp.NgayDat.Month == today.Month
                        && dp.NgayDat.Day == today.Day);

                // ===== 3. THỐNG KÊ DOANH THU =====
                // Doanh thu hôm nay (từ hóa đơn đã thanh toán)
                viewModel.DoanhThuHomNay = db.HoaDons
                    .Where(hd => hd.TrangThaiHoaDon == 2
                                 && hd.NgayThanhToan.HasValue
                                 && hd.NgayThanhToan.Value.Year == today.Year
                                 && hd.NgayThanhToan.Value.Month == today.Month
                                 && hd.NgayThanhToan.Value.Day == today.Day)
                    .Sum(hd => (decimal?)hd.ThanhToanCuoi) ?? 0;

                // Doanh thu tháng này
                viewModel.DoanhThuThangNay = db.HoaDons
                    .Where(hd => hd.TrangThaiHoaDon == 2
                                  && hd.NgayThanhToan.HasValue
                                  && hd.NgayThanhToan.Value.Year == today.Year
                                  && hd.NgayThanhToan.Value.Month == today.Month)
                    .Sum(hd => (decimal?)hd.ThanhToanCuoi) ?? 0;

                // Số hóa đơn chưa thanh toán
                viewModel.SoHoaDonChuaThanhToan = db.HoaDons
                    .Count(hd => hd.TrangThaiHoaDon == 0 || hd.TrangThaiHoaDon == 1);

                // ===== 4. DANH SÁCH CHECK-IN HÔM NAY =====
                var checkInHomNay = db.DatPhongs
                    .Where(dp => dp.TrangThaiDatPhong == 1 // Đã xác nhận
                                  && dp.NgayNhan.HasValue
                                  && dp.NgayNhan.Value.Year == today.Year
                                  && dp.NgayNhan.Value.Month == today.Month
                                  && dp.NgayNhan.Value.Day == today.Day)
                    .OrderBy(dp => dp.NgayNhan)
                    .ToList();

                viewModel.DanhSachCheckInHomNay = checkInHomNay.Select(dp => MapToCheckInOutViewModel(dp)).ToList();

                // ===== 5. DANH SÁCH CHECK-OUT HÔM NAY =====
                var checkOutHomNay = db.DatPhongs
                    .Where(dp => dp.TrangThaiDatPhong == 2 // Đã check-in
                                  && dp.NgayTra.HasValue
                                  && dp.NgayTra.Value.Year == today.Year
                                  && dp.NgayTra.Value.Month == today.Month
                                  && dp.NgayTra.Value.Day == today.Day)
                    .OrderBy(dp => dp.NgayTra)
                    .ToList();

                viewModel.DanhSachCheckOutHomNay = checkOutHomNay.Select(dp => MapToCheckInOutViewModel(dp)).ToList();

                // ===== 6. THÔNG BÁO CẦN CHÚ Ý =====
                viewModel.ThongBaoCanChuY = GenerateThongBao();

                // ===== 7. DOANH THU 7 NGÀY GẦN NHẤT =====
                viewModel.DoanhThu7NgayGanNhat = GetDoanhThu7Ngay();

                // ===== 8. THÔNG TIN NHÂN VIÊN =====
                viewModel.TenNhanVien = Session["HoVaTen"]?.ToString() ?? "Nhân viên";
                viewModel.VaiTro = Session["VaiTro"]?.ToString() ?? "Lễ Tân";
                viewModel.ThoiGianDangNhap = Session["DangNhapLuc"] as DateTime?;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - Dashboard/Index] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ERROR - StackTrace] {ex.StackTrace}");

                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dashboard. Vui lòng thử lại!";
                return View(new DashboardViewModel());
            }
        }

        /// <summary>
        /// Map DatPhong entity sang CheckInOutViewModel
        /// </summary>
        private CheckInOutViewModel MapToCheckInOutViewModel(DatPhong datPhong)
        {
            if (datPhong == null) return null;

            // Lấy phòng đầu tiên trong đơn đặt (có thể có nhiều phòng)
            var chiTietDatPhong = datPhong.ChiTietDatPhongs.FirstOrDefault();
            string tenPhong = "Chưa gán";
            string loaiPhong = "N/A";

            if (chiTietDatPhong != null)
            {
                if (chiTietDatPhong.Phong != null)
                {
                    tenPhong = chiTietDatPhong.Phong.MaPhong;
                }
                else if (chiTietDatPhong.LoaiPhong != null)
                {
                    loaiPhong = chiTietDatPhong.LoaiPhong.TenLoai;
                }
            }

            return new CheckInOutViewModel
            {
                DatPhongId = datPhong.DatPhongId,
                MaDatPhong = datPhong.MaDatPhong,
                TenKhachHang = datPhong.KhachHang?.HoVaTen ?? "N/A",
                SoDienThoai = datPhong.KhachHang?.SoDienThoai,
                TenPhong = tenPhong,
                LoaiPhong = loaiPhong,
                NgayCheckIn = datPhong.NgayNhan,
                NgayCheckOut = datPhong.NgayTra,
                SoNguoi = datPhong.SoLuongKhach,
                TrangThai = datPhong.TrangThaiDatPhong,
                GhiChu = datPhong.GhiChu
            };
        }

        /// <summary>
        /// Tạo danh sách thông báo cần chú ý
        /// </summary>
        private List<ThongBaoViewModel> GenerateThongBao()
        {
            var thongBaoList = new List<ThongBaoViewModel>();
            DateTime now = DateTime.Now;

            try
            {
                // 1. Thông báo phòng quá giờ check-out
                var phongQuaGioCheckOut = db.ChiTietDatPhongs
                    .Where(ct => ct.DatPhong.TrangThaiDatPhong == 2
                                  && ct.NgayDi.HasValue
                                  && ct.NgayDi.Value < now)
                    .Take(5)
                    .ToList();

                foreach (var ct in phongQuaGioCheckOut)
                {
                    thongBaoList.Add(new ThongBaoViewModel
                    {
                        LoaiThongBao = "danger",
                        Icon = "fa-exclamation-triangle",
                        TieuDe = $"Phòng {ct.Phong?.MaPhong ?? "N/A"} quá giờ check-out",
                        NoiDung = $"Khách đã quá giờ check-out. Vui lòng kiểm tra!",
                        Link = Url.Action("Index", "SoDoPhong"),
                        ThoiGian = ct.NgayDi.Value
                    });
                }

                // 2. Thông báo đơn đặt chờ xác nhận
                int soDonChoXacNhan = db.DatPhongs.Count(dp => dp.TrangThaiDatPhong == 0);
                if (soDonChoXacNhan > 0)
                {
                    thongBaoList.Add(new ThongBaoViewModel
                    {
                        LoaiThongBao = "warning",
                        Icon = "fa-clock",
                        TieuDe = $"{soDonChoXacNhan} đơn đặt phòng chờ xác nhận",
                        NoiDung = "Vui lòng xác nhận các đơn đặt phòng mới",
                        Link = Url.Action("Index", "DatPhong"),
                        ThoiGian = DateTime.Now
                    });
                }

                // 3. Thông báo phòng cần dọn
                int soPhongCanDon = db.Phongs.Count(p => p.TrangThaiPhong == 3 && p.DaHoatDong);
                if (soPhongCanDon > 0)
                {
                    thongBaoList.Add(new ThongBaoViewModel
                    {
                        LoaiThongBao = "info",
                        Icon = "fa-broom",
                        TieuDe = $"{soPhongCanDon} phòng đang chờ dọn dẹp",
                        NoiDung = "Vui lòng thông báo bộ phận housekeeping",
                        Link = Url.Action("Index", "SoDoPhong"),
                        ThoiGian = DateTime.Now
                    });
                }

                // 4. Thông báo check-in sắp tới (trong vòng 2 giờ)
                var checkInSapToi = db.DatPhongs
                    .Where(dp => dp.TrangThaiDatPhong == 1
                                  && dp.NgayNhan.HasValue
                                  && dp.NgayNhan.Value > now
                                  && dp.NgayNhan.Value <= now.AddHours(2))
                    .ToList();

                foreach (var dp in checkInSapToi)
                {
                    thongBaoList.Add(new ThongBaoViewModel
                    {
                        LoaiThongBao = "info",
                        Icon = "fa-bell",
                        TieuDe = $"Check-in sắp tới: {dp.KhachHang?.HoVaTen}",
                        NoiDung = $"Khách sẽ check-in lúc {dp.NgayNhan.Value:HH:mm}",
                        Link = Url.Action("Index", "CheckIn"),
                        ThoiGian = dp.NgayNhan.Value
                    });
                }

                // Sắp xếp theo thời gian (mới nhất trước)
                return thongBaoList.OrderByDescending(t => t.ThoiGian).Take(10).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - GenerateThongBao] {ex.Message}");
                return new List<ThongBaoViewModel>();
            }
        }

        /// <summary>
        /// Lấy doanh thu 7 ngày gần nhất cho Chart.js
        /// </summary>
        private List<DoanhThuNgayViewModel> GetDoanhThu7Ngay()
        {
            var result = new List<DoanhThuNgayViewModel>();
            DateTime today = DateTime.Now.Date;

            try
            {
                for (int i = 6; i >= 0; i--)
                {
                    DateTime ngay = today.AddDays(-i);

                    var doanhThu = db.HoaDons
                        .Where(hd => hd.TrangThaiHoaDon == 2
                                      && hd.NgayThanhToan.HasValue
                                      && hd.NgayThanhToan.Value.Year == ngay.Year
                                      && hd.NgayThanhToan.Value.Month == ngay.Month
                                      && hd.NgayThanhToan.Value.Day == ngay.Day)
                        .Sum(hd => (decimal?)hd.ThanhToanCuoi) ?? 0;

                    var soDon = db.HoaDons
                        .Count(hd => hd.TrangThaiHoaDon == 2
                                      && hd.NgayThanhToan.HasValue
                                      && hd.NgayThanhToan.Value.Year == ngay.Year
                                      && hd.NgayThanhToan.Value.Month == ngay.Month
                                      && hd.NgayThanhToan.Value.Day == ngay.Day);

                    result.Add(new DoanhThuNgayViewModel
                    {
                        Ngay = ngay,
                        DoanhThu = doanhThu,
                        SoDon = soDon
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - GetDoanhThu7Ngay] {ex.Message}");
            }

            return result;
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