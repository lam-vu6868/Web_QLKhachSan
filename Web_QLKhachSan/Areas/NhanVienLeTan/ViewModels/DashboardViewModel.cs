using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels
{
    /// <summary>
    /// ViewModel chính cho trang Dashboard Lễ Tân
    /// Tổng hợp tất cả thông tin cần thiết cho overview
    /// </summary>
    public class DashboardViewModel
    {
 public DashboardViewModel()
     {
       ThongKePhong = new ThongKePhongViewModel();
      DanhSachCheckInHomNay = new List<CheckInOutViewModel>();
      DanhSachCheckOutHomNay = new List<CheckInOutViewModel>();
         ThongBaoCanChuY = new List<ThongBaoViewModel>();
            DoanhThu7NgayGanNhat = new List<DoanhThuNgayViewModel>();
      }

     // ===== THỐNG KÊ PHÒNG =====
        public ThongKePhongViewModel ThongKePhong { get; set; }

        // ===== THỐNG KÊ ĐẶT PHÒNG =====
        [Display(Name = "Đơn đặt chờ xác nhận")]
        public int SoDonChoXacNhan { get; set; }

  [Display(Name = "Tổng đơn đặt hôm nay")]
        public int TongDonDatHomNay { get; set; }

        // ===== THỐNG KÊ DOANH THU =====
        [Display(Name = "Doanh thu hôm nay")]
    [DataType(DataType.Currency)]
        public decimal DoanhThuHomNay { get; set; }

        [Display(Name = "Doanh thu tháng này")]
  [DataType(DataType.Currency)]
        public decimal DoanhThuThangNay { get; set; }

        [Display(Name = "Số hóa đơn chưa thanh toán")]
        public int SoHoaDonChuaThanhToan { get; set; }

        // ===== CHECK-IN/OUT HÔM NAY =====
        [Display(Name = "Danh sách check-in hôm nay")]
        public List<CheckInOutViewModel> DanhSachCheckInHomNay { get; set; }

        [Display(Name = "Danh sách check-out hôm nay")]
    public List<CheckInOutViewModel> DanhSachCheckOutHomNay { get; set; }

        // ===== THÔNG BÁO CẦN CHÚ Ý =====
      [Display(Name = "Thông báo cần chú ý")]
        public List<ThongBaoViewModel> ThongBaoCanChuY { get; set; }

      // ===== BIỂU ĐỒ DOANH THU =====
        [Display(Name = "Doanh thu 7 ngày gần nhất")]
        public List<DoanhThuNgayViewModel> DoanhThu7NgayGanNhat { get; set; }

        // ===== THÔNG TIN NHÂN VIÊN =====
        [Display(Name = "Tên nhân viên")]
        public string TenNhanVien { get; set; }

        [Display(Name = "Vai trò")]
   public string VaiTro { get; set; }

        [Display(Name = "Thời gian đăng nhập")]
        public DateTime? ThoiGianDangNhap { get; set; }
    }

    /// <summary>
    /// ViewModel con - Thống kê phòng tổng hợp
    /// </summary>
    public class ThongKePhongViewModel
    {
        [Display(Name = "Tổng số phòng")]
   public int TongSoPhong { get; set; }

        [Display(Name = "Phòng trống")]
        public int SoPhongTrong { get; set; }

        [Display(Name = "Đã đặt")]
      public int SoPhongDaDat { get; set; }

        [Display(Name = "Đang ở")]
        public int SoPhongDangO { get; set; }

        [Display(Name = "Đang dọn")]
        public int SoPhongDangDon { get; set; }

        [Display(Name = "Bảo trì")]
      public int SoPhongBaoTri { get; set; }

        /// <summary>
        /// Tỷ lệ lấp đầy (%)
        /// </summary>
        [Display(Name = "Tỷ lệ lấp đầy")]
        public double TyLeLayDay
        {
     get
            {
              if (TongSoPhong == 0) return 0;
         return Math.Round((double)(SoPhongDangO + SoPhongDaDat) / TongSoPhong * 100, 1);
         }
   }

        /// <summary>
        /// Tỷ lệ phòng trống (%)
        /// </summary>
    [Display(Name = "Tỷ lệ trống")]
        public double TyLePhongTrong
        {
            get
            {
    if (TongSoPhong == 0) return 0;
       return Math.Round((double)SoPhongTrong / TongSoPhong * 100, 1);
 }
     }
    }

    /// <summary>
    /// ViewModel con - Thông tin Check-in/Check-out
    /// </summary>
    public class CheckInOutViewModel
    {
        public int DatPhongId { get; set; }

      [Display(Name = "Mã đặt phòng")]
        public string MaDatPhong { get; set; }

     [Display(Name = "Tên khách hàng")]
        public string TenKhachHang { get; set; }

        [Display(Name = "Số điện thoại")]
     public string SoDienThoai { get; set; }

     [Display(Name = "Phòng")]
        public string TenPhong { get; set; }

        [Display(Name = "Loại phòng")]
    public string LoaiPhong { get; set; }

        [Display(Name = "Ngày check-in")]
        [DataType(DataType.DateTime)]
 public DateTime? NgayCheckIn { get; set; }

        [Display(Name = "Ngày check-out")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayCheckOut { get; set; }

 [Display(Name = "Số người")]
        public int? SoNguoi { get; set; }

 [Display(Name = "Trạng thái")]
        public byte TrangThai { get; set; }

        /// <summary>
        /// Text trạng thái đặt phòng
        /// </summary>
        public string TrangThaiText
        {
            get
            {
                switch (TrangThai)
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
/// Màu sắc badge trạng thái
   /// </summary>
        public string TrangThaiColor
        {
            get
            {
  switch (TrangThai)
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

        [Display(Name = "Ghi chú")]
   public string GhiChu { get; set; }

   /// <summary>
        /// Có yêu cầu đặc biệt?
        /// </summary>
    public bool CoYeuCauDacBiet
{
    get { return !string.IsNullOrWhiteSpace(GhiChu); }
  }

        /// <summary>
        /// Thời gian còn lại đến check-in/check-out (giờ)
        /// </summary>
        public int? GioConLai
        {
            get
  {
       if (!NgayCheckIn.HasValue && !NgayCheckOut.HasValue) return null;
  
              DateTime targetTime = NgayCheckIn ?? NgayCheckOut.Value;
      TimeSpan diff = targetTime - DateTime.Now;
     return (int)diff.TotalHours;
     }
        }

  /// <summary>
   /// Đã quá giờ check-out chưa?
     /// </summary>
        public bool QuaGioCheckOut
        {
            get
 {
                if (!NgayCheckOut.HasValue) return false;
     return DateTime.Now > NgayCheckOut.Value;
 }
        }
    }

    /// <summary>
  /// ViewModel con - Thông báo cảnh báo
  /// </summary>
 public class ThongBaoViewModel
    {
      [Display(Name = "Loại thông báo")]
        public string LoaiThongBao { get; set; } // "warning", "danger", "info", "success"

        [Display(Name = "Icon")]
        public string Icon { get; set; } // Font Awesome class

     [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Liên kết")]
        public string Link { get; set; }

        [Display(Name = "Thời gian")]
   public DateTime ThoiGian { get; set; }

     /// <summary>
        /// CSS class cho badge
        /// </summary>
        public string BadgeClass
        {
  get
            {
             switch (LoaiThongBao?.ToLower())
   {
         case "warning": return "badge-warning";
             case "danger": return "badge-danger";
            case "info": return "badge-info";
        case "success": return "badge-success";
      default: return "badge-secondary";
    }
   }
        }

        /// <summary>
        /// Thời gian tương đối (VD: "5 phút trước")
        /// </summary>
    public string ThoiGianTuongDoi
        {
    get
      {
     TimeSpan diff = DateTime.Now - ThoiGian;
  
        if (diff.TotalMinutes < 1)
 return "Vừa xong";
      else if (diff.TotalMinutes < 60)
    return $"{(int)diff.TotalMinutes} phút trước";
    else if (diff.TotalHours < 24)
     return $"{(int)diff.TotalHours} giờ trước";
          else
          return $"{(int)diff.TotalDays} ngày trước";
            }
        }
    }

    /// <summary>
    /// ViewModel con - Doanh thu theo ngày (cho Chart.js)
    /// </summary>
    public class DoanhThuNgayViewModel
    {
  [Display(Name = "Ngày")]
        [DataType(DataType.Date)]
    public DateTime Ngay { get; set; }

        [Display(Name = "Doanh thu")]
  [DataType(DataType.Currency)]
        public decimal DoanhThu { get; set; }

        [Display(Name = "Số đơn")]
        public int SoDon { get; set; }

  /// <summary>
        /// Label cho Chart.js (VD: "01/01")
   /// </summary>
        public string Label
        {
    get { return Ngay.ToString("dd/MM"); }
     }

        /// <summary>
        /// Ngày trong tuần (VD: "Thứ 2")
        /// </summary>
        public string ThuTrongTuan
   {
       get
            {
         switch (Ngay.DayOfWeek)
    {
        case DayOfWeek.Monday: return "Thứ 2";
          case DayOfWeek.Tuesday: return "Thứ 3";
        case DayOfWeek.Wednesday: return "Thứ 4";
         case DayOfWeek.Thursday: return "Thứ 5";
        case DayOfWeek.Friday: return "Thứ 6";
             case DayOfWeek.Saturday: return "Thứ 7";
          case DayOfWeek.Sunday: return "Chủ nhật";
    default: return "";
     }
            }
   }
    }
}
