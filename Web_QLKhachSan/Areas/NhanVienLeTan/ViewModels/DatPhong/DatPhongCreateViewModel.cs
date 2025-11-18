using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
    /// <summary>
    /// ViewModel cho form tạo đơn đặt phòng mới
    /// Nhân viên lễ tân sử dụng để nhập thông tin đặt phòng cho khách
    /// </summary>
    public class DatPhongCreateViewModel
    {
        public DatPhongCreateViewModel()
        {
     DanhSachPhongDat = new List<PhongDatItemViewModel>();
     NgayNhan = DateTime.Now.AddDays(1);
            NgayTra = DateTime.Now.AddDays(2);
        }

      // ===== THÔNG TIN KHÁCH HÀNG =====
      
      [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
  [Display(Name = "Họ và tên")]
        [StringLength(100)]
   public string HoVaTen { get; set; }

     [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
   [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Địa chỉ")]
    [StringLength(255)]
        public string DiaChi { get; set; }

   [Display(Name = "Giới tính")]
      public byte? GioiTinh { get; set; }

        // ===== THÔNG TIN ĐẶT PHÒNG =====
    
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        [Display(Name = "Ngày nhận phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayNhan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
  [Display(Name = "Ngày trả phòng")]
        [DataType(DataType.Date)]
        public DateTime NgayTra { get; set; }

 [Required(ErrorMessage = "Vui lòng nhập số lượng khách")]
        [Range(1, 50, ErrorMessage = "Số lượng khách từ 1 đến 50")]
        [Display(Name = "Số lượng khách")]
     public int SoLuongKhach { get; set; }

        [Display(Name = "Ghi chú")]
   [StringLength(500)]
    public string GhiChu { get; set; }

        // ===== DANH SÁCH PHÒNG ĐẶT =====
    
        [Display(Name = "Danh sách phòng đặt")]
 public List<PhongDatItemViewModel> DanhSachPhongDat { get; set; }

        // ===== THANH TOÁN =====
        
        [Display(Name = "Hình thức thanh toán")]
        public byte HinhThucThanhToan { get; set; }

        [Display(Name = "Mã khuyến mãi")]
  public int? MaKhuyenMaiId { get; set; }

        [Display(Name = "Tầng ưu tiên")]
        public int? TangUuTien { get; set; }

  // ===== COMPUTED PROPERTIES =====

 /// <summary>
        /// Số đêm = NgayTra - NgayNhan
     /// </summary>
        public int SoDem
        {
      get
            {
      if (NgayTra > NgayNhan)
        return (NgayTra - NgayNhan).Days;
     return 0;
          }
}

        /// <summary>
        /// Tổng tiền phòng
        /// </summary>
 public decimal TongTienPhong
    {
      get
     {
          return DanhSachPhongDat?.Sum(p => p.ThanhTien) ?? 0;
         }
        }

        /// <summary>
  /// Tổng tiền sau giảm giá
        /// </summary>
   public decimal TongCong
  {
            get
            {
        decimal tongTien = TongTienPhong;
           // Có thể thêm logic giảm giá từ mã khuyến mãi ở đây
        return tongTien;
            }
        }

        /// <summary>
   /// Tổng số phòng đã chọn
   /// </summary>
        public int TongSoPhong
      {
            get
          {
    return DanhSachPhongDat?.Sum(p => p.SoLuong) ?? 0;
    }
        }

   // ===== VALIDATION METHODS =====

        /// <summary>
        /// Kiểm tra ngày nhận phải trước ngày trả
      /// </summary>
     public bool IsValidDateRange()
        {
 return NgayTra > NgayNhan;
        }

/// <summary>
        /// Kiểm tra đã chọn ít nhất 1 phòng
        /// </summary>
        public bool HasRooms()
        {
            return DanhSachPhongDat != null && DanhSachPhongDat.Any(p => p.SoLuong > 0);
        }

 /// <summary>
      /// Kiểm tra ngày nhận không được là quá khứ
        /// </summary>
        public bool IsValidCheckInDate()
 {
       return NgayNhan.Date >= DateTime.Now.Date;
        }
    }
}
