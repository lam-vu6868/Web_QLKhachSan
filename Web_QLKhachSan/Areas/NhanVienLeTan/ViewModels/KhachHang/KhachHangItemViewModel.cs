using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhachHang
{
    /// <summary>
    /// ViewModel đại diện cho 1 khách hàng trong danh sách
    /// </summary>
    public class KhachHangItemViewModel
    {
        // ===== THÔNG TIN CƠ BẢN =====
        
        public int MaKhachHang { get; set; }

        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Giới tính")]
        public byte? GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string AnhDaiDienUrl { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        // ===== THỐNG KÊ =====

        [Display(Name = "Tổng số đơn đặt phòng")]
        public int TongSoDonDatPhong { get; set; }

        [Display(Name = "Tổng số hóa đơn")]
        public int TongSoHoaDon { get; set; }

        [Display(Name = "Tổng tiền đã thanh toán")]
        [DataType(DataType.Currency)]
        public decimal? TongTienDaThanhToan { get; set; }

        [Display(Name = "Số lần đánh giá")]
        public int SoLanDanhGia { get; set; }

        // ===== COMPUTED PROPERTIES =====

        /// <summary>
        /// Hiển thị giới tính dạng text
        /// </summary>
        public string GioiTinhText
        {
            get
            {
                if (!GioiTinh.HasValue) return "Không xác định";
                switch (GioiTinh.Value)
                {
                    case 0: return "Nam";
                    case 1: return "Nữ";
                    case 2: return "Khác";
                    default: return "Không xác định";
                }
            }
        }

        /// <summary>
        /// Tuổi hiện tại
        /// </summary>
        public int? Tuoi
        {
            get
            {
                if (!NgaySinh.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - NgaySinh.Value.Year;
                if (NgaySinh.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Có ảnh đại diện không?
        /// </summary>
        public bool CoAnhDaiDien
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AnhDaiDienUrl);
            }
        }

        /// <summary>
        /// Có tài khoản không? (có TenDangNhap)
        /// </summary>
        public bool CoTaiKhoan
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TenDangNhap);
            }
        }

        /// <summary>
        /// Là khách hàng VIP? (tổng tiền > 10 triệu)
        /// </summary>
        public bool LaKhachHangVIP
        {
            get
            {
                return TongTienDaThanhToan.HasValue && TongTienDaThanhToan.Value >= 10000000;
            }
        }
    }
}

