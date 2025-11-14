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

            // Tạo ViewModel
            var model = new ThongTinDatPhongViewModel
            {
                HoVaTen = khachHang.HoVaTen,
                Email = khachHang.Email,
                SoDienThoai = khachHang.SoDienThoai,
                NgayNhan = DateTime.Now.AddDays(1), // Mặc định ngày mai
                NgayTra = DateTime.Now.AddDays(2)   // Mặc định 1 đêm
            };

            // Lấy thông tin phòng nếu có phongId
            if (phongId.HasValue)
            {
                var phong = db.Phongs
                    .Include(p => p.LoaiPhong)
                    .Include(p => p.PhongAnhs)
                    .FirstOrDefault(p => p.PhongId == phongId.Value && p.DaHoatDong);
                
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
                model.SoNguoi = model.SoNguoiToiDa;
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
            Session["ThongTinDatPhong"] = model;

            // Chuyển sang trang dịch vụ đặt thêm
            return RedirectToAction("DichVuDatThem");
        }

        // GET: DatPhong/DichVuDatThem
        public ActionResult DichVuDatThem(int? phongId)
        {
            // Nếu chưa có thông tin đặt phòng trong Session, nhưng có phongId (được truyền qua link),
            // tạo ThongTinDatPhongViewModel từ DB để tiếp tục flow mà không cần POST.
            if (Session["ThongTinDatPhong"] == null)
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
                        HinhAnh = phong.HinhAnhThumb ?? (phong.PhongAnhs.Any() ? phong.PhongAnhs.FirstOrDefault().Url : "")
                    };

                    // Lưu vào Session để các bước sau dùng
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

        public ActionResult ThanhToan()
        {
            return View();
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