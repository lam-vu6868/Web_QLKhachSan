using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.DatPhong
{
 /// <summary>
    /// ViewModel cho trang danh sách đơn đặt phòng (Index)
    /// </summary>
    public class DatPhongListViewModel
 {
        public DatPhongListViewModel()
        {
            DanhSachDatPhong = new List<DatPhongItemViewModel>();
            Filter = new DatPhongFilterViewModel();
            ThongKe = new DatPhongStatViewModel();
        }

        // ===== DANH SÁCH ĐƠN ĐẶT PHÒNG =====
      
        [Display(Name = "Danh sách đơn đặt phòng")]
     public List<DatPhongItemViewModel> DanhSachDatPhong { get; set; }

        // ===== BỘ LỌC =====
        
      public DatPhongFilterViewModel Filter { get; set; }

        // ===== THỐNG KÊ =====
        
        public DatPhongStatViewModel ThongKe { get; set; }

        // ===== PAGINATION =====
        
        [Display(Name = "Tổng số bản ghi")]
     public int TotalRecords { get; set; }

        [Display(Name = "Trang hiện tại")]
   public int CurrentPage { get; set; }

        [Display(Name = "Số bản ghi mỗi trang")]
        public int PageSize { get; set; }

        [Display(Name = "Tổng số trang")]
        public int TotalPages { get; set; }

        /// <summary>
    /// Có trang trước không?
        /// </summary>
        public bool HasPreviousPage
  {
            get { return CurrentPage > 1; }
        }

        /// <summary>
        /// Có trang sau không?
        /// </summary>
        public bool HasNextPage
    {
    get { return CurrentPage < TotalPages; }
   }
    }
}
