using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// Metadata class cho DatPhongCreateViewModel - Định nghĩa validation rules
    /// </summary>
    [MetadataType(typeof(DatPhongCreateViewModelMetadata))]
    public partial class DatPhongCreateViewModel
    {
        // Partial class này kết hợp với DatPhongCreateViewModel.cs
        // Để tách riêng validation logic
    }

    /// <summary>
    /// Metadata class chứa tất cả validation attributes
    /// </summary>
    public class DatPhongCreateViewModelMetadata
    {
        // ===== THÔNG TIN KHÁCH HÀNG =====

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", 
            ErrorMessage = "Số điện thoại không hợp lệ! Định dạng: 0xxxxxxxxx")]
        [StringLength(15, MinimumLength = 10, 
       ErrorMessage = "Số điện thoại phải từ 10-15 ký tự")]
      [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

     [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, MinimumLength = 2, 
    ErrorMessage = "Họ tên phải từ 2-100 ký tự")]
   [RegularExpression(@"^[\p{L}\s]+$", 
         ErrorMessage = "Họ tên chỉ được chứa chữ cái và khoảng trắng")]
        [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
   [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Range(0, 2, ErrorMessage = "Giới tính không hợp lệ")]
        [Display(Name = "Giới tính")]
public byte? GioiTinh { get; set; }

        // ===== THÔNG TIN ĐẶT PHÒNG =====

        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [DataType(DataType.Date)]
   [Display(Name = "Ngày nhận phòng")]
        public DateTime NgayNhan { get; set; }

      [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
 [DataType(DataType.Date)]
   [Display(Name = "Ngày trả phòng")]
        public DateTime NgayTra { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng khách")]
        [Range(1, 50, ErrorMessage = "Số lượng khách phải từ 1-50 người")]
        [Display(Name = "Số lượng khách")]
 public int SoLuongKhach { get; set; }

      [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

      // ===== THANH TOÁN =====

        [StringLength(20, ErrorMessage = "Mã khuyến mãi tối đa 20 ký tự")]
        [Display(Name = "Mã khuyến mãi")]
   public string MaKhuyenMai { get; set; }

  [Range(0, double.MaxValue, ErrorMessage = "Tiền đặt cọc không hợp lệ")]
 [DataType(DataType.Currency)]
        [Display(Name = "Tiền đặt cọc")]
        public decimal? TienDatCoc { get; set; }

        [Range(0, 2, ErrorMessage = "Hình thức thanh toán không hợp lệ")]
        [Display(Name = "Hình thức thanh toán")]
        public byte? HinhThucThanhToan { get; set; }
    }

    /// <summary>
    /// Custom Validation Attributes
 /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
          if (value is DateTime dateValue)
            {
 if (dateValue.Date < DateTime.Now.Date)
        {
      return new ValidationResult("Ngày nhận phòng phải từ hôm nay trở đi");
            }
            }
   return ValidationResult.Success;
        }
    }

    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateRangeAttribute(string startDatePropertyName)
    {
          _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
     var endDate = (DateTime)value;
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
     var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);

 if (endDate <= startDate)
    {
         return new ValidationResult("Ngày trả phòng phải sau ngày nhận phòng");
            }

        return ValidationResult.Success;
        }
    }
}
