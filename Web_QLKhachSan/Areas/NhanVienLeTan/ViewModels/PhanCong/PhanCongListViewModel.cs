using System.Collections.Generic;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.PhanCong
{
    /// <summary>
    /// ViewModel cho danh sách phân công công việc
    /// </summary>
    public class PhanCongListViewModel
    {
        public PhanCongFilterViewModel Filter { get; set; }
        public List<PhanCongItemViewModel> DanhSachPhanCong { get; set; }

        // Phân trang
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages
        {
            get
            {
                if (PageSize <= 0) return 1;
                return (int)System.Math.Ceiling((double)TotalRecords / PageSize);
            }
        }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

