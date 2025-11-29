using System;
using System.Collections.Generic;

namespace Web_QLKhachSan.Areas.NhanVienBuongPhong.ViewModels.CongViec
{
    /// <summary>
    /// ViewModel cho trang danh sách công việc
    /// </summary>
    public class CongViecListViewModel
    {
        public List<CongViecItemViewModel> DanhSachCongViec { get; set; }
        public CongViecFilterViewModel Filter { get; set; }

        // Phân trang
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages
        {
            get
            {
                if (PageSize <= 0) return 1;
                return (int)Math.Ceiling((double)TotalRecords / PageSize);
            }
        }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}

