using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_QLKhachSan.Models;

namespace Web_QLKhachSan.Controllers
{
    public class ThuVienAnhController : Controller
    {
        private DB_QLKhachSanEntities db = new DB_QLKhachSanEntities();

        // GET: ThuVienAnh
        public ActionResult Index()
        {
            // Lấy tất cả ảnh từ bảng PhongAnh
            var phongAnhs = db.PhongAnhs.Include("Phong").ToList();
            
            return View(phongAnhs);
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