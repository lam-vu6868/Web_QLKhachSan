using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho trang chi ti?t hóa ??n
    /// </summary>
    public class HoaDonDetailsViewModel
    {
   // ===== THÔNG TIN HÓA ??N =====
   public int HoaDonId { get; set; }

 [Display(Name = "Mã hóa ??n")]
 public string MaHoaDon { get; set; }

  [Display(Name = "Ngày l?p")]
  [DataType(DataType.DateTime)]
   public DateTime NgayLap { get; set; }

        [Display(Name = "T?ng ti?n")]
  [DataType(DataType.Currency)]
        public decimal? TongTien { get; set; }

  [Display(Name = "Gi?m giá")]
   [DataType(DataType.Currency)]
  public decimal? GiamGia { get; set; }

 [Display(Name = "Thu?")]
    [DataType(DataType.Currency)]
        public decimal? Thue { get; set; }

   [Display(Name = "Thanh toán cu?i")]
 [DataType(DataType.Currency)]
  public decimal? ThanhToanCuoi { get; set; }

        [Display(Name = "Tr?ng thái")]
        public byte TrangThaiHoaDon { get; set; }

  public string TrangThaiHoaDonText { get; set; }
 public string TrangThaiHoaDonColor { get; set; }

  [Display(Name = "Ph??ng th?c thanh toán")]
        public string PhuongThucThanhToan { get; set; }

  [Display(Name = "Ngày thanh toán")]
  [DataType(DataType.DateTime)]
        public DateTime? NgayThanhToan { get; set; }

  [Display(Name = "Ng??i l?p hóa ??n")]
   public string NguoiLapHoaDon { get; set; }

    [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        // ===== THÔNG TIN ??T PHÒNG =====
        public int? DatPhongId { get; set; }
   public string MaDatPhong { get; set; }
        public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }
     public int? SoDem { get; set; }

        // ===== THÔNG TIN KHÁCH HÀNG =====
public int? MaKhachHang { get; set; }
     public string TenKhachHang { get; set; }
        public string SoDienThoaiKhachHang { get; set; }
  public string EmailKhachHang { get; set; }
public string DiaChiKhachHang { get; set; }

        // ===== CHI TI?T PHÒNG =====
   public List<ChiTietPhongHoaDonViewModel> DanhSachPhong { get; set; }

 // ===== CHI TI?T D?CH V? =====
  public List<ChiTietDichVuHoaDonViewModel> DanhSachDichVu { get; set; }

   // ===== L?CH S? THANH TOÁN =====
public List<ThanhToanItemViewModel> LichSuThanhToan { get; set; }

    // ===== T?NG TI?N =====
    public decimal TongTienPhong { get; set; }
   public decimal TongTienDichVu { get; set; }
   
    // ? FIX: S? d?ng TongTien t? HoaDon (?ã tính ?úng khi checkout)
  public decimal TongCong => TongTien ?? ((TongTienPhong + TongTienDichVu) - (GiamGia ?? 0) + (Thue ?? 0));
    
  public decimal ConLai => TongCong - (LichSuThanhToan?.Sum(tt => tt.SoTien) ?? 0);

   // ===== TH?I GIAN =====
  public DateTime NgayTao { get; set; }

    public HoaDonDetailsViewModel()
   {
  DanhSachPhong = new List<ChiTietPhongHoaDonViewModel>();
       DanhSachDichVu = new List<ChiTietDichVuHoaDonViewModel>();
   LichSuThanhToan = new List<ThanhToanItemViewModel>();
        }
    }

    /// <summary>
  /// Chi ti?t phòng trong hóa ??n
 /// </summary>
    public class ChiTietPhongHoaDonViewModel
 {
   public string TenLoaiPhong { get; set; }
     public string MaPhong { get; set; }
   public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
   public int SoDem { get; set; }
   public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
  }

    /// <summary>
    /// Chi ti?t d?ch v? trong hóa ??n
    /// </summary>
    public class ChiTietDichVuHoaDonViewModel
  {
  public string TenDichVu { get; set; }
    public int SoLuong { get; set; }
  public decimal DonGia { get; set; }
      public decimal ThanhTien { get; set; }
   public DateTime? NgaySuDung { get; set; }
    }
}
