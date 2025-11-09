using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.DangNhapNV.ViewModels
{
    /// <summary>
    /// ViewModel cho ??t l?i m?t kh?u nhân viên
    /// </summary>
    public class DatLaiMatKhauNVViewModel
  {
        [Required(ErrorMessage = "Vui lòng nh?p m?t kh?u m?i")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i t? 6-100 ký t?")]
    [DataType(DataType.Password)]
 [Display(Name = "M?t kh?u m?i")]
        public string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nh?n m?t kh?u")]
  [DataType(DataType.Password)]
        [Display(Name = "Xác nh?n m?t kh?u")]
        [Compare("MatKhauMoi", ErrorMessage = "M?t kh?u xác nh?n không kh?p")]
        public string XacNhanMatKhau { get; set; }

        public string Email { get; set; }
    }
}
