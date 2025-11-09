using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels
{
    /// <summary>
    /// ViewModel chính cho trang Sơ đồ Phòng
    /// </summary>
    public class SoDoPhongViewModel
    {
        public SoDoPhongViewModel()
   {
       DanhSachPhong = new List<PhongStatusViewModel>();
        }
        // ===== DANH SÁCH PHÒNG =====
  public List<PhongStatusViewModel> DanhSachPhong { get; set; }

  // ===== THỐNG KÊ TỔNG QUAN =====
        [Display(Name = "Tổng số phòng")]
        public int TongSoPhong
    {
            get { return DanhSachPhong?.Count ?? 0; }
        }

        [Display(Name = "Phòng trống")]
        public int SoPhongTrong
        {
        get { return DanhSachPhong?.Count(p => p.TrangThaiPhong == 0) ?? 0; }
        }

        [Display(Name = "Đã đặt")]
        public int SoPhongDaDat
        {
     get { return DanhSachPhong?.Count(p => p.TrangThaiPhong == 1) ?? 0; }
        }

        [Display(Name = "Đang sử dụng")]
   public int SoPhongDangO
        {
   get { return DanhSachPhong?.Count(p => p.TrangThaiPhong == 2) ?? 0; }
        }

        [Display(Name = "Đang dọn dẹp")]
      public int SoPhongDangDon
     {
            get { return DanhSachPhong?.Count(p => p.TrangThaiPhong == 3) ?? 0; }
}

        [Display(Name = "Bảo trì")]
     public int SoPhongBaoTri
        {
            get { return DanhSachPhong?.Count(p => p.TrangThaiPhong == 4) ?? 0; }
        }

        /// <summary>
        /// Tỷ lệ lấp đầy phòng (%)
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

        // ===== PHÒNG THEO TẦNG =====
        /// <summary>
        /// Group phòng theo tầng để hiển thị dễ hơn
     /// </summary>
        public Dictionary<int, List<PhongStatusViewModel>> PhongTheoTang
        {
            get
            {
      if (DanhSachPhong == null || !DanhSachPhong.Any())
       return new Dictionary<int, List<PhongStatusViewModel>>();

     return DanhSachPhong
          .GroupBy(p => p.Tang)
         .OrderBy(g => g.Key)
    .ToDictionary(g => g.Key, g => g.OrderBy(p => p.MaPhong).ToList());
          }
        }

        /// <summary>
        /// Danh sách các tầng (để render dropdown filter)
        /// </summary>
        public List<int> DanhSachTang
        {
get
    {
                return DanhSachPhong?
         .Select(p => p.Tang)
         .Distinct()
   .OrderBy(t => t)
   .ToList() ?? new List<int>();
    }
}

    // ===== FILTERS =====
        [Display(Name = "Lọc theo tầng")]
        public int? LocTheoTang { get; set; }

        [Display(Name = "Lọc theo trạng thái")]
   public byte? LocTheoTrangThai { get; set; }

        [Display(Name = "Lọc theo loại phòng")]
        public int? LocTheoLoaiPhong { get; set; }

    [Display(Name = "Tìm kiếm")]
    public string TimKiem { get; set; }

     // ===== DANH SÁCH CHECK-IN/OUT HÔM NAY =====
        /// <summary>
        /// Danh sách phòng check-out hôm nay
        /// </summary>
        public List<PhongStatusViewModel> PhongCheckOutHomNay
        {
            get
            {
        return DanhSachPhong?
   .Where(p => p.CheckOutHomNay)
              .OrderBy(p => p.MaPhong)
      .ToList() ?? new List<PhongStatusViewModel>();
          }
     }

  /// <summary>
        /// Danh sách phòng check-in hôm nay
      /// </summary>
        public List<PhongStatusViewModel> PhongCheckInHomNay
        {
       get
            {
     return DanhSachPhong?
     .Where(p => p.CheckInHomNay)
    .OrderBy(p => p.MaPhong)
  .ToList() ?? new List<PhongStatusViewModel>();
  }
        }

        // ===== DANH SÁCH LOẠI PHÒNG (CHO FILTER) =====
        public List<LoaiPhongDropdownViewModel> DanhSachLoaiPhong { get; set; }
    }

    /// <summary>
    /// ViewModel nhỏ cho dropdown loại phòng
    /// </summary>
    public class LoaiPhongDropdownViewModel
    {
        public int LoaiPhongId { get; set; }
public string TenLoai { get; set; }
        public int SoPhong { get; set; }
    }
}
