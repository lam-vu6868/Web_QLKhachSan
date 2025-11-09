using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web_QLKhachSan.Filters
{
    /// <summary>
    /// Authorization Filter cho Nhân viên
    /// Ki?m tra Session["NhanVienId"] tr??c khi cho phép truy c?p
    /// </summary>
    public class NhanVienAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Ki?m tra Session có NhanVienId không
            var nhanVienId = httpContext.Session["NhanVienId"];
            return nhanVienId != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // L?u URL hi?n t?i ?? redirect sau khi ??ng nh?p
            var returnUrl = filterContext.HttpContext.Request.Url?.PathAndQuery;

            // ? Redirect v? trang ??ng nh?p nhân viên (s? d?ng Area routing)
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "area", "DangNhapNV" },
                    { "controller", "DangNhapNV" },
                    { "action", "DangNhap" },
                    { "returnUrl", returnUrl }
                }
            );
        }
    }
}
