using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
    /// <summary>
    /// ViewModel cho filter/tìm kiếm khuyến mãi
    /// </summary>
    public class KhuyenMaiFilterViewModel
    {
        public KhuyenMaiFilterViewModel()
        {
        PageSize = 10;
            CurrentPage = 1;
        }

  [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; }

      [Display(Name = "Trạng thái")]
        public bool? DaHoatDong { get; set; }

     [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
     [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }

        [Display(Name = "Hiệu lực")]
        public string HieuLuc { get; set; } // "all", "active", "upcoming", "expired"

        public int PageSize { get; set; }
  public int CurrentPage { get; set; }
    }
}
