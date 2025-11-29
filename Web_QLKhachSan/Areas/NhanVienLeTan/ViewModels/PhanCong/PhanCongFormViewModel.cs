using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.PhanCong
{
    /// <summary>
    /// ViewModel cho form tạo/sửa phân công
    /// </summary>
    public class PhanCongFormViewModel
    {
        public int? PhanCongId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phòng!")]
        [Display(Name = "Phòng cần dọn")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng!")]
        public int PhongId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên buồng phòng!")]
        [Display(Name = "Nhân viên buồng phòng")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên buồng phòng!")]
        public int NhanVienBuongPhongId { get; set; }

        [Display(Name = "Mức độ ưu tiên")]
        public byte MucDoUuTien { get; set; } = 1; // Mặc định: Thường

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự!")]
        public string GhiChu { get; set; }

        [Display(Name = "Thời gian dự kiến hoàn thành")]
        [DataType(DataType.DateTime)]
        [FutureDate(30, ErrorMessage = "Thời gian dự kiến không được quá 30 phút trong quá khứ!")]
        public DateTime? ThoiGianDuKien { get; set; }

        // ===== DROPDOWN DATA =====
        public SelectList DanhSachPhong { get; set; }
        public SelectList DanhSachNhanVienBuongPhong { get; set; }
    }
}

