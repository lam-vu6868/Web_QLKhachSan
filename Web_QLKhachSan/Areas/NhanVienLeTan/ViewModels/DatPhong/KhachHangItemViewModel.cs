using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho thông tin khách hàng trong ??n ??t phòng
    /// </summary>
    public class KhachHangItemViewModel
    {
  public int MaKhachHang { get; set; }

      [Display(Name = "H? và tên")]
   public string HoVaTen { get; set; }

        [Display(Name = "S? ?i?n tho?i")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "??a ch?")]
        public string DiaChi { get; set; }

    [Display(Name = "Gi?i tính")]
        public byte? GioiTinh { get; set; }

        /// <summary>
        /// Hi?n th? gi?i tính d?ng text
    /// </summary>
      public string GioiTinhText
        {
            get
    {
        if (!GioiTinh.HasValue) return "Không xác ??nh";
    switch (GioiTinh.Value)
{
        case 0: return "Nam";
          case 1: return "N?";
              case 2: return "Khác";
        default: return "Không xác ??nh";
       }
            }
        }
    }
}
