using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;

namespace Sneaker.Controllers
{
    public class LienHeController : Controller
    {
        QLBanGiayEntities db = new QLBanGiayEntities();


        // GET: LienHe
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
         public ActionResult Index(FormCollection collection, LIENHE LH)
        {
           // var ma = collection["MA"];
            var hoten = collection["HoTenKh"];
            var email = collection["Email"];
            var chude = collection["ChuDe"];
            var noidung = collection["NoiDung"];
             if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên Không được để trống";
            }
           if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi2"] = "Email Không được để trống";
            }
            if (String.IsNullOrEmpty(chude))
            {
                ViewData["Loi3"] = "Chủ đề Không được để trống";
            }
            if (String.IsNullOrEmpty(noidung))
            {
                ViewData["Loi4"] = "Nội dung Không được để trống";
            }
            else
            {
                //LH.MALIENHE = ma;
                LH.HOTEN = hoten;
                LH.EMAIL = email;
                LH.CHUDE = chude;
                LH.NOIDUNG = noidung;

                db.LIENHEs.Add(LH);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Site");

        }
    }
}