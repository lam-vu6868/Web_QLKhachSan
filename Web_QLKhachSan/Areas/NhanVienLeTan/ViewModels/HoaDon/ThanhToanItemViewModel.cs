using System;
using System.ComponentModel.DataAnnotations;

namespace Web_QLKhachSan.Areas.NhanVienLeTan.ViewModels.HoaDon
{
    /// <summary>
    /// ViewModel cho chi tiết thanh toán
    /// </summary>
    public class ThanhToanItemViewModel
    {
   public int ThanhToanId { get; set; }

        [Display(Name = "Ngày thanh toán")]
   [DataType(DataType.DateTime)]
  public DateTime NgayThanhToan { get; set; }

   [Display(Name = "Số tiền")]
        [DataType(DataType.Currency)]
     public decimal SoTien { get; set; }

    [Display(Name = "Phương thức")]
 public byte? PhuongThucThanhToan { get; set; }

        public string PhuongThucText { get; set; }

   [Display(Name = "Mã giao dịch")]
   public string MaGiaoDich { get; set; }

   [Display(Name = "Trạng thái")]
  public byte TrangThaiThanhToan { get; set; }

     public string TrangThaiText { get; set; }
  public string TrangThaiColor { get; set; }

  [Display(Name = "Người thực hiện")]
  public string NguoiThucHien { get; set; }

    [Display(Name = "Ghi chú")]
     public string GhiChu { get; set; }

   [Display(Name = "Hình ảnh biên lai")]
   public string HinhAnhBienLai { get; set; }
    }
}
