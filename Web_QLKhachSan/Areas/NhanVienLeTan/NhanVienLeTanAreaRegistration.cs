using System.Web.Mvc;

namespace Web_QLKhachSan.Areas.NhanVienLeTan
{
    public class NhanVienLeTanAreaRegistration : AreaRegistration 
    {
    public override string AreaName 
        {
     get 
{
    return "NhanVienLeTan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
        context.MapRoute(
            "NhanVienLeTan_default",
       "NhanVienLeTan/{controller}/{action}/{id}",
     new { action = "Index", id = UrlParameter.Optional }
          );
        }
    }
}
