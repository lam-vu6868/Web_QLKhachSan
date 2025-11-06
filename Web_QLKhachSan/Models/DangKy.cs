using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Models
{
    public class DangKy
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Họ và tên phải từ 6 đến 100 ký tự")]
        [RegularExpression(@"^[a-zA-ZàáảãạăằắẳẵặâầấẨẫậèéẻẽẹêềếểễệìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳýỷỹỵđÀÁẢÃẠĂẰẮẲẴẶÂẦẤẨẪẬÈÉẺẼẸÊỀẾỂỄỆÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỮỢÙÚỦŨỤƯỪỨỬỮỰỲÝỶỸỴĐ\s]+$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        [CustomValidation(typeof(DangKy), nameof(ValidateNgaySinh))]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public byte? GioiTinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string XacNhanMatKhau { get; set; }

        [CustomValidation(typeof(DangKy), nameof(ValidateDieuKhoan))]
        [Display(Name = "Đồng ý điều khoản")]
        public bool DaDongYDieuKhoan { get; set; }

        // Custom Validation cho Ngày sinh
        public static ValidationResult ValidateNgaySinh(DateTime? ngaySinh, ValidationContext context)
        {
            if (ngaySinh == null)
            {
                return new ValidationResult("Vui lòng chọn ngày sinh");
            }

          var today = DateTime.Today;
            var age = today.Year - ngaySinh.Value.Year;

         // Kiểm tra nếu chưa đến sinh nhật năm nay
            if (ngaySinh.Value.Date > today.AddYears(-age))
            {
                 age--;
            }

            if (age < 18)
            {
                return new ValidationResult("Bạn phải đủ 18 tuổi để đăng ký");
            }

            if (age > 100)
            {
            return new ValidationResult("Ngày sinh không hợp lệ");
            }

            return ValidationResult.Success;
        }

   // Custom Validation cho checkbox đồng ý điều khoản
        public static ValidationResult ValidateDieuKhoan(bool daDongY, ValidationContext context)
        {
            if (!daDongY)
            {
                return new ValidationResult("Bạn phải đồng ý với điều khoản và chính sách");
            }

            return ValidationResult.Success;
        }
    }
}
