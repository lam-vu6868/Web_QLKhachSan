using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web_QLKhachSan.Models
{
    public class DanhGiaViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn phòng")]
        [Display(Name = "Phòng đã đặt")]
        public int PhongId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5 sao")]
        [Display(Name = "Điểm đánh giá")]
        public byte Diem { get; set; }

        [StringLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự")]
        [Display(Name = "Nội dung bình luận")]
        public string BinhLuan { get; set; }

        // Danh sách ảnh đính kèm (sẽ xử lý sau)
        public List<HttpPostedFileBase> HinhAnhs { get; set; }
    }

    public class DanhGiaIndexViewModel
    {
        public List<DanhGia> DanhSachDanhGia { get; set; }
        public List<System.Web.Mvc.SelectListItem> DanhSachPhong { get; set; }
        public string TenKhachHang { get; set; }
        public string EmailKhachHang { get; set; }
        
        // Thống kê
        public double TongDiem { get; set; }
        public int SoDanhGia { get; set; }
        public double PhanTramHaiLong { get; set; }
    }
}
