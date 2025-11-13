using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho trang chi tiết đơn đặt phòng
    /// </summary>
    public class DatPhongDetailViewModel
    {
    public DatPhongDetailViewModel()
        {
            DanhSachPhong = new List<PhongDetailItemViewModel>();
   DanhSachDichVu = new List<DichVuDetailItemViewModel>();
        LichSuThayDoi = new List<LichSuItemViewModel>();
  }

  // ===== THÔNG TIN ĐƠN ĐẶT =====
   public int DatPhongId { get; set; }

        [Display(Name = "Mã đặt phòng")]
    public string MaDatPhong { get; set; }

 [Display(Name = "Ngày đặt")]
 public DateTime NgayDat { get; set; }

     [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Ngày cập nhật")]
  public DateTime? NgayCapNhat { get; set; }

     // ===== TRẠNG THÁI =====
    public byte TrangThaiDatPhong { get; set; }
        public byte TrangThaiThanhToan { get; set; }

 public string TrangThaiDatPhongText { get; set; }
public string TrangThaiDatPhongColor { get; set; }
        public string TrangThaiThanhToanText { get; set; }
   public string TrangThaiThanhToanColor { get; set; }

        // ===== TIMELINE TRẠNG THÁI =====
        public DateTime? NgayTaoDon { get; set; }
        public DateTime? NgayXacNhan { get; set; }
   public DateTime? NgayCheckIn { get; set; }
        public DateTime? NgayCheckOut { get; set; }

   // ===== THÔNG TIN KHÁCH HÀNG =====
      public int? MaKhachHang { get; set; }

  [Display(Name = "Họ và tên")]
        public string HoVaTen { get; set; }

      [Display(Name = "Số điện thoại")]
     public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
 public string Email { get; set; }

        [Display(Name = "CCCD/Passport")]
   public string CCCD { get; set; }

        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
     public DateTime? NgaySinh { get; set; }

   [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Hạng khách hàng")]
        public string HangKhachHang { get; set; } // VIP, Thường, Mới

        // ===== THÔNG TIN ĐẶT PHÒNG =====
        [Display(Name = "Ngày nhận phòng")]
        public DateTime? NgayNhan { get; set; }

        [Display(Name = "Ngày trả phòng")]
   public DateTime? NgayTra { get; set; }

        [Display(Name = "Số đêm")]
        public int? SoDem { get; set; }

        [Display(Name = "Số lượng khách")]
        public int? SoLuongKhach { get; set; }

        [Display(Name = "Ghi chú")]
public string GhiChu { get; set; }

        // ===== DANH SÁCH PHÒNG =====
        [Display(Name = "Danh sách phòng")]
 public List<PhongDetailItemViewModel> DanhSachPhong { get; set; }

      // ===== DANH SÁCH DỊCH VỤ =====
        [Display(Name = "Danh sách dịch vụ")]
        public List<DichVuDetailItemViewModel> DanhSachDichVu { get; set; }

   // ===== THÔNG TIN THANH TOÁN =====
        [Display(Name = "Tổng tiền phòng")]
      [DataType(DataType.Currency)]
 public decimal TongTienPhong { get; set; }

        [Display(Name = "Tổng tiền dịch vụ")]
        [DataType(DataType.Currency)]
      public decimal TongTienDichVu { get; set; }

        [Display(Name = "Giảm giá")]
        [DataType(DataType.Currency)]
   public decimal TienGiamGia { get; set; }

        [Display(Name = "Thuế VAT")]
        [DataType(DataType.Currency)]
   public decimal ThueVAT { get; set; }

        [Display(Name = "Tổng cộng")]
    [DataType(DataType.Currency)]
      public decimal TongCong { get; set; }

        [Display(Name = "Đã thanh toán")]
        [DataType(DataType.Currency)]
public decimal DaThanhToan { get; set; }

        [Display(Name = "Còn phải thanh toán")]
[DataType(DataType.Currency)]
        public decimal ConPhaiThanhToan { get; set; }

        [Display(Name = "Hình thức thanh toán")]
   public string HinhThucThanhToan { get; set; }

        // ===== KHUYẾN MÃI =====
        [Display(Name = "Mã khuyến mãi")]
        public string MaKhuyenMai { get; set; }

        [Display(Name = "Tên khuyến mãi")]
        public string TenKhuyenMai { get; set; }

    // ===== NHÂN VIÊN =====
   [Display(Name = "Nhân viên tạo đơn")]
        public string NhanVienTaoDon { get; set; }

        [Display(Name = "Nhân viên xác nhận")]
        public string NhanVienXacNhan { get; set; }

      [Display(Name = "Nhân viên check-in")]
   public string NhanVienCheckIn { get; set; }

        [Display(Name = "Nhân viên check-out")]
 public string NhanVienCheckOut { get; set; }

        // ===== LỊCH SỬ THAY ĐỔI =====
        public List<LichSuItemViewModel> LichSuThayDoi { get; set; }

  // ===== ACTIONS =====
        /// <summary>
        /// Có thể xác nhận không?
        /// </summary>
  public bool CanXacNhan => TrangThaiDatPhong == 0;

        /// <summary>
        /// Có thể check-in không?
        /// </summary>
    public bool CanCheckIn => TrangThaiDatPhong == 1 && NgayNhan.HasValue && NgayNhan.Value.Date <= DateTime.Now.Date;

     /// <summary>
        /// Có thể check-out không?
        /// </summary>
        public bool CanCheckOut => TrangThaiDatPhong == 2;

     /// <summary>
/// Có thể hủy không?
        /// </summary>
        public bool CanHuy => TrangThaiDatPhong < 2;

  /// <summary>
     /// Có thể sửa không?
        /// </summary>
        public bool CanEdit => TrangThaiDatPhong < 2;

        /// <summary>
     /// Có thể in không?
        /// </summary>
     public bool CanPrint => true;

  /// <summary>
        /// Quá hạn check-in?
  /// </summary>
        public bool QuaHanCheckIn => TrangThaiDatPhong == 1 && NgayNhan.HasValue && DateTime.Now > NgayNhan.Value.AddDays(1);
    }

/// <summary>
    /// ViewModel cho từng phòng trong đơn
    /// </summary>
    public class PhongDetailItemViewModel
    {
        public int ChiTietDatPhongId { get; set; }

  [Display(Name = "Loại phòng")]
        public string TenLoaiPhong { get; set; }

        [Display(Name = "Phòng")]
        public string MaPhong { get; set; } // "P101" hoặc "Chưa gán"

 public int? PhongId { get; set; }

        [Display(Name = "Ngày nhận")]
        [DataType(DataType.Date)]
 public DateTime? NgayDen { get; set; }

        [Display(Name = "Ngày trả")]
        [DataType(DataType.Date)]
        public DateTime? NgayDi { get; set; }

        [Display(Name = "Số đêm")]
        public int SoDem
        {
       get
            {
   if (NgayDi.HasValue && NgayDen.HasValue)
       return (NgayDi.Value - NgayDen.Value).Days;
       return 0;
     }
      }

        [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
  public decimal? DonGia { get; set; }

   [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Giảm giá")]
        [DataType(DataType.Currency)]
public decimal? GiamGia { get; set; }

     [Display(Name = "Thành tiền")]
   [DataType(DataType.Currency)]
        public decimal? ThanhTien { get; set; }

      [Display(Name = "Trạng thái")]
        public byte TrangThaiPhong { get; set; }

        public string TrangThaiPhongText { get; set; }
    public string TrangThaiPhongColor { get; set; }

   [Display(Name = "Ghi chú")]
    public string GhiChu { get; set; }
    }

    /// <summary>
    /// ViewModel cho dịch vụ trong đơn
    /// </summary>
    public class DichVuDetailItemViewModel
    {
        public int ChiTietDatDichVuId { get; set; }

        [Display(Name = "Dịch vụ")]
        public string TenDichVu { get; set; }

        [Display(Name = "Ngày sử dụng")]
        [DataType(DataType.Date)]
 public DateTime? NgaySuDung { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

 [Display(Name = "Đơn giá")]
        [DataType(DataType.Currency)]
        public decimal? DonGia { get; set; }

        [Display(Name = "Thành tiền")]
        [DataType(DataType.Currency)]
 public decimal? ThanhTien { get; set; }

        [Display(Name = "Ghi chú")]
     public string GhiChu { get; set; }
}

    /// <summary>
    /// ViewModel cho lịch sử thay đổi
 /// </summary>
    public class LichSuItemViewModel
    {
  [Display(Name = "Thời gian")]
        public DateTime ThoiGian { get; set; }

        [Display(Name = "Người thực hiện")]
        public string NguoiThucHien { get; set; }

     [Display(Name = "Hành động")]
        public string HanhDong { get; set; } // "Tạo đơn", "Xác nhận", "Check-in", "Check-out", "Hủy"

    [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}
