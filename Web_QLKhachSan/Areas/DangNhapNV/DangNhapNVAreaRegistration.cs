using System.Web.Mvc;

namespace Web_QLKhachSan.Areas.DangNhapNV
{
    public class DangNhapNVAreaRegistration : AreaRegistration 
    {
    public override string AreaName 
        {
            get 
        {
    return "DangNhapNV";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
context.MapRoute(
          "DangNhapNV_default",
       "DangNhapNV/{controller}/{action}/{id}",
           new { controller = "DangNhapNV", action = "DangNhap", id = UrlParameter.Optional }
     );
        }
    }
}
