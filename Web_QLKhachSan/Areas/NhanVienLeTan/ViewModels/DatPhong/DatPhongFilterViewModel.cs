using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho bộ lọc tìm kiếm đơn đặt phòng
    /// </summary>
    public class DatPhongFilterViewModel
  {
        // ===== TÌM KIẾM =====
   
 [Display(Name = "Tìm kiếm")]
        [StringLength(100)]
public string TimKiem { get; set; }

  // ===== LỌC THEO TRẠNG THÁI =====
    
 [Display(Name = "Trạng thái đặt phòng")]
 public byte? TrangThaiDatPhong { get; set; }

[Display(Name = "Trạng thái thanh toán")]
   public byte? TrangThaiThanhToan { get; set; }

   // ===== LỌC THEO THỜI GIAN =====
   
        [Display(Name = "Từ ngày")]
 [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

 [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
public DateTime? DenNgay { get; set; }

        // ===== LỌC THEO LOẠI PHÒNG =====
     
        [Display(Name = "Loại phòng")]
public int? LoaiPhongId { get; set; }

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
   TrangThaiDatPhong.HasValue ||
      TrangThaiThanhToan.HasValue ||
  TuNgay.HasValue ||
        DenNgay.HasValue ||
            LoaiPhongId.HasValue;
   }
        }

        /// <summary>
   /// Reset tất cả bộ lọc
   /// </summary>
        public void Reset()
   {
    TimKiem = null;
     TrangThaiDatPhong = null;
      TrangThaiThanhToan = null;
      TuNgay = null;
          DenNgay = null;
  LoaiPhongId = null;
            CurrentPage = 1;
}
    }
}
