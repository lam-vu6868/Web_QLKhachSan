using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.PhanCong
{
    /// <summary>
    /// ViewModel cho bộ lọc danh sách phân công
    /// </summary>
    public class PhanCongFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; } // Tìm theo mã phân công, mã phòng, tên nhân viên

        [Display(Name = "Trạng thái")]
        public byte? TrangThaiCongViec { get; set; } // 0=Chờ xử lý, 1=Đang làm, 2=Hoàn thành, 3=Hủy

        [Display(Name = "Mức độ ưu tiên")]
        public byte? MucDoUuTien { get; set; } // 1=Thường, 2=Cao, 3=Khẩn cấp

        [Display(Name = "Nhân viên buồng phòng")]
        public int? NhanVienBuongPhongId { get; set; }

        [Display(Name = "Phòng")]
        public int? PhongId { get; set; }

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

