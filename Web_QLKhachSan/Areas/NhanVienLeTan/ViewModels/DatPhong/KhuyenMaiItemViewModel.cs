using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
  /// <summary>
    /// ViewModel cho thông tin khuy?n mãi trong ??n ??t phòng
 /// </summary>
    public class KhuyenMaiItemViewModel
{
    public int KhuyenMaiId { get; set; }

   [Display(Name = "Mã khuy?n mãi")]
        public string MaKhuyenMai { get; set; }

        [Display(Name = "Tên khuy?n mãi")]
  public string TenKhuyenMai { get; set; }

  [Display(Name = "Giá tr? (%)")]
        public decimal? GiaTri { get; set; }

        [Display(Name = "Ngày b?t ??u")]
        [DataType(DataType.Date)]
      public DateTime? NgayBatDau { get; set; }

  [Display(Name = "Ngày k?t thúc")]
        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        /// <summary>
        /// Hi?n th? thông tin khuy?n mãi ??y ??
      /// </summary>
     public string ThongTinDayDu
   {
        get
  {
        if (string.IsNullOrEmpty(TenKhuyenMai)) return "";
       return $"{TenKhuyenMai} - Gi?m {GiaTri}%";
      }
 }

   /// <summary>
        /// Còn hi?u l?c không?
   /// </summary>
        public bool ConHieuLuc
  {
         get
  {
    if (!NgayBatDau.HasValue || !NgayKetThuc.HasValue) return false;
        var today = DateTime.Now.Date;
      return NgayBatDau.Value.Date <= today && NgayKetThuc.Value.Date >= today;
            }
        }
    }
}
