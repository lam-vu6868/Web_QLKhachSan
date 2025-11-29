using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.PhanCong;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý phân công công việc cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class PhanCongController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        private bool CheckRole()
        {
            string vaiTro = Session["VaiTro"]?.ToString();
            return vaiTro == "LeTan" || vaiTro == "Nhân viên lễ tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        // GET: Index
        public ActionResult Index(PhanCongFilterViewModel filter)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                var viewModel = new PhanCongListViewModel();
                viewModel.Filter = filter ?? new PhanCongFilterViewModel();

                var query = db.PhanCongCongViecs
                    .Include(pc => pc.Phong)
                    .Include(pc => pc.NhanVien) // NhanVienBuongPhong
                    .Include(pc => pc.NhanVien1) // NhanVienLeTan
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
                        (pc.NhanVien != null && pc.NhanVien.HoVaTen.ToLower().Contains(searchTerm)) ||
                        (pc.NhanVien != null && pc.NhanVien.MaNV.ToLower().Contains(searchTerm)));
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

                // Lọc theo nhân viên buồng phòng
                if (filter.NhanVienBuongPhongId.HasValue)
                {
                    query = query.Where(pc => pc.NhanVienBuongPhongId == filter.NhanVienBuongPhongId.Value);
                }

                // Lọc theo phòng
                if (filter.PhongId.HasValue)
                {
                    query = query.Where(pc => pc.PhongId == filter.PhongId.Value);
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
                var phanCongs = query
                    .OrderByDescending(pc => pc.ThoiGianPhanCong)
                    .ThenByDescending(pc => pc.MucDoUuTien)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                viewModel.DanhSachPhanCong = phanCongs.Select(pc => MapToPhanCongItemViewModel(pc)).ToList();

                // ===== DROPDOWN DATA CHO FILTER =====
                var nhanVienBuongPhongFilter = db.NhanViens
                    .Where(nv => nv.VaiTro == "NhanVienBuongPhong" || 
                                 nv.VaiTro == "Nhân viên buồng phòng" || 
                                 nv.VaiTro == "Buồng Phòng" || 
                                 nv.VaiTro == "Housekeeping")
                    .Where(nv => nv.DaHoatDong)
                    .OrderBy(nv => nv.HoVaTen)
                    .ToList();

                ViewBag.DanhSachNhanVienBuongPhong = nhanVienBuongPhongFilter
                    .Select(nv => new SelectListItem
                    {
                        Value = nv.NhanVienId.ToString(),
                        Text = nv.HoVaTen + " (" + nv.MaNV + ")"
                    })
                    .ToList();

                ViewBag.DanhSachPhong = db.Phongs
                    .Where(p => p.DaHoatDong)
                    .OrderBy(p => p.MaPhong)
                    .Select(p => new SelectListItem
                    {
                        Value = p.PhongId.ToString(),
                        Text = p.MaPhong + " - " + p.TenPhong
                    })
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - PhanCong/Index] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách phân công!";
                return View(new PhanCongListViewModel());
            }
        }

        // GET: Create
        public ActionResult Create()
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            var viewModel = new PhanCongFormViewModel();
            LoadDropdownData(viewModel);
            return View(viewModel);
        }

        private void LoadDropdownData(PhanCongFormViewModel viewModel, int? selectedPhongId = null, int? selectedNhanVienId = null)
        {
            // Lấy danh sách phòng cần dọn
            viewModel.DanhSachPhong = new SelectList(
                db.Phongs
                    .Where(p => p.DaHoatDong && (p.TrangThaiPhong == 3 || p.TrangThaiPhong == 2))
                    .OrderBy(p => p.MaPhong)
                    .Select(p => new { p.PhongId, DisplayText = p.MaPhong + " - " + p.TenPhong }),
                "PhongId",
                "DisplayText",
                selectedPhongId
            );

            // Lấy danh sách nhân viên buồng phòng
            var nhanVienBuongPhong = db.NhanViens
                .Where(nv => nv.DaHoatDong && 
                    (nv.VaiTro == "NhanVienBuongPhong" || 
                     nv.VaiTro == "Nhân viên buồng phòng" || 
                     nv.VaiTro == "Buồng Phòng" || 
                     nv.VaiTro == "Housekeeping"))
                .OrderBy(nv => nv.HoVaTen)
                .ToList();

            if (nhanVienBuongPhong.Count == 0)
            {
                nhanVienBuongPhong = db.NhanViens
                    .Where(nv => nv.DaHoatDong)
                    .OrderBy(nv => nv.HoVaTen)
                    .Take(20)
                    .ToList();
            }

            viewModel.DanhSachNhanVienBuongPhong = new SelectList(
                nhanVienBuongPhong.Select(nv => new { 
                    nv.NhanVienId, 
                    DisplayText = (nv.HoVaTen ?? "N/A") + " (" + (nv.MaNV ?? "N/A") + ")" 
                }),
                "NhanVienId",
                "DisplayText",
                selectedNhanVienId
            );
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhanCongFormViewModel model)
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            // Debug: Log giá trị nhận được
            System.Diagnostics.Debug.WriteLine($"[DEBUG] PhongId: {model.PhongId}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] NhanVienBuongPhongId: {model.NhanVienBuongPhongId}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] MucDoUuTien: {model.MucDoUuTien}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ThoiGianDuKien: {model.ThoiGianDuKien}");

            // Validation bổ sung
            if (model.PhongId <= 0)
            {
                ModelState.AddModelError("PhongId", "Vui lòng chọn phòng!");
            }

            if (model.NhanVienBuongPhongId <= 0)
            {
                ModelState.AddModelError("NhanVienBuongPhongId", "Vui lòng chọn nhân viên buồng phòng!");
            }

            // Validation thời gian dự kiến
            if (model.ThoiGianDuKien.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime minDateTime = now.AddMinutes(-30); // Cho phép 30 phút trong quá khứ
                DateTime maxDateTime = now.AddYears(1); // Không quá 1 năm trong tương lai

                if (model.ThoiGianDuKien.Value < minDateTime)
                {
                    ModelState.AddModelError("ThoiGianDuKien", "Thời gian dự kiến không được quá 30 phút trong quá khứ!");
                }
                else if (model.ThoiGianDuKien.Value > maxDateTime)
                {
                    ModelState.AddModelError("ThoiGianDuKien", "Thời gian dự kiến không được quá 1 năm trong tương lai!");
                }
            }

            // Log tất cả ModelState errors
            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModelState Errors:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {key}: {error.ErrorMessage}");
                        }
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                return View(model);
            }

            try
            {
                // Kiểm tra phòng và nhân viên có tồn tại không
                var phong = db.Phongs.Find(model.PhongId);
                if (phong == null)
                {
                    ModelState.AddModelError("PhongId", "Phòng không tồn tại!");
                    LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                    return View(model);
                }

                var nhanVien = db.NhanViens.Find(model.NhanVienBuongPhongId);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("NhanVienBuongPhongId", "Nhân viên không tồn tại!");
                    LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                    return View(model);
                }

                // Lấy NhanVienId từ Session
                int? nhanVienLeTanId = Session["NhanVienId"] as int?;

                // Tạo MaPhanCong trực tiếp trong code (thay vì dùng trigger)
                string maPhanCong = TaoMaPhanCong();

                // Tạo phân công mới
                var phanCong = new PhanCongCongViec
                {
                    MaPhanCong = maPhanCong,
                    PhongId = model.PhongId,
                    NhanVienBuongPhongId = model.NhanVienBuongPhongId,
                    NhanVienLeTanId = nhanVienLeTanId,
                    TrangThaiCongViec = 0, // Chờ xử lý
                    MucDoUuTien = model.MucDoUuTien,
                    GhiChu = model.GhiChu,
                    ThoiGianPhanCong = DateTime.Now,
                    ThoiGianDuKien = model.ThoiGianDuKien,
                    NgayTao = DateTime.Now
                };

                db.PhanCongCongViecs.Add(phanCong);
                db.SaveChanges();

                // Cập nhật trạng thái phòng thành "Đang dọn"
                if (phong.TrangThaiPhong != 3)
                {
                    phong.TrangThaiPhong = 3;
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = $"Phân công công việc dọn phòng {phong.MaPhong} thành công!";
                return RedirectToAction("Index");
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                // Log chi tiết lỗi SQL
                System.Diagnostics.Debug.WriteLine($"[SQL ERROR] Number: {sqlEx.Number}");
                System.Diagnostics.Debug.WriteLine($"[SQL ERROR] Message: {sqlEx.Message}");
                System.Diagnostics.Debug.WriteLine($"[SQL ERROR] StackTrace: {sqlEx.StackTrace}");
                
                string errorMessage = "Có lỗi xảy ra khi tạo phân công!";
                
                if (sqlEx.Number == 2601) // Duplicate key
                {
                    errorMessage = "Mã phân công đã tồn tại. Vui lòng thử lại sau vài giây!";
                }
                else if (sqlEx.Number == 547) // Foreign key
                {
                    errorMessage = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin phòng và nhân viên!";
                }
                else if (sqlEx.Number == 515) // Cannot insert NULL
                {
                    errorMessage = "Thiếu thông tin bắt buộc. Vui lòng kiểm tra lại các trường bắt buộc!";
                }
                else if (sqlEx.Number == 2627) // Unique constraint violation
                {
                    errorMessage = "Dữ liệu trùng lặp. Vui lòng kiểm tra lại!";
                }
                else
                {
                    // Hiển thị lỗi SQL chi tiết hơn (nhưng lọc bớt thông tin kỹ thuật)
                    var sqlMessage = sqlEx.Message;
                    if (sqlMessage.Contains("MaPhanCong"))
                    {
                        errorMessage = "Lỗi tạo mã phân công. Vui lòng thử lại!";
                    }
                    else if (sqlMessage.Contains("trigger"))
                    {
                        errorMessage = "Lỗi xử lý dữ liệu. Vui lòng thử lại hoặc liên hệ quản trị viên!";
                    }
                    else
                    {
                        errorMessage = $"Lỗi database: {sqlMessage}";
                    }
                }
                
                ModelState.AddModelError("", errorMessage);
                LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                return View(model);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                // Log validation errors
                System.Diagnostics.Debug.WriteLine("[VALIDATION ERROR]");
                foreach (var validationError in dbEx.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"  - {error.PropertyName}: {error.ErrorMessage}");
                    }
                }
                
                ModelState.AddModelError("", "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường!");
                LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                return View(model);
            }
            catch (Exception ex)
            {
                // Log tất cả thông tin lỗi
                System.Diagnostics.Debug.WriteLine($"[GENERAL ERROR] Type: {ex.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[GENERAL ERROR] Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GENERAL ERROR] StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[INNER ERROR] Message: {ex.InnerException.Message}");
                }
                
                ModelState.AddModelError("", $"Có lỗi xảy ra khi tạo phân công: {ex.Message}");
                LoadDropdownData(model, model.PhongId, model.NhanVienBuongPhongId);
                return View(model);
            }
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            if (!CheckRole())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
            }

            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân công!";
                return RedirectToAction("Index");
            }

            var phanCong = db.PhanCongCongViecs
                .Include(pc => pc.Phong)
                .Include(pc => pc.NhanVien)
                .Include(pc => pc.NhanVien1)
                .FirstOrDefault(pc => pc.PhanCongId == id.Value);

            if (phanCong == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phân công!";
                return RedirectToAction("Index");
            }

            var viewModel = MapToPhanCongItemViewModel(phanCong);
            return View(viewModel);
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Tạo mã phân công tự động: PC-YYYYMMDD-XXX
        /// </summary>
        private string TaoMaPhanCong()
        {
            string ngayHienTai = DateTime.Now.ToString("yyyyMMdd");
            string prefix = "PC-" + ngayHienTai + "-";
            
            // Lấy số thứ tự lớn nhất trong ngày
            var maxMa = db.PhanCongCongViecs
                .Where(pc => pc.MaPhanCong != null && pc.MaPhanCong.StartsWith(prefix))
                .Select(pc => pc.MaPhanCong)
                .ToList()
                .Select(ma => 
                {
                    // Lấy phần số cuối cùng (sau dấu - cuối)
                    var parts = ma.Split('-');
                    if (parts.Length >= 3)
                    {
                        int soThuTu;
                        if (int.TryParse(parts[parts.Length - 1], out soThuTu))
                            return soThuTu;
                    }
                    return 0;
                })
                .DefaultIfEmpty(0)
                .Max();
            
            int soThuTuMoi = maxMa + 1;
            return prefix + soThuTuMoi.ToString("000");
        }

        /// <summary>
        /// Map từ PhanCongCongViec entity sang PhanCongItemViewModel
        /// </summary>
        private PhanCongItemViewModel MapToPhanCongItemViewModel(PhanCongCongViec phanCong)
        {
            return new PhanCongItemViewModel
            {
                PhanCongId = phanCong.PhanCongId,
                MaPhanCong = phanCong.MaPhanCong,
                PhongId = phanCong.PhongId,
                MaPhong = phanCong.Phong?.MaPhong,
                TenPhong = phanCong.Phong?.TenPhong,
                Tang = phanCong.Phong?.Tang,
                NhanVienBuongPhongId = phanCong.NhanVienBuongPhongId,
                TenNhanVienBuongPhong = phanCong.NhanVien?.HoVaTen,
                MaNhanVienBuongPhong = phanCong.NhanVien?.MaNV,
                NhanVienLeTanId = phanCong.NhanVienLeTanId,
                TenNhanVienLeTan = phanCong.NhanVien1?.HoVaTen,
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

