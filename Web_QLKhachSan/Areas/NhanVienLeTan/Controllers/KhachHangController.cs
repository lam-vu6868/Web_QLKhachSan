using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Web_QLKhachSan.Models;
using Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhachHang;
using Web_QLKhachSan.Filters;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.Controllers
{
    /// <summary>
    /// Controller quản lý khách hàng cho Nhân viên Lễ Tân
    /// </summary>
    [NhanVienAuthorize]
    public class KhachHangController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        private bool CheckRole()
        {
            string vaiTro = Session["VaiTro"]?.ToString();
            return vaiTro == "LeTan" || vaiTro == "Lễ Tân" || vaiTro == "Admin" || vaiTro == "Quản lý";
        }

        // GET: Index
        public ActionResult Index(KhachHangFilterViewModel filter)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                var viewModel = new KhachHangListViewModel();
                viewModel.Filter = filter ?? new KhachHangFilterViewModel();

                var query = db.KhachHangs.AsQueryable();

                // ===== FILTERS =====

                // Tìm kiếm
                if (!string.IsNullOrWhiteSpace(filter.TimKiem))
                {
                    string searchTerm = filter.TimKiem.Trim().ToLower();
                    query = query.Where(kh =>
                        kh.HoVaTen.ToLower().Contains(searchTerm) ||
                        kh.SoDienThoai.Contains(searchTerm) ||
                        (kh.Email != null && kh.Email.ToLower().Contains(searchTerm)) ||
                        (kh.TenDangNhap != null && kh.TenDangNhap.ToLower().Contains(searchTerm)));
                }

                // Lọc theo giới tính
                if (filter.GioiTinh.HasValue)
                {
                    query = query.Where(kh => kh.GioiTinh == filter.GioiTinh.Value);
                }

                // Lọc theo thời gian tạo
                if (filter.TuNgayTao.HasValue)
                {
                    query = query.Where(kh => kh.NgayTao >= filter.TuNgayTao.Value);
                }

                if (filter.DenNgayTao.HasValue)
                {
                    query = query.Where(kh => kh.NgayTao <= filter.DenNgayTao.Value.AddDays(1));
                }

                // Lọc theo có tài khoản
                if (filter.CoTaiKhoan.HasValue)
                {
                    if (filter.CoTaiKhoan.Value)
                    {
                        query = query.Where(kh => !string.IsNullOrEmpty(kh.TenDangNhap));
                    }
                    else
                    {
                        query = query.Where(kh => string.IsNullOrEmpty(kh.TenDangNhap));
                    }
                }

                // Lọc theo khách hàng VIP (tổng tiền >= 10 triệu)
                if (filter.LaKhachHangVIP.HasValue && filter.LaKhachHangVIP.Value)
                {
                    query = query.Where(kh =>
                        kh.HoaDons.Any(hd => hd.TrangThaiHoaDon == 1) &&
                        kh.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1).Sum(hd => (decimal?)hd.TongTien) >= 10000000);
                }

                // ===== STATISTICS =====
                viewModel.ThongKe = new KhachHangStatViewModel
                {
                    TongSoKhachHang = db.KhachHangs.Count(),
                    SoKhachHangCoTaiKhoan = db.KhachHangs.Count(kh => !string.IsNullOrEmpty(kh.TenDangNhap)),
                    SoKhachHangNam = db.KhachHangs.Count(kh => kh.GioiTinh == 0),
                    SoKhachHangNu = db.KhachHangs.Count(kh => kh.GioiTinh == 1),
                    TongDoanhThu = db.HoaDons
                        .Where(hd => hd.TrangThaiHoaDon == 1)
                        .Sum(hd => (decimal?)hd.TongTien) ?? 0,
                    SoKhachHangMoiThangNay = db.KhachHangs.Count(kh =>
                        kh.NgayTao.Year == DateTime.Now.Year &&
                        kh.NgayTao.Month == DateTime.Now.Month)
                };

                // Tính số khách hàng VIP
                viewModel.ThongKe.SoKhachHangVIP = db.KhachHangs.Count(kh =>
                    kh.HoaDons.Any(hd => hd.TrangThaiHoaDon == 1) &&
                    kh.HoaDons.Where(hd => hd.TrangThaiHoaDon == 1).Sum(hd => (decimal?)hd.TongTien) >= 10000000);

                // ===== PAGINATION =====
                int totalRecords = query.Count();
                int pageSize = filter.PageSize > 0 ? filter.PageSize : 10;
                int currentPage = filter.CurrentPage > 0 ? filter.CurrentPage : 1;
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                viewModel.TotalRecords = totalRecords;
                viewModel.PageSize = pageSize;
                viewModel.CurrentPage = currentPage;
                viewModel.TotalPages = totalPages;

                // ===== GET DATA =====
                var khachHangs = query
                    .Include(kh => kh.DatPhongs)
                    .Include(kh => kh.HoaDons)
                    .Include(kh => kh.DanhGias)
                    .OrderByDescending(kh => kh.NgayTao)
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                viewModel.DanhSachKhachHang = khachHangs.Select(kh => MapToKhachHangItemViewModel(kh)).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Index] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách khách hàng!";
                return View(new KhachHangListViewModel());
            }
        }

        // GET: Details
        public ActionResult Details(int? id)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                if (id == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                var khachHang = db.KhachHangs
                    .Include(kh => kh.DatPhongs)
                    .Include(kh => kh.HoaDons)
                    .Include(kh => kh.DanhGias)
                    .FirstOrDefault(kh => kh.MaKhachHang == id.Value);

                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                var viewModel = MapToKhachHangItemViewModel(khachHang);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Details] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết khách hàng!";
                return RedirectToAction("Index");
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

                var viewModel = new KhachHangFormViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Create GET] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form tạo khách hàng!";
                return RedirectToAction("Index");
            }
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhachHangFormViewModel model)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                // ===== VALIDATION LOGIC =====
                
                // Nếu có tên đăng nhập thì bắt buộc phải có Email và Mật khẩu
                if (!string.IsNullOrWhiteSpace(model.TenDangNhap))
                {
                    if (string.IsNullOrWhiteSpace(model.Email))
                    {
                        ModelState.AddModelError("Email", "Vui lòng nhập Email! (Bắt buộc khi tạo tài khoản)");
                    }
                    if (string.IsNullOrWhiteSpace(model.MatKhau))
                    {
                        ModelState.AddModelError("MatKhau", "Vui lòng nhập Mật khẩu! (Bắt buộc khi tạo tài khoản)");
                    }
                    if (string.IsNullOrWhiteSpace(model.XacNhanMatKhau))
                    {
                        ModelState.AddModelError("XacNhanMatKhau", "Vui lòng xác nhận Mật khẩu! (Bắt buộc khi tạo tài khoản)");
                    }
                    if (!string.IsNullOrWhiteSpace(model.MatKhau) && model.MatKhau.Length < 6)
                    {
                        ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự!");
                    }
                    if (!string.IsNullOrWhiteSpace(model.MatKhau) && !string.IsNullOrWhiteSpace(model.XacNhanMatKhau) && model.MatKhau != model.XacNhanMatKhau)
                    {
                        ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu xác nhận không khớp!");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                if (db.KhachHangs.Any(kh => kh.SoDienThoai == model.SoDienThoai))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng! Vui lòng chọn số điện thoại khác.");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa (nếu có)
                if (!string.IsNullOrWhiteSpace(model.Email) && db.KhachHangs.Any(kh => kh.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng! Vui lòng chọn email khác.");
                    return View(model);
                }

                // Kiểm tra tên đăng nhập đã tồn tại chưa (nếu có)
                if (!string.IsNullOrWhiteSpace(model.TenDangNhap) && db.KhachHangs.Any(kh => kh.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng! Vui lòng chọn tên đăng nhập khác.");
                    return View(model);
                }

                var khachHang = new KhachHang
                {
                    HoVaTen = model.HoVaTen,
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    DiaChi = model.DiaChi,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = model.NgaySinh,
                    TenDangNhap = model.TenDangNhap,
                    AnhDaiDienUrl = model.AnhDaiDienUrl,
                    DaDongYDieuKhoan = true,
                    NgayTao = DateTime.Now
                };

                // Nếu có mật khẩu, hash nó
                if (!string.IsNullOrWhiteSpace(model.MatKhau))
                {
                    khachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                }

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Tạo khách hàng {khachHang.HoVaTen} thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Create POST] {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACKTRACE] {ex.StackTrace}");
                // Chỉ set TempData nếu không có validation errors
                if (ViewData.ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo khách hàng!";
                }
                else
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo khách hàng! Vui lòng kiểm tra lại thông tin.");
                }
                return View(model);
            }
        }

        // GET: Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!CheckRole())
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập!";
                    return RedirectToAction("DangNhap", "DangNhapNV", new { area = "DangNhapNV" });
                }

                if (id == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                var khachHang = db.KhachHangs.Find(id.Value);
                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                var viewModel = new KhachHangFormViewModel
                {
                    MaKhachHang = khachHang.MaKhachHang,
                    HoVaTen = khachHang.HoVaTen,
                    SoDienThoai = khachHang.SoDienThoai,
                    Email = khachHang.Email,
                    DiaChi = khachHang.DiaChi,
                    GioiTinh = khachHang.GioiTinh,
                    NgaySinh = khachHang.NgaySinh,
                    TenDangNhap = khachHang.TenDangNhap,
                    AnhDaiDienUrl = khachHang.AnhDaiDienUrl
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Edit GET] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải form sửa khách hàng!";
                return RedirectToAction("Index");
            }
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KhachHangFormViewModel model)
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

                if (!model.MaKhachHang.HasValue)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                var khachHang = db.KhachHangs.Find(model.MaKhachHang.Value);
                if (khachHang == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khách hàng!";
                    return RedirectToAction("Index");
                }

                // Kiểm tra số điện thoại đã tồn tại chưa (trừ chính nó)
                if (db.KhachHangs.Any(kh => kh.SoDienThoai == model.SoDienThoai && kh.MaKhachHang != model.MaKhachHang.Value))
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng!");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại chưa (nếu có, trừ chính nó)
                if (!string.IsNullOrWhiteSpace(model.Email) && 
                    db.KhachHangs.Any(kh => kh.Email == model.Email && kh.MaKhachHang != model.MaKhachHang.Value))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                    return View(model);
                }

                // Kiểm tra tên đăng nhập đã tồn tại chưa (nếu có, trừ chính nó)
                if (!string.IsNullOrWhiteSpace(model.TenDangNhap) && 
                    db.KhachHangs.Any(kh => kh.TenDangNhap == model.TenDangNhap && kh.MaKhachHang != model.MaKhachHang.Value))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng!");
                    return View(model);
                }

                // Cập nhật thông tin
                khachHang.HoVaTen = model.HoVaTen;
                khachHang.SoDienThoai = model.SoDienThoai;
                khachHang.Email = model.Email;
                khachHang.DiaChi = model.DiaChi;
                khachHang.GioiTinh = model.GioiTinh;
                khachHang.NgaySinh = model.NgaySinh;
                khachHang.TenDangNhap = model.TenDangNhap;
                khachHang.AnhDaiDienUrl = model.AnhDaiDienUrl;
                khachHang.NgayCapNhat = DateTime.Now;

                // Nếu có mật khẩu mới, cập nhật nó
                if (!string.IsNullOrWhiteSpace(model.MatKhau))
                {
                    // Validate mật khẩu
                    if (model.MatKhau.Length < 6)
                    {
                        ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự!");
                        return View(model);
                    }
                    if (model.MatKhau != model.XacNhanMatKhau)
                    {
                        ModelState.AddModelError("XacNhanMatKhau", "Mật khẩu xác nhận không khớp!");
                        return View(model);
                    }
                    khachHang.MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
                }

                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Cập nhật thông tin khách hàng {khachHang.HoVaTen} thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR - KhachHang/Edit POST] {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật khách hàng!";
                return View(model);
            }
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Map từ KhachHang entity sang KhachHangItemViewModel
        /// </summary>
        private KhachHangItemViewModel MapToKhachHangItemViewModel(KhachHang khachHang)
        {
            // Tính tổng tiền đã thanh toán
            decimal? tongTienDaThanhToan = khachHang.HoaDons
                .Where(hd => hd.TrangThaiHoaDon == 1)
                .Sum(hd => (decimal?)hd.TongTien);

            return new KhachHangItemViewModel
            {
                MaKhachHang = khachHang.MaKhachHang,
                HoVaTen = khachHang.HoVaTen,
                SoDienThoai = khachHang.SoDienThoai,
                Email = khachHang.Email,
                DiaChi = khachHang.DiaChi,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                NgayTao = khachHang.NgayTao,
                NgayCapNhat = khachHang.NgayCapNhat,
                AnhDaiDienUrl = khachHang.AnhDaiDienUrl,
                TenDangNhap = khachHang.TenDangNhap,
                TongSoDonDatPhong = khachHang.DatPhongs?.Count ?? 0,
                TongSoHoaDon = khachHang.HoaDons?.Count ?? 0,
                TongTienDaThanhToan = tongTienDaThanhToan,
                SoLanDanhGia = khachHang.DanhGias?.Count ?? 0
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

