namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho thống kê hóa đơn
    /// </summary>
    public class HoaDonThongKeViewModel
    {
        // ===== TỔNG QUAN =====
    public int TongHoaDon { get; set; }
  public int ChuaThanhToan { get; set; }
  public int DaThanhToan { get; set; }

        // ===== DOANH THU =====
   public decimal TongDoanhThu { get; set; }
   public decimal DoanhThuHomNay { get; set; }
      public decimal DoanhThuThangNay { get; set; }

    // ===== PHƯƠNG THỨC THANH TOÁN =====
        public decimal ThanhToanTienMat { get; set; }
        public decimal ThanhToanChuyenKhoan { get; set; }
        public decimal ThanhToanOnline { get; set; }

        // ===== PHẦN TRĂM =====
  public decimal PhanTramDaThanhToan => TongHoaDon > 0 
    ? (decimal)DaThanhToan / TongHoaDon * 100 
     : 0;
    }
}
