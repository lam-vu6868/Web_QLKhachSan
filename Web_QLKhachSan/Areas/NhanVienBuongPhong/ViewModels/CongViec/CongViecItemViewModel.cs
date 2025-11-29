using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienBuongPhong.ViewModels.CongViec
{
    /// <summary>
    /// ViewModel đại diện cho 1 công việc trong danh sách
    /// </summary>
    public class CongViecItemViewModel
    {
        public int PhanCongId { get; set; }

        [Display(Name = "Mã phân công")]
        public string MaPhanCong { get; set; }

        [Display(Name = "Phòng")]
        public int PhongId { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public int? Tang { get; set; }

        [Display(Name = "Nhân viên lễ tân")]
        public int? NhanVienLeTanId { get; set; }
        public string TenNhanVienLeTan { get; set; }

        [Display(Name = "Trạng thái")]
        public byte TrangThaiCongViec { get; set; }

        [Display(Name = "Mức độ ưu tiên")]
        public byte MucDoUuTien { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Ghi chú nhân viên")]
        public string GhiChuNhanVien { get; set; }

        [Display(Name = "Thời gian phân công")]
        public DateTime ThoiGianPhanCong { get; set; }

        [Display(Name = "Thời gian bắt đầu")]
        public DateTime? ThoiGianBatDau { get; set; }

        [Display(Name = "Thời gian hoàn thành")]
        public DateTime? ThoiGianHoanThanh { get; set; }

        [Display(Name = "Thời gian dự kiến")]
        public DateTime? ThoiGianDuKien { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        // ===== COMPUTED PROPERTIES =====

        /// <summary>
        /// Text trạng thái công việc
        /// </summary>
        public string TrangThaiText
        {
            get
            {
                switch (TrangThaiCongViec)
                {
                    case 0: return "Chờ xử lý";
                    case 1: return "Đang làm";
                    case 2: return "Hoàn thành";
                    case 3: return "Hủy";
                    default: return "Không xác định";
                }
            }
        }

        /// <summary>
        /// CSS class cho badge trạng thái
        /// </summary>
        public string TrangThaiBadgeClass
        {
            get
            {
                switch (TrangThaiCongViec)
                {
                    case 0: return "badge-warning"; // Chờ xử lý - Vàng
                    case 1: return "badge-info"; // Đang làm - Xanh dương
                    case 2: return "badge-success"; // Hoàn thành - Xanh lá
                    case 3: return "badge-danger"; // Hủy - Đỏ
                    default: return "badge-secondary";
                }
            }
        }

        /// <summary>
        /// Text mức độ ưu tiên
        /// </summary>
        public string MucDoUuTienText
        {
            get
            {
                switch (MucDoUuTien)
                {
                    case 1: return "Thường";
                    case 2: return "Cao";
                    case 3: return "Khẩn cấp";
                    default: return "Thường";
                }
            }
        }

        /// <summary>
        /// CSS class cho badge mức độ ưu tiên
        /// </summary>
        public string MucDoUuTienBadgeClass
        {
            get
            {
                switch (MucDoUuTien)
                {
                    case 1: return "badge-secondary"; // Thường - Xám
                    case 2: return "badge-warning"; // Cao - Vàng
                    case 3: return "badge-danger"; // Khẩn cấp - Đỏ
                    default: return "badge-secondary";
                }
            }
        }

        /// <summary>
        /// Có quá hạn không? (nếu có thời gian dự kiến)
        /// </summary>
        public bool QuaHan
        {
            get
            {
                if (!ThoiGianDuKien.HasValue || TrangThaiCongViec == 2 || TrangThaiCongViec == 3) return false;
                return DateTime.Now > ThoiGianDuKien.Value;
            }
        }

        /// <summary>
        /// Có thể bắt đầu công việc không? (chỉ khi trạng thái = Chờ xử lý)
        /// </summary>
        public bool CoTheBatDau
        {
            get
            {
                return TrangThaiCongViec == 0;
            }
        }

        /// <summary>
        /// Có thể hoàn thành công việc không? (chỉ khi trạng thái = Đang làm)
        /// </summary>
        public bool CoTheHoanThanh
        {
            get
            {
                return TrangThaiCongViec == 1;
            }
        }
    }
}

