using System.Collections.Generic;
using System.Linq;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DichVu
{
    /// <summary>
    /// ViewModel cho trang danh sách dịch vụ
    /// </summary>
    public class DichVuListViewModel
    {
        public DichVuListViewModel()
        {
  Filter = new DichVuFilterViewModel();
            DanhSachDichVu = new List<DichVuItemViewModel>();
            ThongKe = new DichVuThongKeViewModel();
   }

        // === BỘ LỌC ===
        public DichVuFilterViewModel Filter { get; set; }

 // === DANH SÁCH DỊCH VỤ ===
        public List<DichVuItemViewModel> DanhSachDichVu { get; set; }

        // === THỐNG KÊ ===
        public DichVuThongKeViewModel ThongKe { get; set; }

     // === PHÂN TRANG ===
        public int CurrentPage { get; set; } = 1;
      public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages
 {
      get
            {
        if (TotalRecords == 0) return 1;
         return (int)System.Math.Ceiling((double)TotalRecords / PageSize);
            }
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // === DANH SÁCH LOẠI DỊCH VỤ (cho dropdown filter) ===
        public List<LoaiDichVuItemViewModel> DanhSachLoaiDichVu { get; set; }
    }

    /// <summary>
  /// Thống kê dịch vụ
    /// </summary>
    public class DichVuThongKeViewModel
    {
   public int TongDichVu { get; set; }
        public int DangHoatDong { get; set; }
        public int NgungHoatDong { get; set; }
   public int DangGiamGia { get; set; }
  public decimal DoanhThuDuKien { get; set; }
    }

    /// <summary>
    /// ViewModel cho loại dịch vụ (dùng cho dropdown)
    /// </summary>
    public class LoaiDichVuItemViewModel
    {
        public int LoaiDichVuId { get; set; }
 public string TenLoai { get; set; }
public string MoTa { get; set; }
        public string Icon { get; set; }
        public int SoLuongDichVu { get; set; }
    }
}
