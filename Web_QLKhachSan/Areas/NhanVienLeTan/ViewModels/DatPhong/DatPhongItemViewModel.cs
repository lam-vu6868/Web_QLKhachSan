using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel đại diện cho 1 đơn đặt phòng trong danh sách
    /// </summary>
    public class DatPhongItemViewModel
    {
        // ===== THÔNG TIN CƠ BẢN =====
        
        public int DatPhongId { get; set; }

        [Display(Name = "Mã đặt phòng")]
      public string MaDatPhong { get; set; }

        [Display(Name = "Ngày đặt")]
        [DataType(DataType.DateTime)]
    public DateTime NgayDat { get; set; }

        [Display(Name = "Ngày nhận")]
        [DataType(DataType.Date)]
        public DateTime? NgayNhan { get; set; }

        [Display(Name = "Ngày trả")]
        [DataType(DataType.Date)]
        public DateTime? NgayTra { get; set; }

        [Display(Name = "Số đêm")]
    public int? SoDem { get; set; }

        // ===== THÔNG TIN KHÁCH HÀNG (NESTED VIEWMODEL) =====
   
 public KhachHangItemViewModel KhachHang { get; set; }

        // ===== THÔNG TIN NHÂN VIÊN (NESTED VIEWMODEL) =====
        
        public NhanVienItemViewModel NhanVien { get; set; }

        // ===== THÔNG TIN KHUYẾN MÃI (NESTED VIEWMODEL) =====
        
        public KhuyenMaiItemViewModel KhuyenMai { get; set; }

   // ===== THÔNG TIN PHÒNG =====

        [Display(Name = "Số lượng phòng")]
        public int SoLuongPhong { get; set; }

    [Display(Name = "Danh sách loại phòng")]
  public string DanhSachLoaiPhong { get; set; }

        [Display(Name = "Danh sách phòng")]
        public string DanhSachPhong { get; set; }

      [Display(Name = "Số lượng khách")]
        public int? SoLuongKhach { get; set; }

   // ===== TRẠNG THÁI =====
        
        /// <summary>
        /// 0=Chờ xác nhận, 1=Đã xác nhận, 2=Đã check-in, 3=Đã check-out, 4=Đã hủy
  /// </summary>
        public byte TrangThaiDatPhong { get; set; }

        [Display(Name = "Trạng thái đặt phòng")]
        public string TrangThaiDatPhongText { get; set; }

        public string TrangThaiDatPhongColor { get; set; }

/// <summary>
        /// 0=Chưa thanh toán, 2=Đã thanh toán đủ
   /// (Bỏ trạng thái 1 - không có đặt cọc)
   /// </summary>
  public byte TrangThaiThanhToan { get; set; }

 [Display(Name = "Trạng thái thanh toán")]
   public string TrangThaiThanhToanText { get; set; }

      public string TrangThaiThanhToanColor { get; set; }

        // ===== TỔNG TIỀN =====
        
        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
     public decimal? TongTien { get; set; }

  // ===== THANH TOÁN =====
        
 /// <summary>
        /// 0=Tiền mặt, 1=Chuyển khoản, 2=QR Code/VNPay
        /// </summary>
        public byte? HinhThucThanhToan { get; set; }

        // ===== ONLINE PAYMENT =====
      
        public string PaymentRefId { get; set; }
        public string PaymentMethod { get; set; }
        public string OnlinePaymentStatus { get; set; }
public decimal? TotalAmount { get; set; }

        // ===== THÔNG TIN KHÁC =====
     
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

   [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayTao { get; set; }

        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayCapNhat { get; set; }

      // ===== COMPUTED PROPERTIES =====

   /// <summary>
        /// Hiển thị tên khách hàng
        /// </summary>
        public string TenKhachHienThi
     {
            get
     {
        return KhachHang?.HoVaTen ?? "N/A";
 }
        }

        /// <summary>
        /// Số điện thoại khách hàng
        /// </summary>
public string SoDienThoaiKhach
   {
            get
            {
  return KhachHang?.SoDienThoai ?? "";
            }
        }

        /// <summary>
        /// Đơn đặt online hay offline?
        /// </summary>
        public bool LaDonOnline
        {
       get
  {
    return !string.IsNullOrEmpty(PaymentRefId);
  }
   }

   /// <summary>
    /// Còn bao nhiêu ngày đến ngày nhận?
    /// </summary>
        public int SoNgayDenNgayNhan
        {
            get
            {
if (!NgayNhan.HasValue) return 0;
       return (NgayNhan.Value.Date - DateTime.Now.Date).Days;
            }
     }

    /// <summary>
      /// Đã quá hạn check-in chưa?
        /// </summary>
        public bool QuaHanCheckIn
        {
            get
         {
        if (!NgayNhan.HasValue) return false;
         return DateTime.Now.Date > NgayNhan.Value.Date && TrangThaiDatPhong < 2;
            }
        }

        /// <summary>
        /// Check-in hôm nay?
        /// </summary>
        public bool CheckInHomNay
        {
     get
          {
   if (!NgayNhan.HasValue) return false;
              return NgayNhan.Value.Date == DateTime.Now.Date;
            }
        }

        /// <summary>
        /// Check-out hôm nay?
     /// </summary>
        public bool CheckOutHomNay
        {
     get
     {
          if (!NgayTra.HasValue) return false;
            return NgayTra.Value.Date == DateTime.Now.Date;
            }
        }
    }
}
