using System.Collections.Generic;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhachHang
{
    /// <summary>
    /// ViewModel cho danh sách khách hàng
    /// </summary>
    public class KhachHangListViewModel
    {
        /// <summary>
        /// Bộ lọc
        /// </summary>
        public KhachHangFilterViewModel Filter { get; set; }

        /// <summary>
        /// Danh sách khách hàng
        /// </summary>
        public List<KhachHangItemViewModel> DanhSachKhachHang { get; set; }

        /// <summary>
        /// Thống kê
        /// </summary>
        public KhachHangStatViewModel ThongKe { get; set; }

        // ===== PAGINATION =====

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalRecords { get; set; }

        /// <summary>
        /// Số bản ghi mỗi trang
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }

        // ===== COMPUTED PROPERTIES =====

        /// <summary>
        /// Có trang trước không?
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return CurrentPage > 1;
            }
        }

        /// <summary>
        /// Có trang sau không?
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return CurrentPage < TotalPages;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public KhachHangListViewModel()
        {
            Filter = new KhachHangFilterViewModel();
            DanhSachKhachHang = new List<KhachHangItemViewModel>();
            ThongKe = new KhachHangStatViewModel();
        }
    }
}

