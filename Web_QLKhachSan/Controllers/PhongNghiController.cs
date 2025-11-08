using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class PhongNghiController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: PhongNghi
        public ActionResult Index()
        {
            // Lấy danh sách loại phòng từ database
            var loaiPhongs = db.LoaiPhongs.Where(lp => lp.NgayTao != null).ToList();
            ViewBag.LoaiPhongs = loaiPhongs;

            // Lấy danh sách tiện ích từ database
            var tienIches = db.TienIches.Where(ti => ti.NgayTao != null).ToList();
            ViewBag.TienIches = tienIches;

            // Lấy danh sách phòng với thông tin liên quan
            var phongs = db.Phongs.Include("LoaiPhong")
                                  .Include("PhongAnhs")
                                  .Where(p => p.DaHoatDong == true)
                                  .ToList();
            
            return View(phongs);
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