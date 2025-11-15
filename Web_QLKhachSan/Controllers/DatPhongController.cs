using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class DatPhongController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: DatPhong/ThongTinKhachHang
        public ActionResult ThongTinKhachHang(int? phongId)
        {
            // Kiểm tra đăng nhập
            if (Session["MaKhachHang"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("DangNhap", "Login");
            }

            // Lấy thông tin khách hàng từ session
            int maKhachHang = Convert.ToInt32(Session["MaKhachHang"]);
            var khachHang = db.KhachHangs.Find(maKhachHang);

            if (khachHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra nếu đã có thông tin đặt phòng trong Session
            var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            ThongTinDatPhongViewModel model;

            // Nếu có phongId trong URL và khác với session hiện tại, tức là đặt phòng mới -> RESET session
            bool isNewBooking = phongId.HasValue && (existingModel == null || phongId.Value != existingModel.PhongId);

            if (existingModel != null && !isNewBooking)
            {
                // Sử dụng model đã có trong session để giữ nguyên thông tin đã nhập (cùng phòng)
                model = existingModel;
                
                // Chỉ cập nhật thông tin khách hàng nếu cần thiết
                if (model.HoVaTen != khachHang.HoVaTen || 
                    model.Email != khachHang.Email || 
                    model.SoDienThoai != khachHang.SoDienThoai)
                {
                    model.HoVaTen = khachHang.HoVaTen;
                    model.Email = khachHang.Email;
                    model.SoDienThoai = khachHang.SoDienThoai;
                }
                
                // Đảm bảo DichVuDaChon không bị null
                if (model.DichVuDaChon == null)
                {
                    model.DichVuDaChon = new List<DichVuDaChon>();
                }
            }
            else
            {
                // Tạo ViewModel mới (đặt phòng mới hoặc chưa có session)
                model = new ThongTinDatPhongViewModel
                {
                    HoVaTen = khachHang.HoVaTen,
                    Email = khachHang.Email,
                    SoDienThoai = khachHang.SoDienThoai,
                    NgayNhan = DateTime.Now.AddDays(1), // Mặc định ngày mai
                    NgayTra = DateTime.Now.AddDays(2),   // Mặc định 1 đêm
                    DichVuDaChon = new List<DichVuDaChon>() // Reset dịch vụ cho đặt phòng mới
                };
            }

            // Lấy thông tin phòng nếu có phongId (cho cả model mới và model đã có khi đặt phòng mới)
            if (phongId.HasValue || model.PhongId.HasValue)
            {
                var roomId = phongId ?? model.PhongId;
                var phong = db.Phongs
                    .Include(p => p.LoaiPhong)
                    .Include(p => p.PhongAnhs)
                    .FirstOrDefault(p => p.PhongId == roomId.Value && p.DaHoatDong);
                
                if (phong == null)
                {
                    TempData["ErrorMessage"] = "Phòng không tồn tại hoặc không khả dụng!";
                    return RedirectToAction("Index", "PhongNghi");
                }

                // Gán thông tin phòng vào model
                model.PhongId = phong.PhongId;
                model.TenPhong = phong.TenPhong;
                model.LoaiPhong = phong.LoaiPhong.TenLoai;
                model.GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0;
                model.SoNguoiToiDa = phong.LoaiPhong.SoNguoiToiDa ?? 2;
                if (model.SoNguoi == 0) // Chỉ set default nếu chưa có giá trị
                {
                    model.SoNguoi = model.SoNguoiToiDa;
                }
                model.HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : "");
            }

            return View(model);
        }

        // POST: DatPhong/ThongTinKhachHang
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThongTinKhachHang(ThongTinDatPhongViewModel model)
        {
            // Kiểm tra đăng nhập
            if (Session["MaKhachHang"] == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("DangNhap", "Login");
            }

            // CRITICAL: Preserve services from existing session
            var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            if (existingModel != null && existingModel.DichVuDaChon != null)
            {
                // Preserve services từ session
                model.DichVuDaChon = existingModel.DichVuDaChon;
            }
            else
            {
                // Khởi tạo nếu chưa có
                model.DichVuDaChon = new List<DichVuDaChon>();
            }

            // Validation tùy chỉnh
            string errorMessage;
            if (!model.IsValid(out errorMessage))
            {
                ModelState.AddModelError("", errorMessage);
            }

            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, load lại thông tin phòng
                if (model.PhongId.HasValue)
                {
                    var phong = db.Phongs
                        .Include(p => p.LoaiPhong)
                        .Include(p => p.PhongAnhs)
                        .FirstOrDefault(p => p.PhongId == model.PhongId.Value);
                    
                    if (phong != null)
                    {
                        model.TenPhong = phong.TenPhong;
                        model.LoaiPhong = phong.LoaiPhong.TenLoai;
                        model.GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0;
                        model.SoNguoiToiDa = phong.LoaiPhong.SoNguoiToiDa ?? 2;
                        model.HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : "");
                    }
                }
                return View(model);
            }

            // Lưu thông tin vào Session để dùng cho bước tiếp theo
            // Đảm bảo DichVuDaChon được khởi tạo
            if (model.DichVuDaChon == null)
            {
                model.DichVuDaChon = new List<DichVuDaChon>();
            }
            Session["ThongTinDatPhong"] = model;

            // Chuyển sang trang dịch vụ đặt thêm
            return RedirectToAction("DichVuDatThem");
        }

        // GET: DatPhong/DichVuDatThem
        public ActionResult DichVuDatThem(int? phongId)
        {
            // Kiểm tra nếu chưa có thông tin đặt phòng trong Session
            // hoặc nếu có phongId và khác với session (đặt phòng mới)
            var existingModel = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            bool isNewBooking = phongId.HasValue && (existingModel == null || phongId.Value != existingModel.PhongId);
            
            if (existingModel == null || isNewBooking)
            {
                if (phongId.HasValue)
                {
                    // Kiểm tra đăng nhập
                    if (Session["MaKhachHang"] == null)
                    {
                        TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                        return RedirectToAction("DangNhap", "Login");
                    }

                    int maKhachHang = Convert.ToInt32(Session["MaKhachHang"]);
                    var khachHang = db.KhachHangs.Find(maKhachHang);
                    if (khachHang == null)
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng!";
                        return RedirectToAction("Index", "Home");
                    }

                    var phong = db.Phongs
                        .Include(p => p.LoaiPhong)
                        .Include(p => p.PhongAnhs)
                        .FirstOrDefault(p => p.PhongId == phongId.Value && p.DaHoatDong);

                    if (phong == null)
                    {
                        TempData["ErrorMessage"] = "Phòng không tồn tại hoặc không khả dụng!";
                        return RedirectToAction("Index", "PhongNghi");
                    }

                    var autoModel = new ThongTinDatPhongViewModel
                    {
                        HoVaTen = khachHang.HoVaTen,
                        Email = khachHang.Email,
                        SoDienThoai = khachHang.SoDienThoai,
                        NgayNhan = DateTime.Now.AddDays(1),
                        NgayTra = DateTime.Now.AddDays(2),
                        PhongId = phong.PhongId,
                        TenPhong = phong.TenPhong,
                        LoaiPhong = phong.LoaiPhong?.TenLoai,
                        GiaPhong = phong.Gia ?? phong.LoaiPhong.GiaCoBan ?? 0,
                        SoNguoiToiDa = phong.LoaiPhong?.SoNguoiToiDa ?? 2,
                        SoNguoi = phong.LoaiPhong?.SoNguoiToiDa ?? 2,
                        HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : ""),
                        DichVuDaChon = new List<DichVuDaChon>() // Khởi tạo danh sách trống cho đặt phòng mới
                    };

                    // Lưu vào Session để các bước sau dùng (hoặc thay thế session cũ)
                    Session["ThongTinDatPhong"] = autoModel;
                }
                else
                {
                    TempData["ErrorMessage"] = "Vui lòng nhập thông tin khách hàng trước!";
                    return RedirectToAction("ThongTinKhachHang");
                }
            }

            // Lấy thông tin đặt phòng từ Session (sẵn có hoặc mới tạo)
            var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            
            // Đảm bảo DichVuDaChon không null
            if (thongTinDatPhong.DichVuDaChon == null)
            {
                thongTinDatPhong.DichVuDaChon = new List<DichVuDaChon>();
            }

            // Load danh sách loại dịch vụ và dịch vụ từ database
            var loaiDichVus = db.LoaiDichVus
                .Where(ldv => ldv.DichVus.Any(dv => dv.DaHoatDong)) // Chỉ lấy loại có dịch vụ đang hoạt động
                .OrderBy(ldv => ldv.LoaiDichVuId)
                .ToList();

            // Tạo ViewModel
            var viewModel = new DichVuDatThemViewModel
            {
                ThongTinDatPhong = thongTinDatPhong,
                DanhSachLoaiDichVu = loaiDichVus
            };

            return View(viewModel);
        }

        // POST: DatPhong/DichVuDatThem
        [HttpPost]
        public ActionResult DichVuDatThem(int[] services)
        {
            // Lấy thông tin đặt phòng từ Session
            var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            if (thongTinDatPhong == null)
            {
                TempData["ErrorMessage"] = "Thông tin đặt phòng không hợp lệ!";
                return RedirectToAction("ThongTinKhachHang");
            }

            // Clear dịch vụ cũ để tránh nhân đôi
            thongTinDatPhong.DichVuDaChon = new List<DichVuDaChon>();
            
            if (services != null && services.Length > 0)
            {
                // Lấy thông tin các dịch vụ đã chọn từ database
                var dichVuList = db.DichVus
                    .Where(dv => services.Contains(dv.DichVuId) && dv.DaHoatDong)
                    .ToList();

                foreach (var dichVu in dichVuList)
                {
                    thongTinDatPhong.DichVuDaChon.Add(new DichVuDaChon
                    {
                        DichVuId = dichVu.DichVuId,
                        TenDichVu = dichVu.TenDichVu,
                        Gia = dichVu.GiaUuDai ?? dichVu.Gia ?? 0
                    });
                }
            }

            // Cập nhật lại Session
            Session["ThongTinDatPhong"] = thongTinDatPhong;

            // Chuyển sang trang thanh toán
            return RedirectToAction("ThanhToan");
        }

        public ActionResult ThanhToan()
        {
            // Lấy thông tin đặt phòng từ Session
            var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            if (thongTinDatPhong == null)
            {
                TempData["ErrorMessage"] = "Thông tin đặt phòng không hợp lệ!";
                return RedirectToAction("ThongTinKhachHang");
            }

            // Thêm logic để kiểm tra số đêm cho tùy chọn gia hạn
            ViewBag.CanUseCustomDelay = thongTinDatPhong.SoDem >= 2;
            ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);

            return View(thongTinDatPhong);
        }

        // POST: DatPhong/ThanhToan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan(string payment, string delayTime, string customDate, string customTime)
        {
            // Lấy thông tin đặt phòng từ Session
            var thongTinDatPhong = Session["ThongTinDatPhong"] as ThongTinDatPhongViewModel;
            if (thongTinDatPhong == null)
            {
                TempData["ErrorMessage"] = "Thông tin đặt phòng không hợp lệ!";
                return RedirectToAction("ThongTinKhachHang");
            }

            // Xử lý payment method
            if (payment == "instant-transfer")
            {
                // Xử lý thanh toán ngay
                // TODO: Lưu thông tin thanh toán ngay vào database
                TempData["PaymentMethod"] = "Chuyển khoản ngay";
            }
            else if (payment == "delayed-transfer")
            {
                // Validate delay time selection
                if (string.IsNullOrEmpty(delayTime))
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn thời gian gia hạn!";
                    ViewBag.CanUseCustomDelay = thongTinDatPhong.SoDem >= 2;
                    ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);
                    return View(thongTinDatPhong);
                }

                DateTime? paymentDeadline = null;

                // Xử lý theo loại gia hạn
                switch (delayTime)
                {
                    case "3":
                        paymentDeadline = DateTime.Now.AddHours(3);
                        TempData["PaymentMethod"] = "Gia hạn 3 giờ";
                        break;
                    case "6":
                        paymentDeadline = DateTime.Now.AddHours(6);
                        TempData["PaymentMethod"] = "Gia hạn 6 giờ";
                        break;
                    case "24":
                        paymentDeadline = DateTime.Now.AddHours(24);
                        TempData["PaymentMethod"] = "Gia hạn 24 giờ";
                        break;
                    case "custom":
                        // Validate custom datetime for guests staying 2+ nights
                        if (thongTinDatPhong.SoDem >= 2)
                        {
                            if (string.IsNullOrEmpty(customDate) || string.IsNullOrEmpty(customTime))
                            {
                                TempData["ErrorMessage"] = "Vui lòng chọn ngày và giờ thanh toán!";
                                ViewBag.CanUseCustomDelay = true;
                                ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);
                                return View(thongTinDatPhong);
                            }

                            if (DateTime.TryParse($"{customDate} {customTime}", out DateTime customDateTime))
                            {
                                var maxDate = thongTinDatPhong.NgayTra.AddDays(-1);
                                
                                if (customDateTime <= DateTime.Now)
                                {
                                    TempData["ErrorMessage"] = "Thời gian thanh toán phải sau thời điểm hiện tại!";
                                    ViewBag.CanUseCustomDelay = true;
                                    ViewBag.MaxDelayDate = maxDate;
                                    return View(thongTinDatPhong);
                                }
                                
                                if (customDateTime >= thongTinDatPhong.NgayTra)
                                {
                                    TempData["ErrorMessage"] = "Thời gian thanh toán phải trước ngày trả phòng!";
                                    ViewBag.CanUseCustomDelay = true;
                                    ViewBag.MaxDelayDate = maxDate;
                                    return View(thongTinDatPhong);
                                }

                                paymentDeadline = customDateTime;
                                TempData["PaymentMethod"] = $"Gia hạn tùy chỉnh đến {customDateTime:dd/MM/yyyy HH:mm}";
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Định dạng ngày giờ không hợp lệ!";
                                ViewBag.CanUseCustomDelay = true;
                                ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);
                                return View(thongTinDatPhong);
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Tùy chọn gia hạn tùy chỉnh chỉ áp dụng cho đặt phòng từ 2 đêm trở lên!";
                            ViewBag.CanUseCustomDelay = false;
                            ViewBag.MaxDelayDate = thongTinDatPhong.NgayTra.AddDays(-1);
                            return View(thongTinDatPhong);
                        }
                        break;
                }

                // TODO: Lưu thông tin gia hạn vào database
                TempData["PaymentDeadline"] = paymentDeadline?.ToString("dd/MM/yyyy HH:mm");
            }

            return RedirectToAction("XacNhanHoaDon");
        }
        public ActionResult XacNhanHoaDon()
        {
            return View();
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