using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
    /// <summary>
    /// ViewModel cho item khuyến mãi trong danh sách
 /// </summary>
    public class KhuyenMaiItemViewModel
    {
        public int KhuyenMaiId { get; set; }

        [Display(Name = "Mã khuyến mãi")]
        public string MaKhuyenMai { get; set; }

  [Display(Name = "Tên khuyến mãi")]
        public string TenKhuyenMai { get; set; }

        [Display(Name = "Giá trị (%)")]
    public decimal? GiaTri { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
 public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
   [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

   [Display(Name = "Điều kiện áp dụng")]
        public string DieuKienApDung { get; set; }

        [Display(Name = "Số lần sử dụng tối đa")]
   public int? SoLanSuDungToiDa { get; set; }

        [Display(Name = "Đã sử dụng")]
        public int SoLanDaSuDung { get; set; }

    [Display(Name = "Đang hoạt động")]
      public bool DaHoatDong { get; set; }

     // Computed Properties

        /// <summary>
        /// Số lần còn lại
   /// </summary>
  public int? SoLanConLai
  {
        get
 {
          if (!SoLanSuDungToiDa.HasValue) return null;
 return Math.Max(0, SoLanSuDungToiDa.Value - SoLanDaSuDung);
        }
  }

   /// <summary>
      /// Trạng thái hiệu lực
    /// </summary>
        public string TrangThaiHieuLuc
    {
        get
         {
         var today = DateTime.Now.Date;
          
        if (!NgayBatDau.HasValue || !NgayKetThuc.HasValue)
          return "Không xác định";

          if (NgayBatDau.Value.Date > today)
       return "Chưa bắt đầu";
         
          if (NgayKetThuc.Value.Date < today)
  return "Đã hết hạn";

         if (NgayKetThuc.Value.Date <= today.AddDays(7))
   return "Sắp hết hạn";

      return "Đang áp dụng";
     }
 }

    /// <summary>
        /// Màu cho trạng thái hiệu lực
        /// </summary>
        public string TrangThaiHieuLucColor
        {
            get
     {
      switch (TrangThaiHieuLuc)
           {
     case "Đang áp dụng": return "#28a745"; // success - xanh lá
         case "Sắp hết hạn": return "#ffc107"; // warning - vàng
           case "Đã hết hạn": return "#6c757d"; // secondary - xám
        case "Chưa bắt đầu": return "#17a2b8"; // info - xanh dương
        default: return "#343a40"; // dark
       }
  }
        }

      /// <summary>
  /// Badge class cho trạng thái
      /// </summary>
        public string TrangThaiBadgeClass
        {
     get
     {
       switch (TrangThaiHieuLuc)
       {
           case "Đang áp dụng": return "badge-success";
     case "Sắp hết hạn": return "badge-warning";
       case "Đã hết hạn": return "badge-secondary";
         case "Chưa bắt đầu": return "badge-info";
 default: return "badge-dark";
    }
            }
 }

        /// <summary>
   /// Có thể sử dụng không?
        /// </summary>
    public bool CoTheSuDung
     {
   get
         {
                if (!DaHoatDong) return false;
        if (TrangThaiHieuLuc != "Đang áp dụng") return false;
       if (SoLanSuDungToiDa.HasValue && SoLanDaSuDung >= SoLanSuDungToiDa.Value) return false;
  return true;
   }
        }

        /// <summary>
      /// Số ngày còn lại
     /// </summary>
        public int? SoNgayConLai
     {
     get
{
       if (!NgayKetThuc.HasValue) return null;
 var today = DateTime.Now.Date;
       if (NgayKetThuc.Value.Date < today) return 0;
      return (NgayKetThuc.Value.Date - today).Days;
    }
        }
    }
}
