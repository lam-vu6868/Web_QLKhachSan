using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhachHang
{
    /// <summary>
    /// ViewModel cho bộ lọc tìm kiếm khách hàng
    /// </summary>
    public class KhachHangFilterViewModel
    {
        // ===== TÌM KIẾM =====
        
        [Display(Name = "Tìm kiếm")]
        [StringLength(100)]
        public string TimKiem { get; set; }

        // ===== LỌC THEO GIỚI TÍNH =====
        
        [Display(Name = "Giới tính")]
        public byte? GioiTinh { get; set; }

        // ===== LỌC THEO THỜI GIAN =====
        
        [Display(Name = "Từ ngày tạo")]
        [DataType(DataType.Date)]
        public DateTime? TuNgayTao { get; set; }

        [Display(Name = "Đến ngày tạo")]
        [DataType(DataType.Date)]
        public DateTime? DenNgayTao { get; set; }

        // ===== LỌC THEO LOẠI KHÁCH HÀNG =====
        
        [Display(Name = "Có tài khoản")]
        public bool? CoTaiKhoan { get; set; }

        [Display(Name = "Khách hàng VIP")]
        public bool? LaKhachHangVIP { get; set; }

        // ===== PAGINATION =====
        
        [Display(Name = "Trang hiện tại")]
        public int CurrentPage { get; set; } = 1;

        [Display(Name = "Số bản ghi mỗi trang")]
        public int PageSize { get; set; } = 10;

        // ===== HELPER METHODS =====

        /// <summary>
        /// Có đang lọc không?
        /// </summary>
        public bool HasFilter
        {
            get
            {
                return !string.IsNullOrWhiteSpace(TimKiem) ||
                    GioiTinh.HasValue ||
                    TuNgayTao.HasValue ||
                    DenNgayTao.HasValue ||
                    CoTaiKhoan.HasValue ||
                    LaKhachHangVIP.HasValue;
            }
        }

        /// <summary>
        /// Reset tất cả bộ lọc
        /// </summary>
        public void Reset()
        {
            TimKiem = null;
            GioiTinh = null;
            TuNgayTao = null;
            DenNgayTao = null;
            CoTaiKhoan = null;
            LaKhachHangVIP = null;
            CurrentPage = 1;
        }
    }
}

