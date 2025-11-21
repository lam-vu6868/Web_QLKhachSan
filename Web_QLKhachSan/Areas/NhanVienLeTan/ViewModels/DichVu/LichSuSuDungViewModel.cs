using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu
{
    /// <summary>
    /// ViewModel cho trang Lịch sử sử dụng dịch vụ
    /// </summary>
    public class LichSuSuDungViewModel
    {
        public LichSuSuDungViewModel()
     {
     DanhSachChiTiet = new List<ChiTietSuDungItemViewModel>();
            ThongKeTheoThang = new List<ThongKeThangItem>();
        }

        // === THÔNG TIN DỊCH VỤ ===
        public int DichVuId { get; set; }
        public string MaDichVu { get; set; }
 public string TenDichVu { get; set; }
        public string MoTa { get; set; }
        public decimal? Gia { get; set; }
        public decimal? GiaUuDai { get; set; }
   public string Icon { get; set; }

        // === DANH SÁCH CHI TIẾT SỬ DỤNG ===
        public List<ChiTietSuDungItemViewModel> DanhSachChiTiet { get; set; }

        // === THỐNG KÊ TỔNG QUAN ===
  public int TongSoLanSuDung { get; set; }
        public decimal TongDoanhThu { get; set; }
      public double SoLuongTrungBinh { get; set; }

        // === THỐNG KÊ THEO THÁNG (CHO BIỂU ĐỒ) ===
        public List<ThongKeThangItem> ThongKeTheoThang { get; set; }

        // === PHÂN TRANG ===
 public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages
        {
            get
            {
          if (TotalRecords == 0) return 1;
     return (int)Math.Ceiling((double)TotalRecords / PageSize);
            }
        }

        public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

        // === BỘ LỌC ===
  [Display(Name = "Từ ngày")]
 [DataType(DataType.Date)]
        public DateTime? TuNgay { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? DenNgay { get; set; }

        [Display(Name = "Tìm kiếm")]
  public string TimKiem { get; set; }

        [Display(Name = "Sắp xếp")]
 public string SortBy { get; set; }

        /// <summary>
        /// Có đang lọc không?
     /// </summary>
        public bool HasFilter
        {
            get
         {
     return TuNgay.HasValue ||
           DenNgay.HasValue ||
      !string.IsNullOrWhiteSpace(TimKiem);
            }
  }
    }

    /// <summary>
    /// Item chi tiết sử dụng dịch vụ
    /// </summary>
    public class ChiTietSuDungItemViewModel
    {
        public int ChiTietDatDichVuId { get; set; }
    public int DatPhongId { get; set; }
        public string MaDatPhong { get; set; }

      // Thông tin khách hàng
        public string TenKhachHang { get; set; }
        public string SoDienThoaiKhachHang { get; set; }

        // Thông tin sử dụng
        public DateTime? NgaySuDung { get; set; }
      public DateTime NgayDatPhong { get; set; }
 public int SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
   public string GhiChu { get; set; }

        /// <summary>
        /// Ngày hiển thị (ưu tiên NgaySuDung)
/// </summary>
        public DateTime NgayHienThi => NgaySuDung ?? NgayDatPhong;
    }

    /// <summary>
    /// Item thống kê theo tháng (cho biểu đồ)
    /// </summary>
    public class ThongKeThangItem
    {
        public string Thang { get; set; }
        public int SoLan { get; set; }
        public decimal DoanhThu { get; set; }
    }
}
