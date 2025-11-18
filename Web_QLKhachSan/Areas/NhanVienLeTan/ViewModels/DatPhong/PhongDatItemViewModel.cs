using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
  /// ViewModel đại diện cho 1 loại phòng trong danh sách chọn phòng
    /// </summary>
    public class PhongDatItemViewModel
    {
      // ===== THÔNG TIN LOẠI PHÒNG =====
   
     public int LoaiPhongId { get; set; }

        [Display(Name = "Tên loại phòng")]
    public string TenLoaiPhong { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Số người tối đa")]
   public int? SoNguoiToiDa { get; set; }

        [Display(Name = "Diện tích (m²)")]
  public decimal? DienTich { get; set; }

        [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
  public decimal DonGia { get; set; }

        // ===== SỐ LƯỢNG PHÒNG ĐẶT =====
        
        [Display(Name = "Số lượng")]
        [Range(0, 10, ErrorMessage = "Số lượng từ 0 đến 10")]
        public int SoLuong { get; set; }

   [Display(Name = "Số phòng trống")]
        public int SoPhongTrong { get; set; }

        // ===== GIẢM GIÁ =====
        
   [Display(Name = "Giảm giá (%)")]
        [Range(0, 100, ErrorMessage = "Giảm giá từ 0% đến 100%")]
        public decimal GiamGia { get; set; }

        // ===== TẦNG ƯU TIÊN =====
        
        [Display(Name = "Tầng ưu tiên")]
   public int? Tang { get; set; }

    // ===== COMPUTED PROPERTIES =====
        
    /// <summary>
        /// Số đêm (được set từ DatPhongCreateViewModel)
  /// </summary>
   public int SoDem { get; set; }

    /// <summary>
  /// Thành tiền = DonGia * SoLuong * SoDem - Giảm giá
        /// </summary>
        public decimal ThanhTien
        {
      get
            {
     if (SoLuong <= 0 || SoDem <= 0)
         return 0;

       decimal tongTien = DonGia * SoLuong * SoDem;
      decimal tienGiam = tongTien * (GiamGia / 100);
       return tongTien - tienGiam;
        }
        }

        /// <summary>
        /// Còn phòng trống không?
        /// </summary>
        public bool CoPhongTrong
 {
          get
      {
 return SoPhongTrong > 0;
        }
        }

        /// <summary>
   /// Số lượng đặt có hợp lệ không (không vượt quá số phòng trống)
        /// </summary>
   public bool IsValidQuantity
        {
    get
   {
       return SoLuong >= 0 && SoLuong <= SoPhongTrong;
          }
        }
    }
}
