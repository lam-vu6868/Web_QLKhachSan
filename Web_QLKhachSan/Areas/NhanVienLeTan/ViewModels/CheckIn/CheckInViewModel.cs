using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.CheckIn
{
    /// <summary>
    /// ViewModel chính cho trang danh sách Check-in
    /// </summary>
    public class CheckInListViewModel
    {
        public CheckInListViewModel()
        {
            Filter = new CheckInFilterViewModel();
            DanhSachCheckIn = new List<CheckInItemViewModel>();
            ThongKe = new CheckInStatViewModel();
        }

        public CheckInFilterViewModel Filter { get; set; }
      public List<CheckInItemViewModel> DanhSachCheckIn { get; set; }
        public CheckInStatViewModel ThongKe { get; set; }

        // Pagination
   public int TotalRecords { get; set; }
      public int CurrentPage { get; set; }
        public int PageSize { get; set; }
   public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
   public bool HasNextPage => CurrentPage < TotalPages;
    }

    /// <summary>
    /// ViewModel cho bộ lọc
    /// </summary>
    public class CheckInFilterViewModel
    {
 [Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; }

  [Display(Name = "Ngày check-in")]
        [DataType(DataType.Date)]
        public DateTime? NgayCheckIn { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
 }

    /// <summary>
  /// ViewModel cho thống kê Check-in
    /// </summary>
    public class CheckInStatViewModel
    {
      [Display(Name = "Cần check-in hôm nay")]
     public int CanCheckInHomNay { get; set; }

   [Display(Name = "Đã check-in hôm nay")]
        public int DaCheckInHomNay { get; set; }

        [Display(Name = "Quá hạn")]
  public int QuaHan { get; set; }

     [Display(Name = "Tổng đơn chờ")]
        public int TongDonCho { get; set; }
    }

    /// <summary>
    /// ViewModel cho từng item trong danh sách Check-in
    /// </summary>
    public class CheckInItemViewModel
    {
        public int DatPhongId { get; set; }

     [Display(Name = "Mã đặt phòng")]
        public string MaDatPhong { get; set; }

[Display(Name = "Ngày đặt")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDat { get; set; }

        [Display(Name = "Ngày check-in")]
        [DataType(DataType.Date)]
  public DateTime? NgayNhan { get; set; }

        [Display(Name = "Ngày check-out")]
        [DataType(DataType.Date)]
    public DateTime? NgayTra { get; set; }

        [Display(Name = "Số đêm")]
        public int? SoDem { get; set; }

        [Display(Name = "Số lượng khách")]
        public int? SoLuongKhach { get; set; }

     // Thông tin khách hàng
        [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

      [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
public string Email { get; set; }

        // Thông tin phòng
        [Display(Name = "Số lượng phòng")]
 public int SoLuongPhong { get; set; }

        [Display(Name = "Loại phòng")]
        public string DanhSachLoaiPhong { get; set; }

     [Display(Name = "Danh sách phòng")]
    public string DanhSachPhong { get; set; }

        // Trạng thái
        public byte TrangThaiDatPhong { get; set; }

        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        public decimal? TongTien { get; set; }

        [Display(Name = "Ghi chú")]
      public string GhiChu { get; set; }

 // Online payment
     public bool LaDonOnline { get; set; }
 public string OnlinePaymentStatus { get; set; }

     // Nhân viên
  public string NhanVienDatPhong { get; set; }

        // Computed properties
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
        /// Đã quá hạn check-in?
      /// </summary>
        public bool QuaHanCheckIn
        {
       get
   {
 if (!NgayNhan.HasValue) return false;
          return DateTime.Now.Date > NgayNhan.Value.Date;
  }
     }

        /// <summary>
 /// Số giờ còn lại đến check-in
        /// </summary>
    public int GioConLai
        {
     get
    {
        if (!NgayNhan.HasValue) return 0;
    TimeSpan diff = NgayNhan.Value - DateTime.Now;
         return (int)diff.TotalHours;
            }
      }

    /// <summary>
        /// Trạng thái badge color
        /// </summary>
      public string BadgeColor
        {
        get
        {
       if (QuaHanCheckIn) return "#dc3545"; // Đỏ
                if (CheckInHomNay) return "#28a745"; // Xanh lá
       return "#ffc107"; // Vàng
            }
     }

        /// <summary>
      /// Trạng thái text
        /// </summary>
        public string BadgeText
        {
       get
{
      if (QuaHanCheckIn) return "Quá hạn";
    if (CheckInHomNay) return "Hôm nay";
    return "Sắp tới";
  }
        }

        /// <summary>
      /// Đã thanh toán online?
    /// </summary>
        public bool DaThanhToanOnline
        {
         get
    {
      return LaDonOnline && OnlinePaymentStatus == "PAID";
    }
    }
    }
}
