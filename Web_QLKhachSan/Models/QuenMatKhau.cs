using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Models
{
    public class QuenMatKhau
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
