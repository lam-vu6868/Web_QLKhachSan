using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu
{
    /// <summary>
    /// ViewModel đại diện cho một dịch vụ trong danh sách
  /// </summary>
    public class DichVuItemViewModel
    {
        public int DichVuId { get; set; }

        [Display(Name = "Mã dịch vụ")]
        public string MaDichVu { get; set; }

   [Display(Name = "Tên dịch vụ")]
        public string TenDichVu { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Giá")]
        [DataType(DataType.Currency)]
        public decimal? Gia { get; set; }

        [Display(Name = "Giá ưu đãi")]
        [DataType(DataType.Currency)]
    public decimal? GiaUuDai { get; set; }

  [Display(Name = "Đang hoạt động")]
     public bool DaHoatDong { get; set; }

     [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime NgayTao { get; set; }

  [Display(Name = "Ngày cập nhật")]
 [DataType(DataType.DateTime)]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Loại dịch vụ")]
        public string TenLoaiDichVu { get; set; }

        public int? LoaiDichVuId { get; set; }

        [Display(Name = "Icon")]
     public string Icon { get; set; }

        // === COMPUTED PROPERTIES ===

        /// <summary>
   /// Giá hiển thị (ưu tiên giá ưu đãi nếu có)
        /// </summary>
        public decimal GiaHienThi
        {
            get
  {
          return GiaUuDai ?? Gia ?? 0;
            }
        }

        /// <summary>
        /// Có đang giảm giá không?
        /// </summary>
        public bool DangGiamGia
      {
            get
 {
    return GiaUuDai.HasValue && Gia.HasValue && GiaUuDai < Gia;
            }
        }

        /// <summary>
        /// Phần trăm giảm giá
        /// </summary>
        public int PhanTramGiamGia
    {
    get
{
  if (!GiaUuDai.HasValue || !Gia.HasValue || Gia.Value == 0) return 0;
    if (GiaUuDai.Value >= Gia.Value) return 0;
   
      var giamGia = ((Gia.Value - GiaUuDai.Value) / Gia.Value) * 100;
 return (int)Math.Round(giamGia);
   }
        }

        /// <summary>
        /// Trạng thái hiển thị
        /// </summary>
        public string TrangThaiText
        {
            get
        {
      return DaHoatDong ? "Đang hoạt động" : "Ngừng hoạt động";
}
        }

        /// <summary>
        /// Màu trạng thái
        /// </summary>
        public string TrangThaiColor
        {
     get
       {
return DaHoatDong ? "#28a745" : "#dc3545";
    }
        }
    }
}
