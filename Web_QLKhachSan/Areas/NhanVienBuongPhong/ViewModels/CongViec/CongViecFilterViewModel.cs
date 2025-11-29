using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienBuongPhong.ViewModels.CongViec
{
    /// <summary>
    /// ViewModel cho bộ lọc danh sách công việc
    /// </summary>
    public class CongViecFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; }

        [Display(Name = "Trạng thái")]
        public byte? TrangThaiCongViec { get; set; }

        [Display(Name = "Mức độ ưu tiên")]
        public byte? MucDoUuTien { get; set; }

        [Display(Name = "Từ ngày phân công")]
        [DataType(DataType.Date)]
        public DateTime? TuNgayPhanCong { get; set; }

        [Display(Name = "Đến ngày phân công")]
        [DataType(DataType.Date)]
        public DateTime? DenNgayPhanCong { get; set; }

        // Phân trang
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

