using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.DangNhapNV.ViewModels
{
    /// <summary>
    /// ViewModel cho đăng nhập nhân viên
    /// </summary>
    public class DangNhapNVViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email")]
        [Display(Name = "Tên đăng nhập / Email")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
   public string MatKhau { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool GhiNho { get; set; }
    }
}
