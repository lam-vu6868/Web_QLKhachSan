using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckOut
{
/// <summary>
  /// ViewModel chính cho trang danh sách Check-out
    /// </summary>
    public class CheckOutListViewModel
    {
   public CheckOutListViewModel()
        {
DanhSachCheckOut = new List<CheckOutItemViewModel>();
 Filter = new CheckOutFilterViewModel();
     ThongKe = new CheckOutStatViewModel();
     }

   // Danh sách check-out
        public List<CheckOutItemViewModel> DanhSachCheckOut { get; set; }

  // Bộ lọc
      public CheckOutFilterViewModel Filter { get; set; }

// Thống kê
   public CheckOutStatViewModel ThongKe { get; set; }

   // Pagination
        public int TotalRecords { get; set; }
      public int PageSize { get; set; }
     public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

  public bool HasPreviousPage
     {
    get { return CurrentPage > 1; }
        }

        public bool HasNextPage
        {
            get { return CurrentPage < TotalPages; }
        }
    }

    /// <summary>
    /// ViewModel thống kê cho trang Check-out
    /// </summary>
    public class CheckOutStatViewModel
    {
 [Display(Name = "Cần check-out hôm nay")]
        public int CanCheckOutHomNay { get; set; }

 [Display(Name = "Đã check-out hôm nay")]
     public int DaCheckOutHomNay { get; set; }

      [Display(Name = "Quá hạn check-out")]
        public int QuaHan { get; set; }

 [Display(Name = "Tổng đơn đang ở")]
        public int TongDonDangO { get; set; }
    }
}
