using System.Collections.Generic;

namespace Web_QLKhachSan.Models
{
    public class DichVuDatThemViewModel
    {
        // Thông tin đặt phòng từ bước trước
        public ThongTinDatPhongViewModel ThongTinDatPhong { get; set; }

        // Danh sách loại dịch vụ và dịch vụ
        public List<LoaiDichVu> DanhSachLoaiDichVu { get; set; }

        public DichVuDatThemViewModel()
        {
            DanhSachLoaiDichVu = new List<LoaiDichVu>();
        }
    }
}
