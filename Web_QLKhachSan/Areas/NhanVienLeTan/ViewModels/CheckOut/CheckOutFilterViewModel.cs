using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckOut
{
    /// <summary>
    /// ViewModel cho bộ lọc trang Check-out
    /// </summary>
    public class CheckOutFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; }

        [Display(Name = "Ngày check-out")]
      [DataType(DataType.Date)]
      public DateTime? NgayCheckOut { get; set; }

        [Display(Name = "Trang hiện tại")]
        public int CurrentPage { get; set; } = 1;

        [Display(Name = "Số bản ghi trên trang")]
 public int PageSize { get; set; } = 10;
    }
}
