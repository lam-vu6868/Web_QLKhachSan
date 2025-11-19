sing System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Models
{
    /// <summary>
    /// ViewModel cho chức năng cập nhật thông tin cá nhân
    /// </summary>
    public class CapNhatThongTinViewModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên không được quá 100 ký tự")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái")]
        public string HoVaTen { get; set; }

        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
        public string SoDienThoai { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được quá 500 ký tự")]
        public string DiaChi { get; set; }
    }
}
