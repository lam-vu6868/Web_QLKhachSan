using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu
{
    /// <summary>
    /// ViewModel cho trang tạo mới dịch vụ
    /// </summary>
    public class DichVuCreateViewModel
    {
        public DichVuCreateViewModel()
        {
            DanhSachLoaiDichVu = new List<SelectListItem>();
     DanhSachIcon = new List<SelectListItem>();
          
  // Khởi tạo giá trị mặc định
            DaHoatDong = true; // Mặc định là hoạt động
   }

        // ===== THÔNG TIN CƠ BẢN =====

        [Display(Name = "Mã dịch vụ")]
        [StringLength(50, ErrorMessage = "Mã dịch vụ không được vượt quá 50 ký tự")]
        public string MaDichVu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
  [Display(Name = "Tên dịch vụ")]
      [StringLength(200, ErrorMessage = "Tên dịch vụ không được vượt quá 200 ký tự")]
        public string TenDichVu { get; set; }

        [Display(Name = "Mô tả")]
    [DataType(DataType.MultilineText)]
        public string MoTa { get; set; }

        // ===== THÔNG TIN GIÁ =====

  [Required(ErrorMessage = "Vui lòng nhập giá dịch vụ")]
 [Display(Name = "Giá gốc")]
     [Range(10000, 999999999, ErrorMessage = "Giá gốc phải từ 10,000đ đến 999,999,999đ")]
     [DataType(DataType.Currency)]
        public decimal Gia { get; set; }

        [Display(Name = "Giá ưu đãi")]
        [Range(0, 999999999, ErrorMessage = "Giá ưu đãi phải từ 0 đến 999,999,999đ")]
[DataType(DataType.Currency)]
 public decimal? GiaUuDai { get; set; }

        // ===== PHÂN LOẠI =====

    [Required(ErrorMessage = "Vui lòng chọn loại dịch vụ")]
    [Display(Name = "Loại dịch vụ")]
     public int? LoaiDichVuId { get; set; }

    // ===== TRẠNG THÁI =====

      [Display(Name = "Đang hoạt động")]
public bool DaHoatDong { get; set; }

        // ===== GIAO DIỆN =====

  [Required(ErrorMessage = "Vui lòng chọn icon cho dịch vụ")]
  [Display(Name = "Icon")]
        [StringLength(50, ErrorMessage = "Icon không được vượt quá 50 ký tự")]
        public string Icon { get; set; }

     // ===== DROPDOWN DATA =====

      /// <summary>
  /// Danh sách loại dịch vụ cho dropdown
        /// </summary>
        public List<SelectListItem> DanhSachLoaiDichVu { get; set; }

        /// <summary>
   /// Danh sách icon FontAwesome có sẵn
        /// </summary>
        public List<SelectListItem> DanhSachIcon { get; set; }

        // ===== VALIDATION LOGIC =====

     /// <summary>
/// Kiểm tra giá ưu đãi phải nhỏ hơn giá gốc
        /// </summary>
        public bool IsGiaUuDaiValid()
        {
      if (!GiaUuDai.HasValue) return true;
 return GiaUuDai.Value < Gia;
        }

        /// <summary>
        /// Tính % giảm giá (nếu có)
        /// </summary>
    public int? PhanTramGiamGia
        {
     get
            {
  if (!GiaUuDai.HasValue || Gia == 0) return null;
   if (GiaUuDai.Value >= Gia) return null;
             
   var giamGia = ((Gia - GiaUuDai.Value) / Gia) * 100;
                return (int)Math.Round(giamGia);
      }
        }

        /// <summary>
        /// Giá hiển thị cuối cùng
        /// </summary>
        public decimal GiaHienThi
     {
            get
         {
    return GiaUuDai ?? Gia;
         }
        }
    }
}
