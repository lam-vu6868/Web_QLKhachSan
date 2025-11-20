using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu
{
    /// <summary>
    /// ViewModel cho bộ lọc dịch vụ
    /// </summary>
    public class DichVuFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; }

        [Display(Name = "Loại dịch vụ")]
        public int? LoaiDichVuId { get; set; }

     [Display(Name = "Trạng thái")]
        public bool? DaHoatDong { get; set; }

   [Display(Name = "Từ giá")]
        public decimal? TuGia { get; set; }

  [Display(Name = "Đến giá")]
        public decimal? DenGia { get; set; }
    }
}
