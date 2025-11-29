using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.DangNhapNV.ViewModels
{
    /// <summary>
    /// ViewModel cho ??t l?i m?t kh?u nhân viên
    /// </summary>
    public class DatLaiMatKhauNVViewModel
  {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
    [DataType(DataType.Password)]
 [Display(Name = "Mật khẩu mới")]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
  [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; }

        public string Email { get; set; }
    }
}
