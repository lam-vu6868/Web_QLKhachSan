using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class DichVuController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: DichVu
        public ActionResult Index()
        {
            // Load danh sách loại dịch vụ và dịch vụ của từng loại
            var loaiDichVus = db.LoaiDichVus
                .Where(l => l.DichVus.Any(d => d.DaHoatDong))
                .OrderBy(l => l.LoaiDichVuId)
                .ToList();

            return View(loaiDichVus);
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