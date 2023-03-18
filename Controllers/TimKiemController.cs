using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;

namespace Sneaker.Controllers
{
    public class TimKiemController : Controller
    {
        QLBanGiayEntities db = new QLBanGiayEntities();


        // GET: TimKiem
        public ActionResult KQTimKiem( string search)
        {
            var lstsp = db.SANPHAMs.Where(n => n.TENSP.Contains(search));
            return View(lstsp.OrderBy(n=>n.TENSP));
        }
    }
}