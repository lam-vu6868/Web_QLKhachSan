using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class ChiTietPhongController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: ChiTietPhong - Nhận PhongId
        public ActionResult Index(int? id)
        {
            try
            {
                if (id == null)
                {
                    ViewBag.ErrorMessage = "ID is null";
                    return View(null as Phong);
                }

                ViewBag.RequestedId = id;

                var phong = db.Phongs
                    .Include("LoaiPhong")
                    .Include("LoaiPhong.TienIches")
                    .Include("LoaiPhong.TienNghis")
                    .Include("LoaiPhong.TienIches.LoaiTienIch")
                    .Include("PhongAnhs")
                    .FirstOrDefault(p => p.PhongId == id);

                if (phong == null)
                {
                    ViewBag.ErrorMessage = $"Không tìm thấy Phong với ID = {id}";
                    ViewBag.TotalPhongs = db.Phongs.Count();
                    var allPhongIds = db.Phongs.Select(p => p.PhongId).ToList();
                    ViewBag.AllIds = string.Join(", ", allPhongIds);
                    return View(null as Phong);
                }

                return View(phong);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Exception: " + ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(null as Phong);
            }
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