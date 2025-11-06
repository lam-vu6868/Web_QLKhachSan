using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_QLKhachSan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // ✅ DEBUG: Kiểm tra Session và User.Identity
            System.Diagnostics.Debug.WriteLine("=== HOME INDEX DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"User.Identity.Name: {User.Identity.Name}");
            System.Diagnostics.Debug.WriteLine($"Session[HoVaTen]: {Session["HoVaTen"]}");
            System.Diagnostics.Debug.WriteLine($"Session[MaKhachHang]: {Session["MaKhachHang"]}");
            System.Diagnostics.Debug.WriteLine($"Session[Email]: {Session["Email"]}");
            System.Diagnostics.Debug.WriteLine("========================");

            return View();
            hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
        }
    }
}