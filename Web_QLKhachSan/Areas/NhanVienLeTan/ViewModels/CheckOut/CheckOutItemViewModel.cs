using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckOut
{
    /// <summary>
    /// ViewModel cho từng item trong danh sách check-out
    /// </summary>
    public class CheckOutItemViewModel
    {
      public int DatPhongId { get; set; }

        [Display(Name = "Mã đặt phòng")]
        public string MaDatPhong { get; set; }

        [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

    [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
  public string Email { get; set; }

  [Display(Name = "Danh sách phòng")]
     public string DanhSachPhong { get; set; }

        [Display(Name = "Danh sách loại phòng")]
        public string DanhSachLoaiPhong { get; set; }

  [Display(Name = "Số lượng phòng")]
        public int SoLuongPhong { get; set; }

        [Display(Name = "Ngày check-in")]
    [DataType(DataType.Date)]
   public DateTime? NgayCheckIn { get; set; }

      [Display(Name = "Ngày check-out")]
    [DataType(DataType.Date)]
        public DateTime? NgayCheckOut { get; set; }

        [Display(Name = "Số đêm")]
        public int? SoDem { get; set; }

 [Display(Name = "Số lượng khách")]
    public int? SoLuongKhach { get; set; }

        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
     public decimal? TongTien { get; set; }

      [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        // Trạng thái
        public byte TrangThaiDatPhong { get; set; }

        // Thanh toán
        public bool DaThanhToanOnline { get; set; }
     public string OnlinePaymentStatus { get; set; }

        // Computed Properties
      public bool LaDonOnline
      {
     get { return !string.IsNullOrEmpty(OnlinePaymentStatus); }
        }

public bool CheckOutHomNay
      {
    get
 {
      if (!NgayCheckOut.HasValue) return false;
  return NgayCheckOut.Value.Date == DateTime.Now.Date;
          }
        }

        public bool QuaHanCheckOut
        {
            get
            {
      if (!NgayCheckOut.HasValue) return false;
       return DateTime.Now.Date > NgayCheckOut.Value.Date;
            }
   }

     /// <summary>
    /// Text trạng thái
        /// </summary>
        public string BadgeText
        {
    get
            {
      switch (TrangThaiDatPhong)
 {
     case 2: return "Đang ở";
          case 3: return "Đã check-out";
    default: return "Không xác định";
}
            }
      }

        /// <summary>
        /// Màu badge
   /// </summary>
        public string BadgeColor
  {
      get
            {
  switch (TrangThaiDatPhong)
       {
       case 2: return "#dc3545"; // Đỏ - Đang ở
 case 3: return "#6c757d"; // Xám - Đã check-out
              default: return "#343a40";
     }
  }
 }
    }
}
