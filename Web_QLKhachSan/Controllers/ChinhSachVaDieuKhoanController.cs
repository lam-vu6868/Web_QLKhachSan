using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_QLKhachSan.Controllers
{
    public class ChinhSachVaDieuKhoanController : Controller
    {
        // GET: ChinhSachVaDieuKhoan
        public ActionResult Index()
        {
            ViewBag.Title = "Điều Khoản Sử Dụng và Chính Sách Bảo Mật";
            ViewBag.ActiveTab = "terms"; // Default to show terms first
            return View();
        }

        // GET: ChinhSachVaDieuKhoan/DieuKhoan
        public ActionResult DieuKhoan()
        {
            ViewBag.Title = "Điều Khoản Sử Dụng";
            ViewBag.ActiveTab = "terms";
            return View("Index");
        }

        // GET: ChinhSachVaDieuKhoan/ChinhSach
        public ActionResult ChinhSach()
        {
            ViewBag.Title = "Chính Sách Bảo Mật";
            ViewBag.ActiveTab = "privacy";
            return View("Index");
        }
    }
}