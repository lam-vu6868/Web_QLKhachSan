using System;
using System.Collections.Generic;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
    /// <summary>
    /// ViewModel cho danh sách khuyến mãi
    /// </summary>
    public class KhuyenMaiListViewModel
    {
        public KhuyenMaiListViewModel()
        {
            DanhSachKhuyenMai = new List<KhuyenMaiItemViewModel>();
            Filter = new KhuyenMaiFilterViewModel();
  ThongKe = new KhuyenMaiThongKeViewModel();
     }

        public List<KhuyenMaiItemViewModel> DanhSachKhuyenMai { get; set; }
     public KhuyenMaiFilterViewModel Filter { get; set; }
        public KhuyenMaiThongKeViewModel ThongKe { get; set; }

        // Phân trang
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
 }
}
