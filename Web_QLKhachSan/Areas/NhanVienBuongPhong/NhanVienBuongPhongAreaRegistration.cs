using System.Web.Mvc;

namespace Web_QLKhachSan.Areas.NhanVienBuongPhong
{
    public class NhanVienBuongPhongAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "NhanVienBuongPhong";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "NhanVienBuongPhong_default",
                "NhanVienBuongPhong/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}