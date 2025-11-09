using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels
{
    /// <summary>
    /// ViewModel hiển thị trạng thái từng phòng trên sơ đồ
    /// </summary>
    public class PhongStatusViewModel
    {
        public int PhongId { get; set; }
        
        [Display(Name = "Mã phòng")]
        public string MaPhong { get; set; }
        
 [Display(Name = "Tên phòng")]
        public string TenPhong { get; set; }
        
      [Display(Name = "Tầng")]
        public int Tang { get; set; }
 
        // ===== TRẠNG THÁI PHÒNG =====
        /// <summary>
   /// 0 = Trống (Available)
      /// 1 = Đã đặt (Reserved)
      /// 2 = Đang sử dụng (Occupied)
   /// 3 = Đang dọn dẹp (Cleaning)
        /// 4 = Bảo trì (Maintenance)
        /// </summary>
        public byte TrangThaiPhong { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string TrangThaiPhongText
      {
    get
  {
   switch (TrangThaiPhong)
       {
              case 0: return "Trống";
 case 1: return "Đã đặt";
         case 2: return "Đang ở";
       case 3: return "Đang dọn";
      case 4: return "Bảo trì";
    default: return "Không xác định";
     }
            }
   }
     
        /// <summary>
        /// Màu sắc hiển thị trên sơ đồ
  /// </summary>
        public string TrangThaiPhongColor
   {
get
     {
        switch (TrangThaiPhong)
       {
        case 0: return "#28a745"; // Xanh lá - Trống
             case 1: return "#ffc107"; // Vàng - Đã đặt
         case 2: return "#dc3545"; // Đỏ - Đang ở
       case 3: return "#fd7e14"; // Cam - Đang dọn
       case 4: return "#6c757d"; // Xám - Bảo trì
          default: return "#343a40"; // Đen - Lỗi
 }
  }
        }
        
    /// <summary>
   /// Icon hiển thị
        /// </summary>
        public string TrangThaiPhongIcon
      {
            get
            {
        switch (TrangThaiPhong)
      {
         case 0: return "fa-door-open";     // Trống
     case 1: return "fa-calendar-check"; // Đã đặt
     case 2: return "fa-user";// Đang ở
        case 3: return "fa-broom";      // Đang dọn
           case 4: return "fa-tools";         // Bảo trì
           default: return "fa-question";
        }
   }
        }
        
        // ===== THÔNG TIN LOẠI PHÒNG =====
        public int LoaiPhongId { get; set; }
        
        [Display(Name = "Loại phòng")]
        public string TenLoaiPhong { get; set; }
    
        [Display(Name = "Giá")]
        [DataType(DataType.Currency)]
  public decimal? GiaPhong { get; set; }
     
        [Display(Name = "Số người tối đa")]
        public int? SoNguoiToiDa { get; set; }
  
        // ===== THÔNG TIN KHÁCH ĐANG Ở (NẾU CÓ) =====
        public int? DatPhongId { get; set; }
        public int? MaKhachHang { get; set; }
        
        [Display(Name = "Tên khách")]
    public string TenKhach { get; set; }
        
        [Display(Name = "SĐT")]
     public string SoDienThoaiKhach { get; set; }
        
        [Display(Name = "Check-in")]
        [DataType(DataType.Date)]
        public DateTime? NgayCheckIn { get; set; }
        
      [Display(Name = "Check-out")]
   [DataType(DataType.Date)]
    public DateTime? NgayCheckOut { get; set; }
        
        [Display(Name = "Số người")]
        public int? SoNguoi { get; set; }
    
        // ===== GHI CHÚ =====
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
        
        /// <summary>
        /// Có yêu cầu đặc biệt hay không
        /// </summary>
public bool CoYeuCauDacBiet
{
      get
            {
 return !string.IsNullOrWhiteSpace(GhiChu);
  }
        }
        
        // ===== TIỆN ÍCH HIỂN THỊ =====
        /// <summary>
        /// Số ngày đã ở (nếu đang check-in)
        /// </summary>
     public int? SoNgayDaO
        {
            get
          {
         if (NgayCheckIn.HasValue && TrangThaiPhong == 2)
                {
  return (DateTime.Now.Date - NgayCheckIn.Value.Date).Days;
    }
   return null;
          }
  }
        
      /// <summary>
        /// Còn bao nhiêu ngày nữa check-out
  /// </summary>
        public int? SoNgayConLai
        {
      get
        {
    if (NgayCheckOut.HasValue && TrangThaiPhong == 2)
          {
     return (NgayCheckOut.Value.Date - DateTime.Now.Date).Days;
       }
     return null;
     }
    }
        
        /// <summary>
        /// Check-out hôm nay?
    /// </summary>
        public bool CheckOutHomNay
        {
            get
            {
     return NgayCheckOut.HasValue && NgayCheckOut.Value.Date == DateTime.Now.Date;
            }
  }
        
     /// <summary>
        /// Check-in hôm nay?
 /// </summary>
        public bool CheckInHomNay
   {
get
     {
                return NgayCheckIn.HasValue && NgayCheckIn.Value.Date == DateTime.Now.Date;
            }
  }
    }
}
