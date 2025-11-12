using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class HomeController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

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

            // Load 4 loại dịch vụ để hiển thị trên trang chủ (bao gồm cả DichVus)
            var loaiDichVus = db.LoaiDichVus
                .OrderBy(l => l.LoaiDichVuId)
                .Take(4)
                .ToList();

            ViewBag.LoaiDichVus = loaiDichVus;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}