using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
  /// <summary>
/// ViewModel cho tạo/sửa khuyến mãi
 /// </summary>
    public class KhuyenMaiFormViewModel
    {
        public KhuyenMaiFormViewModel()
{
   DaHoatDong = true;
   NgayBatDau = DateTime.Now.Date;
      NgayKetThuc = DateTime.Now.AddDays(30).Date;
   }

        public int? KhuyenMaiId { get; set; }

   [Display(Name = "Mã khuyến mãi")]
  [StringLength(50, ErrorMessage = "Mã khuyến mãi không được vượt quá 50 ký tự")]
    public string MaKhuyenMai { get; set; }

   [Required(ErrorMessage = "Vui lòng nhập tên khuyến mãi")]
        [Display(Name = "Tên khuyến mãi")]
        [StringLength(200, ErrorMessage = "Tên khuyến mãi không được vượt quá 200 ký tự")]
   public string TenKhuyenMai { get; set; }

     [Required(ErrorMessage = "Vui lòng nhập giá trị khuyến mãi")]
        [Display(Name = "Giá trị giảm (%)")]
   [Range(1, 100, ErrorMessage = "Giá trị giảm phải từ 1% đến 100%")]
        public decimal GiaTri { get; set; }

 [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
    [Display(Name = "Ngày bắt đầu")]
   [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
     [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
     public DateTime NgayKetThuc { get; set; }

 [Display(Name = "Điều kiện áp dụng")]
  [DataType(DataType.MultilineText)]
     [StringLength(500, ErrorMessage = "Điều kiện áp dụng không được vượt quá 500 ký tự")]
      public string DieuKienApDung { get; set; }

     [Display(Name = "Số lần sử dụng tối đa")]
        [Range(0, 999999, ErrorMessage = "Số lần sử dụng tối đa phải từ 0 đến 999,999")]
        public int? SoLanSuDungToiDa { get; set; }

   [Display(Name = "Đang hoạt động")]
 public bool DaHoatDong { get; set; }

    // Validation
        public bool IsNgayHopLe()
        {
 return NgayKetThuc >= NgayBatDau;
    }

 public bool IsGiaTriHopLe()
        {
     return GiaTri > 0 && GiaTri <= 100;
 }
    }
}
