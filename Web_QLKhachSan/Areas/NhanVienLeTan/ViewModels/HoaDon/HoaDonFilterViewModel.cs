using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho bộ lọc hóa đơn
    /// </summary>
  public class HoaDonFilterViewModel
  {
        [Display(Name = "Tìm kiếm")]
    public string TimKiem { get; set; }

   [Display(Name = "Trạng thái hóa đơn")]
     public byte? TrangThaiHoaDon { get; set; }

        [Display(Name = "Phương thức thanh toán")]
      public string PhuongThucThanhToan { get; set; }

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
    public DateTime? TuNgay { get; set; }

 [Display(Name = "Đến ngày")]
   [DataType(DataType.Date)]
    public DateTime? DenNgay { get; set; }

     [Display(Name = "Từ số tiền")]
   [Range(0, 999999999)]
    public decimal? TuSoTien { get; set; }

 [Display(Name = "Đến số tiền")]
  [Range(0, 999999999)]
        public decimal? DenSoTien { get; set; }

        public int CurrentPage { get; set; } = 1;
  public int PageSize { get; set; } = 10;
    }
}
