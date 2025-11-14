using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web_QLKhachSan
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Default route: specify the root controllers' namespace to avoid
            // ambiguity with controllers defined inside Areas (e.g. Areas.NhanVienLeTan)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Web_QLKhachSan.Controllers" }
            );
        }
    }
}
