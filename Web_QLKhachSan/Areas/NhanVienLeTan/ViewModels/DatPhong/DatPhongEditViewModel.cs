using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho chỉnh sửa đơn đặt phòng
    /// Chỉ cho phép sửa khi đơn chưa check-in (TrangThaiDatPhong < 2)
    /// </summary>
    public class DatPhongEditViewModel
    {
        public DatPhongEditViewModel()
        {
        DanhSachPhongDat = new List<PhongDatItemViewModel>();
        }

   // ===== THÔNG TIN ĐƠN =====
        public int DatPhongId { get; set; }

        [Display(Name = "Mã đặt phòng")]
        public string MaDatPhong { get; set; }

        public byte TrangThaiDatPhong { get; set; }
      public byte TrangThaiThanhToan { get; set; }

      // ===== THÔNG TIN KHÁCH HÀNG =====
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

   // ===== THÔNG TIN ĐẶT PHÒNG =====
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
 public DateTime NgayNhan { get; set; }

[Display(Name = "Giờ nhận")]
 [DataType(DataType.Time)]
        public string GioNhan { get; set; } = "14:00";

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
   [Display(Name = "Ngày trả phòng")]
 [DataType(DataType.Date)]
   public DateTime NgayTra { get; set; }

[Display(Name = "Giờ trả")]
        [DataType(DataType.Time)]
        public string GioTra { get; set; } = "12:00";

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
      public int SoLuongKhach { get; set; }

        [Display(Name = "Ghi chú / Yêu cầu đặc biệt")]
        [StringLength(500)]
        [DataType(DataType.MultilineText)]
     public string GhiChu { get; set; }

        // ===== DANH SÁCH PHÒNG =====
  public List<PhongDatItemViewModel> DanhSachPhongDat { get; set; }

        // ===== KHUYẾN MÃI & THANH TOÁN =====
    [Display(Name = "Mã khuyến mãi")]
        [StringLength(20)]
        public string MaKhuyenMai { get; set; }

        public int? MaKhuyenMaiId { get; set; }

    [Display(Name = "Tiền giảm giá")]
        [DataType(DataType.Currency)]
        public decimal TienGiamGia { get; set; }

        [Display(Name = "Hình thức thanh toán")]
        public byte? HinhThucThanhToan { get; set; }

 // ===== TỔNG TIỀN =====
        [Display(Name = "Tổng tiền phòng")]
      [DataType(DataType.Currency)]
 public decimal TongTienPhong
        {
       get
            {
       return DanhSachPhongDat?.Sum(p => p.ThanhTien) ?? 0;
            }
        }

        [Display(Name = "Thuế VAT (10%)")]
        [DataType(DataType.Currency)]
        public decimal ThueVAT
      {
       get
      {
  return (TongTienPhong - TienGiamGia) * 0.1m;
  }
        }

        [Display(Name = "Tổng cộng")]
        [DataType(DataType.Currency)]
 public decimal TongCong
        {
            get
    {
       return TongTienPhong - TienGiamGia + ThueVAT;
          }
  }

      // ===== LÝ DO SỬA =====
        [Required(ErrorMessage = "Vui lòng nhập lý do thay đổi")]
        [Display(Name = "Lý do thay đổi")]
        [StringLength(200)]
        public string LyDoThayDoi { get; set; }

        // ===== VALIDATION =====
        public bool IsValidDateRange()
        {
   return NgayTra > NgayNhan;
        }

        public bool CanEdit()
        {
            return TrangThaiDatPhong < 2; // Chỉ sửa được khi chưa check-in
        }
    }
}
