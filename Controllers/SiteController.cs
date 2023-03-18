using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sneaker.Models;
using PagedList;
using PagedList.Mvc;
namespace Sneaker.Controllers
{
    public class SiteController : Controller
    {
        QLBanGiayEntities db = new QLBanGiayEntities();


        private List<SANPHAM> Laysanpham(int count)
        {
            return db.SANPHAMs.OrderByDescending(a => a.NGAYSANXUAT).Take(count).ToList();
            
        }
       
        // GET: Site
        public ActionResult Index()
        {
            var spmoi = Laysanpham(12);
            return View(spmoi);

            
        }
       
        public ActionResult Product(int ? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var tcsanpham = db.SANPHAMs.ToList();
            return View(tcsanpham.ToPagedList(pageNum,pageSize));

        }
        public ActionResult LoaiSp()
        {
            var loaisp = from lsp in db.LOAISPs select lsp;
            return PartialView(loaisp);
        }

        
        public ActionResult NhaCC()
        {
            var nhacc = from ncc in db.NCCs select ncc;
            return PartialView(nhacc);
        }

        public ActionResult SPTheoLoai(int id)
        {
            var sp = from s in db.SANPHAMs where s.MALOAI == id select s;
            return View(sp);
        }

        public ActionResult SPTheoNCC(int id)
        {
            var sp = from s in db.SANPHAMs where s.MANCC == id select s;
            return View(sp);
        }


        public ActionResult Details(int id)
        {
            var sp = from s in db.SANPHAMs
                     where s.MASP == id
                     select s;
            return View(sp.Single());
        }
        
    }
}