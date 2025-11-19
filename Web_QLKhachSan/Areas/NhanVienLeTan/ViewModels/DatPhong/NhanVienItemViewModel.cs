using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho thông tin nhân viên trong ??n ??t phòng
    /// </summary>
    public class NhanVienItemViewModel
    {
        public int NhanVienId { get; set; }

        [Display(Name = "H? và tên")]
        public string HoVaTen { get; set; }

        [Display(Name = "S? ?i?n tho?i")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
      public string Email { get; set; }

[Display(Name = "Ch?c v?")]
 public string ChucVu { get; set; }
    }
}
