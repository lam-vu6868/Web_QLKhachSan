using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho danh sách đơn đặt phòng (Index page)
    /// </summary>
    public class DatPhongListViewModel
    {
        public DatPhongListViewModel()
        {
       DanhSachDatPhong = new List<DatPhongItemViewModel>();
            Filter = new DatPhongFilterViewModel();
        }

        /// <summary>
        /// Danh sách đơn đặt phòng
 /// </summary>
        public List<DatPhongItemViewModel> DanhSachDatPhong { get; set; }

        /// <summary>
        /// Bộ lọc
  /// </summary>
        public DatPhongFilterViewModel Filter { get; set; }

        /// <summary>
        /// Thống kê nhanh
        /// </summary>
        public DatPhongStatViewModel ThongKe { get; set; }

     // ===== PAGINATION =====
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
  public bool HasNextPage => CurrentPage < TotalPages;
    }

    /// <summary>
    /// ViewModel cho từng item trong danh sách
    /// </summary>
    public class DatPhongItemViewModel
    {
        public int DatPhongId { get; set; }

    [Display(Name = "Mã đặt phòng")]
      public string MaDatPhong { get; set; }

        [Display(Name = "Ngày đặt")]
    public DateTime NgayDat { get; set; }

        [Display(Name = "Ngày nhận phòng")]
        public DateTime? NgayNhan { get; set; }

        [Display(Name = "Ngày trả phòng")]
        public DateTime? NgayTra { get; set; }

        [Display(Name = "Số đêm")]
      public int? SoDem { get; set; }

        // ===== THÔNG TIN KHÁCH =====
        public int? MaKhachHang { get; set; }

   [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

        [Display(Name = "Số điện thoại")]
      public string SoDienThoai { get; set; }

        public string Email { get; set; }

        // ===== THÔNG TIN PHÒNG =====
        [Display(Name = "Số lượng phòng")]
     public int SoLuongPhong { get; set; }

[Display(Name = "Loại phòng")]
        public string DanhSachLoaiPhong { get; set; } // "Deluxe x2, Suite x1"

        [Display(Name = "Phòng đã gán")]
public string DanhSachPhong { get; set; } // "P101, P102" hoặc "Chưa gán"

        // ===== TRẠNG THÁI =====
        [Display(Name = "Trạng thái đặt phòng")]
public byte TrangThaiDatPhong { get; set; }

    [Display(Name = "Trạng thái thanh toán")]
        public byte TrangThaiThanhToan { get; set; }

        /// <summary>
        /// Text trạng thái đặt phòng
     /// 0=Chờ xác nhận, 1=Đã xác nhận, 2=Đã check-in, 3=Đã check-out, 4=Đã hủy
        /// </summary>
 public string TrangThaiDatPhongText
        {
    get
        {
         switch (TrangThaiDatPhong)
         {
      case 0: return "Chờ xác nhận";
                 case 1: return "Đã xác nhận";
       case 2: return "Đã check-in";
         case 3: return "Đã check-out";
  case 4: return "Đã hủy";
         default: return "Không xác định";
            }
            }
      }

 /// <summary>
     /// Màu badge trạng thái đặt phòng
   /// </summary>
 public string TrangThaiDatPhongColor
        {
   get
   {
       switch (TrangThaiDatPhong)
      {
        case 0: return "#ffc107"; // Vàng - Chờ xác nhận
      case 1: return "#17a2b8"; // Xanh dương - Đã xác nhận
    case 2: return "#28a745"; // Xanh lá - Đã check-in
             case 3: return "#6c757d"; // Xám - Đã check-out
   case 4: return "#dc3545"; // Đỏ - Đã hủy
                    default: return "#343a40";
 }
  }
        }

    /// <summary>
        /// Text trạng thái thanh toán
/// 0=Chưa thanh toán, 1=Đã thanh toán một phần, 2=Đã thanh toán đầy đủ
      /// </summary>
        public string TrangThaiThanhToanText
        {
        get
            {
         switch (TrangThaiThanhToan)
   {
  case 0: return "Chưa thanh toán";
        case 1: return "Đã thanh toán một phần";
    case 2: return "Đã thanh toán đầy đủ";
     default: return "Không xác định";
     }
            }
        }

        /// <summary>
        /// Màu badge trạng thái thanh toán
  /// </summary>
        public string TrangThaiThanhToanColor
      {
            get
         {
       switch (TrangThaiThanhToan)
          {
   case 0: return "#dc3545"; // Đỏ - Chưa thanh toán
          case 1: return "#ffc107"; // Vàng - Thanh toán một phần
       case 2: return "#28a745"; // Xanh lá - Thanh toán đầy đủ
      default: return "#6c757d";
    }
            }
        }

      // ===== TỔNG TIỀN =====
        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
    public decimal? TongTien { get; set; }

   // ===== THÔNG TIN KHÁC =====
        [Display(Name = "Số lượng khách")]
        public int? SoLuongKhach { get; set; }

        [Display(Name = "Ghi chú")]
  public string GhiChu { get; set; }

        [Display(Name = "Nhân viên xử lý")]
      public string TenNhanVien { get; set; }

      // ===== TIỆN ÍCH =====
     /// <summary>
        /// Còn bao nhiêu ngày đến check-in
        /// </summary>
        public int? NgayConLaiDenCheckIn
        {
            get
  {
          if (!NgayNhan.HasValue || TrangThaiDatPhong >= 2) return null;
           return (NgayNhan.Value.Date - DateTime.Now.Date).Days;
            }
        }

   /// <summary>
        /// Đã quá hạn xác nhận chưa?
        /// </summary>
  public bool CanXacNhan => TrangThaiDatPhong == 0;

        /// <summary>
        /// Có thể check-in không?
        /// </summary>
    public bool CanCheckIn => TrangThaiDatPhong == 1 && NgayNhan.HasValue && NgayNhan.Value.Date <= DateTime.Now.Date;

        /// <summary>
        /// Có thể hủy không?
        /// </summary>
    public bool CanHuy => TrangThaiDatPhong < 2; // Chỉ hủy được khi chưa check-in

        /// <summary>
    /// Check-in hôm nay?
        /// </summary>
      public bool CheckInHomNay => NgayNhan.HasValue && NgayNhan.Value.Date == DateTime.Now.Date;

 /// <summary>
   /// Check-out hôm nay?
     /// </summary>
        public bool CheckOutHomNay => NgayTra.HasValue && NgayTra.Value.Date == DateTime.Now.Date;
    }

    /// <summary>
    /// ViewModel cho bộ lọc
    /// </summary>
    public class DatPhongFilterViewModel
    {
[Display(Name = "Tìm kiếm")]
        public string TimKiem { get; set; } // Tìm theo mã đơn, tên khách, SĐT

  [Display(Name = "Trạng thái đặt phòng")]
  public byte? TrangThaiDatPhong { get; set; }

 [Display(Name = "Trạng thái thanh toán")]
   public byte? TrangThaiThanhToan { get; set; }

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

     [Display(Name = "Đến ngày")]
   [DataType(DataType.Date)]
    public DateTime? DenNgay { get; set; }

     [Display(Name = "Loại phòng")]
        public int? LoaiPhongId { get; set; }

        [Display(Name = "Nhân viên xử lý")]
     public int? NhanVienId { get; set; }

        public int PageSize { get; set; } = 10; // Đổi từ 20 → 10 items/trang
        public int CurrentPage { get; set; } = 1;
    }

  /// <summary>
    /// ViewModel thống kê nhanh
    /// </summary>
    public class DatPhongStatViewModel
    {
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

      [Display(Name = "Doanh thu dự kiến")]
        [DataType(DataType.Currency)]
  public decimal DoanhThuDuKien { get; set; }

        [Display(Name = "Đã thanh toán")]
        [DataType(DataType.Currency)]
    public decimal DaThanhToan { get; set; }
    }
}
