using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho thống kê nhanh đơn đặt phòng
    /// </summary>
 public class DatPhongStatViewModel
  {
        // ===== THỐNG KÊ TỔNG QUAN =====
   
    [Display(Name = "Tổng đơn đặt")]
   public int TongDonDat { get; set; }

        [Display(Name = "Chờ xác nhận")]
        public int ChoXacNhan { get; set; }

        [Display(Name = "Đã xác nhận")]
        public int DaXacNhan { get; set; }

 [Display(Name = "Đã check-in")]
        public int DaCheckIn { get; set; }

 [Display(Name = "Đã check-out")]
  public int DaCheckOut { get; set; }

  [Display(Name = "Đã hủy")]
        public int DaHuy { get; set; }

      // ===== THỐNG KÊ DOANH THU =====
        
  [Display(Name = "Doanh thu dự kiến")]
  [DataType(DataType.Currency)]
        public decimal DoanhThuDuKien { get; set; }

        [Display(Name = "Đã thanh toán")]
[DataType(DataType.Currency)]
  public decimal DaThanhToan { get; set; }

 // ===== COMPUTED PROPERTIES =====

  /// <summary>
   /// Tỷ lệ xác nhận (%)
     /// </summary>
 public double TyLeXacNhan
  {
      get
      {
     if (TongDonDat == 0) return 0;
       return Math.Round((double)(DaXacNhan + DaCheckIn + DaCheckOut) / TongDonDat * 100, 1);
  }
  }

        /// <summary>
 /// Tỷ lệ hủy (%)
  /// </summary>
     public double TyLeHuy
{
     get
            {
  if (TongDonDat == 0) return 0;
   return Math.Round((double)DaHuy / TongDonDat * 100, 1);
 }
        }

   /// <summary>
        /// Còn phải thu
        /// </summary>
        public decimal ConPhaiThu
        {
   get
            {
     return DoanhThuDuKien - DaThanhToan;
  }
   }

/// <summary>
        /// Tỷ lệ thanh toán (%)
  /// </summary>
   public double TyLeThanhToan
        {
          get
            {
        if (DoanhThuDuKien == 0) return 0;
      return Math.Round((double)DaThanhToan / (double)DoanhThuDuKien * 100, 1);
   }
}
    }
}
