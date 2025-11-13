using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho tạo đơn đặt phòng mới
    /// </summary>
    public class DatPhongCreateViewModel
    {
        public DatPhongCreateViewModel()
 {
            DanhSachPhongDat = new List<PhongDatItemViewModel>();
 }

        // ===== THÔNG TIN KHÁCH HÀNG =====
        [Display(Name = "Mã khách hàng")]
        public int? MaKhachHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên khách hàng")]
 [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string HoVaTen { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
 [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
  [StringLength(15)]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
  public string Email { get; set; }

        [Display(Name = "CCCD/Passport")]
      [StringLength(20)]
        public string CCCD { get; set; }

     [Display(Name = "Địa chỉ")]
    [StringLength(200)]
        public string DiaChi { get; set; }

        [Display(Name = "Giới tính")]
        public byte? GioiTinh { get; set; } // 0=Nam, 1=Nữ, 2=Khác

        // ===== THÔNG TIN ĐẶT PHÒNG =====
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [Display(Name = "Ngày nhận phòng")]
[DataType(DataType.Date)]
        public DateTime NgayNhan { get; set; }

        [Display(Name = "Giờ nhận")]
    [DataType(DataType.Time)]
        public string GioNhan { get; set; } = "14:00"; // Check-in mặc định 14:00

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
  [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
   public DateTime NgayTra { get; set; }

        [Display(Name = "Giờ trả")]
        [DataType(DataType.Time)]
        public string GioTra { get; set; } = "12:00"; // Check-out mặc định 12:00

        /// <summary>
        /// Số đêm (tự động tính)
        /// </summary>
        [Display(Name = "Số đêm")]
        public int SoDem
        {
      get
    {
                if (NgayTra > NgayNhan)
        return (NgayTra - NgayNhan).Days;
    return 0;
            }
 }

        [Required(ErrorMessage = "Vui lòng nhập số lượng khách")]
        [Display(Name = "Số lượng khách")]
        [Range(1, 50, ErrorMessage = "Số lượng khách từ 1 đến 50")]
        public int SoLuongKhach { get; set; } = 1;

[Display(Name = "Ghi chú / Yêu cầu đặc biệt")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
        public string GhiChu { get; set; }

        // ===== DANH SÁCH PHÒNG ĐẶT =====
    /// <summary>
        /// Danh sách loại phòng và số lượng khách chọn
        /// </summary>
        public List<PhongDatItemViewModel> DanhSachPhongDat { get; set; }

        // ===== THANH TOÁN =====
      [Display(Name = "Mã khuyến mãi")]
    [StringLength(20)]
  public string MaKhuyenMai { get; set; }

        public int? MaKhuyenMaiId { get; set; } // ID khuyến mãi sau khi validate

 [Display(Name = "Đặt cọc trước")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền đặt cọc không hợp lệ")]
      public decimal? TienDatCoc { get; set; }

        [Display(Name = "Hình thức thanh toán")]
        public byte? HinhThucThanhToan { get; set; } // 0=Tiền mặt, 1=Chuyển khoản, 2=QR

        // ===== TỔNG TIỀN (TỰ ĐỘNG TÍNH) =====
        /// <summary>
   /// Tổng tiền phòng (chưa giảm giá, chưa thuế)
      /// </summary>
  [Display(Name = "Tổng tiền phòng")]
        [DataType(DataType.Currency)]
        public decimal TongTienPhong
        {
     get
   {
        return DanhSachPhongDat?.Sum(p => p.ThanhTien) ?? 0;
    }
        }

     /// <summary>
        /// Tiền giảm giá (từ khuyến mãi)
        /// </summary>
    [Display(Name = "Giảm giá")]
     [DataType(DataType.Currency)]
     public decimal TienGiamGia { get; set; }

  /// <summary>
        /// Thuế VAT (10%)
        /// </summary>
        [Display(Name = "Thuế VAT (10%)")]
        [DataType(DataType.Currency)]
        public decimal ThueVAT
        {
            get
            {
         return (TongTienPhong - TienGiamGia) * 0.1m;
 }
        }

        /// <summary>
        /// Tổng cộng (sau giảm giá + thuế)
        /// </summary>
      [Display(Name = "Tổng cộng")]
[DataType(DataType.Currency)]
   public decimal TongCong
        {
get
    {
        return TongTienPhong - TienGiamGia + ThueVAT;
   }
        }

    /// <summary>
 /// Còn phải thanh toán (sau khi đặt cọc)
        /// </summary>
        [Display(Name = "Còn phải thanh toán")]
        [DataType(DataType.Currency)]
        public decimal ConPhaiThanhToan
        {
            get
         {
   return TongCong - (TienDatCoc ?? 0);
      }
        }

 // ===== VALIDATION =====
        /// <summary>
      /// Custom validation cho ngày
        /// </summary>
        public bool IsValidDateRange()
        {
            return NgayTra > NgayNhan && NgayNhan >= DateTime.Now.Date;
        }

        /// <summary>
        /// Có phòng nào được chọn chưa?
        /// </summary>
 public bool HasRooms()
        {
            return DanhSachPhongDat != null && DanhSachPhongDat.Any(p => p.SoLuong > 0);
        }
    }

    /// <summary>
/// ViewModel cho từng item phòng trong đơn đặt
  /// </summary>
    public class PhongDatItemViewModel
    {
  public int LoaiPhongId { get; set; }

        [Display(Name = "Loại phòng")]
        public string TenLoaiPhong { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Số người tối đa")]
   public int? SoNguoiToiDa { get; set; }

      [Display(Name = "Diện tích (m²)")]
 public decimal? DienTich { get; set; }

   [Display(Name = "Đơn giá (VNĐ/đêm)")]
     [DataType(DataType.Currency)]
        public decimal DonGia { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
     [Range(0, 10, ErrorMessage = "Số lượng từ 0 đến 10")]
        public int SoLuong { get; set; } = 0;

        [Display(Name = "Giảm giá (%)")]
        [Range(0, 100, ErrorMessage = "Giảm giá từ 0% đến 100%")]
      public decimal GiamGia { get; set; } = 0;

        /// <summary>
  /// Số đêm (truyền từ parent)
        /// </summary>
        public int SoDem { get; set; } = 1;

/// <summary>
  /// Thành tiền = SoLuong * DonGia * SoDem * (1 - GiamGia/100)
        /// </summary>
   [Display(Name = "Thành tiền")]
    [DataType(DataType.Currency)]
     public decimal ThanhTien
        {
            get
            {
      if (SoLuong <= 0 || SoDem <= 0) return 0;
          decimal tienGoc = SoLuong * DonGia * SoDem;
          decimal tienGiam = tienGoc * (GiamGia / 100);
   return tienGoc - tienGiam;
            }
        }

        /// <summary>
        /// Số phòng trống của loại này
        /// </summary>
   [Display(Name = "Phòng trống")]
        public int SoPhongTrong { get; set; }

        /// <summary>
        /// Hình ảnh thumbnail
        /// </summary>
     public string HinhAnhUrl { get; set; }

        /// <summary>
        /// Có đủ phòng trống không?
        /// </summary>
   public bool HasEnoughRooms => SoLuong <= SoPhongTrong;
    }
}
