using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Models
{
    public class ThongTinDatPhongViewModel
    {
        // Thông tin phòng
        public int? PhongId { get; set; }
        public string TenPhong { get; set; }
        public string LoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string HinhAnh { get; set; }

        // Thông tin khách hàng
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và Tên")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        // Thông tin đặt phòng
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayNhan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
        [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayTra { get; set; }

        [Display(Name = "Số người")]
        public int SoNguoi { get; set; }

        [Display(Name = "Mã khuyến mãi")]
        public string MaKhuyenMai { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string GhiChu { get; set; }

        // Dịch vụ đã chọn
        public List<DichVuDaChon> DichVuDaChon { get; set; }

        public decimal TongTienDichVu
        {
            get
            {
                if (DichVuDaChon == null || DichVuDaChon.Count == 0)
                    return 0;
                
                decimal tong = 0;
                foreach (var dv in DichVuDaChon)
                {
                    tong += dv.Gia;
                }
                return tong;
            }
        }

        // Các trường tính toán
        public int SoDem
        {
            get
            {
                if (NgayNhan != null && NgayTra != null && NgayTra > NgayNhan)
                {
                    return (NgayTra - NgayNhan).Days;
                }
                return 0;
            }
        }

        public decimal TongTienPhong
        {
            get
            {
                return GiaPhong * SoDem;
            }
        }

        public decimal TongCong
        {
            get
            {
                return TongTienPhong + TongTienDichVu;
            }
        }

        // Validation tùy chỉnh
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Kiểm tra ngày nhận không được trước hôm nay
            if (NgayNhan.Date < DateTime.Now.Date)
            {
                errorMessage = "Ngày nhận phòng không được trước ngày hiện tại!";
                return false;
            }

            // Kiểm tra ngày trả phải sau ngày nhận
            if (NgayTra <= NgayNhan)
            {
                errorMessage = "Ngày trả phòng phải sau ngày nhận phòng ít nhất 1 ngày!";
                return false;
            }

            return true;
        }

        public ThongTinDatPhongViewModel()
        {
            DichVuDaChon = new List<DichVuDaChon>();
        }
    }

    // Class để lưu thông tin dịch vụ đã chọn
    public class DichVuDaChon
    {
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public decimal Gia { get; set; }
    }
}
