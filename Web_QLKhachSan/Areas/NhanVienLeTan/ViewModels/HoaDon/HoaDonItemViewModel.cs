using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho item hóa đơn trong danh sách
    /// </summary>
    public class HoaDonItemViewModel
    {
      // ===== THÔNG TIN HÓA ĐƠN =====
        public int HoaDonId { get; set; }

  [Display(Name = "Mã hóa đơn")]
        public string MaHoaDon { get; set; }

    [Display(Name = "Ngày lập")]
   [DataType(DataType.DateTime)]
        public DateTime NgayLap { get; set; }

  [Display(Name = "Tổng tiền")]
   [DataType(DataType.Currency)]
     public decimal? TongTien { get; set; }

      [Display(Name = "Giảm giá")]
  [DataType(DataType.Currency)]
   public decimal? GiamGia { get; set; }

     [Display(Name = "Thuế")]
        [DataType(DataType.Currency)]
 public decimal? Thue { get; set; }

    [Display(Name = "Thanh toán cuối")]
  [DataType(DataType.Currency)]
   public decimal? ThanhToanCuoi { get; set; }

 [Display(Name = "Trạng thái")]
   public byte TrangThaiHoaDon { get; set; }

     public string TrangThaiHoaDonText { get; set; }
   public string TrangThaiHoaDonColor { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; }

  [Display(Name = "Ngày thanh toán")]
  [DataType(DataType.DateTime)]
        public DateTime? NgayThanhToan { get; set; }

        [Display(Name = "Người lập")]
   public string NguoiLapHoaDon { get; set; }

        [Display(Name = "Ghi chú")]
 public string GhiChu { get; set; }

   // ===== THÔNG TIN ĐẶT PHÒNG =====
  public int? DatPhongId { get; set; }
   public string MaDatPhong { get; set; }

        // ===== THÔNG TIN KHÁCH HÀNG =====
        public int? MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoaiKhachHang { get; set; }

// ===== THÔNG TIN THANH TOÁN =====
      public int SoLanThanhToan { get; set; }
        public List<ThanhToanItemViewModel> DanhSachThanhToan { get; set; }

        // ===== THỜI GIAN =====
 public DateTime NgayTao { get; set; }
    }
}
