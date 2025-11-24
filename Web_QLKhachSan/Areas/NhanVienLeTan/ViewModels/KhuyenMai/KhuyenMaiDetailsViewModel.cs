using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
    /// <summary>
    /// ViewModel cho trang chi ti?t khuy?n mãi
    /// </summary>
    public class KhuyenMaiDetailsViewModel
    {
    public int KhuyenMaiId { get; set; }

   [Display(Name = "Mã khuy?n mãi")]
    public string MaKhuyenMai { get; set; }

        [Display(Name = "Tên khuy?n mãi")]
        public string TenKhuyenMai { get; set; }

        [Display(Name = "Giá tr? gi?m (%)")]
        public decimal? GiaTri { get; set; }

        [Display(Name = "Ngày b?t ??u")]
        [DataType(DataType.Date)]
 public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Ngày k?t thúc")]
        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "?i?u ki?n áp d?ng")]
    public string DieuKienApDung { get; set; }

        [Display(Name = "S? l?n s? d?ng t?i ?a")]
        public int? SoLanSuDungToiDa { get; set; }

        [Display(Name = "S? l?n ?ã s? d?ng")]
        public int SoLanDaSuDung { get; set; }

        [Display(Name = "?ang ho?t ??ng")]
        public bool DaHoatDong { get; set; }

        // Computed Properties

  /// <summary>
        /// S? l?n còn l?i
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
        /// Tr?ng thái hi?u l?c
   /// </summary>
      public string TrangThaiHieuLuc
     {
   get
   {
            var today = DateTime.Now.Date;

              if (!NgayBatDau.HasValue || !NgayKetThuc.HasValue)
        return "Không xác ??nh";

   if (NgayBatDau.Value.Date > today)
          return "Ch?a b?t ??u";

 if (NgayKetThuc.Value.Date < today)
     return "?ã h?t h?n";

       if (NgayKetThuc.Value.Date <= today.AddDays(7))
       return "S?p h?t h?n";

       return "?ang áp d?ng";
       }
        }

        /// <summary>
        /// Màu cho tr?ng thái hi?u l?c
        /// </summary>
public string TrangThaiHieuLucColor
    {
            get
            {
    switch (TrangThaiHieuLuc)
            {
                case "?ang áp d?ng": return "#28a745"; // success - xanh lá
        case "S?p h?t h?n": return "#ffc107"; // warning - vàng
      case "?ã h?t h?n": return "#6c757d"; // secondary - xám
        case "Ch?a b?t ??u": return "#17a2b8"; // info - xanh d??ng
    default: return "#343a40"; // dark
                }
  }
        }

        /// <summary>
        /// Badge class cho tr?ng thái
        /// </summary>
        public string TrangThaiBadgeClass
        {
            get
  {
       switch (TrangThaiHieuLuc)
   {
           case "?ang áp d?ng": return "badge-success";
         case "S?p h?t h?n": return "badge-warning";
          case "?ã h?t h?n": return "badge-secondary";
       case "Ch?a b?t ??u": return "badge-info";
   default: return "badge-dark";
         }
        }
        }

        /// <summary>
        /// Có th? s? d?ng không?
        /// </summary>
  public bool CoTheSuDung
        {
            get
            {
   if (!DaHoatDong) return false;
          if (TrangThaiHieuLuc != "?ang áp d?ng") return false;
   if (SoLanSuDungToiDa.HasValue && SoLanDaSuDung >= SoLanSuDungToiDa.Value) return false;
                return true;
            }
  }

        /// <summary>
        /// S? ngày còn l?i
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

    /// <summary>
        /// Ph?n tr?m s? d?ng
  /// </summary>
        public decimal? PhanTramSuDung
        {
            get
   {
      if (!SoLanSuDungToiDa.HasValue || SoLanSuDungToiDa.Value == 0) return null;
         return (decimal)SoLanDaSuDung / SoLanSuDungToiDa.Value * 100;
            }
   }
    }
}
