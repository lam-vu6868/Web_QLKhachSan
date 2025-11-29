using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienBuongPhong.ViewModels.CongViec;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienBuongPhong.Controllers
{
    /// <summary>
    /// Controller quản lý công việc cho Nhân viên Buồng Phòng
    /// </summary>
    [NhanVienAuthorize]
    public class CongViecController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        /// <summary>
        /// Kiểm tra quyền truy cập - chỉ nhân viên buồng phòng
        /// </summary>
        private bool CheckRole()
        {
            string vaiTro = Session["VaiTro"]?.ToString();
            return vaiTro == "NhanVienBuongPhong" || 
                   vaiTro == "Nhân viên buồng phòng" || 
                   vaiTro == "Buồng Phòng" || 
                   vaiTro == "Housekeeping" ||
                   vaiTro == "Admin" || 
                   vaiTro == "Quản lý";
        }

        /// <summary>
        /// Lấy NhanVienId từ Session
        /// </summary>
        private int? GetCurrentNhanVienId()
        {
            return Session["NhanVienId"] as int?;
        }

        // GET: Index - Danh sách công việc được phân công
        public ActionResult Index(CongViecFilterViewModel filter)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                int? nhanVienId = GetCurrentNhanVienId();
                if (!nhanVienId.HasValue)
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập lại!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                var viewModel = new CongViecListViewModel();
                viewModel.Filter = filter ?? new CongViecFilterViewModel();

                // Lấy danh sách công việc được phân công cho nhân viên hiện tại
                var query = db.PhanCongCongViecs
                    .Include(pc => pc.Phong)
                    .Include(pc => pc.NhanVien1) // NhanVienLeTan
                    .Where(pc => pc.NhanVienBuongPhongId == nhanVienId.Value)
                    .AsQueryable();

                // ===== FILTERS =====

                // Tìm kiếm
                if (!string.IsNullOrWhiteSpace(filter.TimKiem))
                {
                    string searchTerm = filter.TimKiem.Trim().ToLower();
                    query = query.Where(pc =>
                        pc.MaPhanCong.ToLower().Contains(searchTerm) ||
                        pc.Phong.MaPhong.ToLower().Contains(searchTerm) ||
                        pc.Phong.TenPhong.ToLower().Contains(searchTerm) ||
                        (pc.GhiChu != null && pc.GhiChu.ToLower().Contains(searchTerm)));
                }

                // Lọc theo trạng thái
                if (filter.TrangThaiCongViec.HasValue)
                {
                    query = query.Where(pc => pc.TrangThaiCongViec == filter.TrangThaiCongViec.Value);
                }

                // Lọc theo mức độ ưu tiên
                if (filter.MucDoUuTien.HasValue)
                {
                    query = query.Where(pc => pc.MucDoUuTien == filter.MucDoUuTien.Value);
                }

                // Lọc theo ngày phân công
                if (filter.TuNgayPhanCong.HasValue)
                {
                    query = query.Where(pc => DbFunctions.TruncateTime(pc.ThoiGianPhanCong) >= filter.TuNgayPhanCong.Value);
                }

                if (filter.DenNgayPhanCong.HasValue)
                {
                    query = query.Where(pc => DbFunctions.TruncateTime(pc.ThoiGianPhanCong) <= filter.DenNgayPhanCong.Value);
                }

                // ===== PHÂN TRANG =====
                int totalRecords = query.Count();
                int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
                int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;

                viewModel.TotalRecords = totalRecords;
                viewModel.CurrentPage = currentPage;
                viewModel.PageSize = pageSize;

                // ===== LẤY DANH SÁCH =====
                var congViecs = query
                    .OrderByDescending(pc => pc.MucDoUuTien) // Ưu tiên mức độ ưu tiên cao trước
                    .ThenByDescending(pc => pc.ThoiGianPhanCong) // Sau đó sắp xếp theo thời gian phân công
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                viewModel.DanhSachCongViec = congViecs.Select(pc => MapToCongViecItemViewModel(pc)).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - CongViec/Index] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách công việc!";
                return View(new CongViecListViewModel());
            }
        }

        // GET: Details - Xem chi tiết công việc
        public ActionResult Details(int? id)
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            int? nhanVienId = GetCurrentNhanVienId();
            if (!nhanVienId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập lại!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc!";
                return RedirectToAction("Index");
            }

            var phanCong = db.PhanCongCongViecs
                .Include(pc => pc.Phong)
                .Include(pc => pc.NhanVien1) // NhanVienLeTan
                .FirstOrDefault(pc => pc.PhanCongId == id.Value && pc.NhanVienBuongPhongId == nhanVienId.Value);

            if (phanCong == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc hoặc bạn không có quyền xem công việc này!";
                return RedirectToAction("Index");
            }

            var viewModel = MapToCongViecItemViewModel(phanCong);
            return View(viewModel);
        }

        // POST: BatDau - Bắt đầu công việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BatDau(int? id)
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            int? nhanVienId = GetCurrentNhanVienId();
            if (!nhanVienId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập lại!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc!";
                return RedirectToAction("Index");
            }

            try
            {
                var phanCong = db.PhanCongCongViecs
                    .FirstOrDefault(pc => pc.PhanCongId == id.Value && pc.NhanVienBuongPhongId == nhanVienId.Value);

                if (phanCong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy công việc hoặc bạn không có quyền thực hiện thao tác này!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra trạng thái - chỉ có thể bắt đầu khi trạng thái = Chờ xử lý
                if (phanCong.TrangThaiCongViec != 0)
                {
                    TempData["ErrorMessage"] = "Chỉ có thể bắt đầu công việc ở trạng thái 'Chờ xử lý'!";
                    return RedirectToAction("Index");
                }

                // Cập nhật trạng thái và thời gian bắt đầu
                phanCong.TrangThaiCongViec = 1; // Đang làm
                phanCong.ThoiGianBatDau = DateTime.Now;
                phanCong.NgayCapNhat = DateTime.Now;

                db.SaveChanges();

                TempData["SuccessMessage"] = $"Đã bắt đầu công việc {phanCong.MaPhanCong} thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - CongViec/BatDau] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi bắt đầu công việc!";
                return RedirectToAction("Index");
            }
        }

        // POST: HoanThanh - Hoàn thành công việc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoanThanh(int? id)
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            int? nhanVienId = GetCurrentNhanVienId();
            if (!nhanVienId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập lại!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy công việc!";
                return RedirectToAction("Index");
            }

            try
            {
                var phanCong = db.PhanCongCongViecs
                    .Include(pc => pc.Phong)
                    .FirstOrDefault(pc => pc.PhanCongId == id.Value && pc.NhanVienBuongPhongId == nhanVienId.Value);

                if (phanCong == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy công việc hoặc bạn không có quyền thực hiện thao tác này!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra trạng thái - chỉ có thể hoàn thành khi trạng thái = Đang làm
                if (phanCong.TrangThaiCongViec != 1)
                {
                    TempData["ErrorMessage"] = "Chỉ có thể hoàn thành công việc ở trạng thái 'Đang làm'!";
                    return RedirectToAction("Index");
                }

                // Cập nhật trạng thái và thời gian hoàn thành
                phanCong.TrangThaiCongViec = 2; // Hoàn thành
                phanCong.ThoiGianHoanThanh = DateTime.Now;
                phanCong.NgayCapNhat = DateTime.Now;

                // Cập nhật trạng thái phòng thành "Trống" (TrangThaiPhong = 0) sau khi dọn dẹp xong
                if (phanCong.Phong != null)
                {
                    phanCong.Phong.TrangThaiPhong = 0; // Trống - sẵn sàng cho khách đặt
                }

                db.SaveChanges();

                TempData["SuccessMessage"] = $"Đã hoàn thành công việc {phanCong.MaPhanCong} thành công! Phòng {phanCong.Phong?.MaPhong} đã được cập nhật trạng thái thành 'Trống'.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - CongViec/HoanThanh] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hoàn thành công việc!";
                return RedirectToAction("Index");
            }
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Map từ PhanCongCongViec entity sang CongViecItemViewModel
        /// </summary>
        private CongViecItemViewModel MapToCongViecItemViewModel(PhanCongCongViec phanCong)
        {
            return new CongViecItemViewModel
            {
                PhanCongId = phanCong.PhanCongId,
                MaPhanCong = phanCong.MaPhanCong,
                PhongId = phanCong.PhongId,
                MaPhong = phanCong.Phong?.MaPhong,
                TenPhong = phanCong.Phong?.TenPhong,
                Tang = phanCong.Phong?.Tang,
                NhanVienLeTanId = phanCong.NhanVienLeTanId,
                TenNhanVienLeTan = phanCong.NhanVien1?.HoVaTen, // NhanVien1 = NhanVienLeTan
                TrangThaiCongViec = phanCong.TrangThaiCongViec,
                MucDoUuTien = phanCong.MucDoUuTien,
                GhiChu = phanCong.GhiChu,
                GhiChuNhanVien = phanCong.GhiChuNhanVien,
                ThoiGianPhanCong = phanCong.ThoiGianPhanCong,
                ThoiGianBatDau = phanCong.ThoiGianBatDau,
                ThoiGianHoanThanh = phanCong.ThoiGianHoanThanh,
                ThoiGianDuKien = phanCong.ThoiGianDuKien,
                NgayTao = phanCong.NgayTao
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

