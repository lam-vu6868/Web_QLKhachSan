namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhuyenMai
{
    /// <summary>
    /// ViewModel cho thống kê khuyến mãi
    /// </summary>
    public class KhuyenMaiThongKeViewModel
    {
        public int TongKhuyenMai { get; set; }
        public int DangHoatDong { get; set; }
        public int DangApDung { get; set; } // Đang trong thời gian hiệu lực
  public int SapHetHan { get; set; } // Sắp hết hạn trong 7 ngày
        public int DaHetHan { get; set; }
        public int ChuaKichHoat { get; set; }
    public int DaDung { get; set; } // Số lần đã sử dụng
    }
}
