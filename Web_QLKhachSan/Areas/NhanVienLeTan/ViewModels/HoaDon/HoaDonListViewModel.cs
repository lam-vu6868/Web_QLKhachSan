using System.Collections.Generic;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho trang danh sách hóa đơn
    /// </summary>
    public class HoaDonListViewModel
    {
        public HoaDonListViewModel()
  {
            DanhSachHoaDon = new List<HoaDonItemViewModel>();
            Filter = new HoaDonFilterViewModel();
      ThongKe = new HoaDonThongKeViewModel();
        CurrentPage = 1;
    PageSize = 10;
        }

        // ===== DANH SÁCH HÓA ĐƠN =====
        public List<HoaDonItemViewModel> DanhSachHoaDon { get; set; }

        // ===== BỘ LỌC =====
   public HoaDonFilterViewModel Filter { get; set; }

    // ===== THỐNG KÊ =====
        public HoaDonThongKeViewModel ThongKe { get; set; }

        // ===== PHÂN TRANG =====
        public int CurrentPage { get; set; }
      public int PageSize { get; set; }
        public int TotalRecords { get; set; }
      public int TotalPages => (int)System.Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
