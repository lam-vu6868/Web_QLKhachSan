namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.KhachHang
{
    /// <summary>
    /// ViewModel cho thống kê khách hàng
    /// </summary>
    public class KhachHangStatViewModel
    {
        /// <summary>
        /// Tổng số khách hàng
        /// </summary>
        public int TongSoKhachHang { get; set; }

        /// <summary>
        /// Số khách hàng có tài khoản
        /// </summary>
        public int SoKhachHangCoTaiKhoan { get; set; }

        /// <summary>
        /// Số khách hàng VIP (tổng tiền >= 10 triệu)
        /// </summary>
        public int SoKhachHangVIP { get; set; }

        /// <summary>
        /// Số khách hàng nam
        /// </summary>
        public int SoKhachHangNam { get; set; }

        /// <summary>
        /// Số khách hàng nữ
        /// </summary>
        public int SoKhachHangNu { get; set; }

        /// <summary>
        /// Tổng doanh thu từ khách hàng
        /// </summary>
        public decimal TongDoanhThu { get; set; }

        /// <summary>
        /// Số khách hàng mới trong tháng này
        /// </summary>
        public int SoKhachHangMoiThangNay { get; set; }
    }
}

