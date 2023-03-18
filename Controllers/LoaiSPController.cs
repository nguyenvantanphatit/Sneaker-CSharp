using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;

namespace Sneaker.Controllers
{
    public class LoaiSPController : Controller
    {
        // GET: LoaiSP
        QLBanGiayEntities db = new QLBanGiayEntities();


        // GET: NhaCC
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
                return View(db.LOAISPs.ToList());
        }




        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                var loaisp = from lsp in db.LOAISPs where lsp.MALOAI == id select lsp;
                return View(loaisp.SingleOrDefault());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
                return View();

        }

        [HttpPost]
        public ActionResult Create(LOAISP loaisp)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                db.LOAISPs.Add(loaisp);
                db.SaveChanges();
                return RedirectToAction("Index", "LoaiSP");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else

            {
                var loaisp = from lsp in db.LOAISPs where lsp.MALOAI == id select lsp;
                return View(loaisp.SingleOrDefault());
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Xacnhanxoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                LOAISP loaisp = db.LOAISPs.SingleOrDefault(n => n.MALOAI == id);
                db.LOAISPs.Remove(loaisp);
                db.SaveChanges();
                return RedirectToAction("Index", "LoaiSP");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("Login", "Admin");
            else
            {
                var lsp = from loaisp in db.LOAISPs where loaisp.MALOAI == id select loaisp;
                return View(lsp.SingleOrDefault());
            }    
        }

        [HttpPost ,ActionName("Edit")]

        public ActionResult Capnhat(int id)
        {
            LOAISP lsp = db.LOAISPs.Where(n => n.MALOAI == id).SingleOrDefault();
            UpdateModel(lsp);
            db.SaveChanges();
            return RedirectToAction("Index", "LoaiSP");

        }


    }
}
    