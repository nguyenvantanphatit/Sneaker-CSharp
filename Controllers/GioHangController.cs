using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Sneaker.Models;
using Sneaker.order;

namespace Sneaker.Controllers
{

    public class GioHangController : Controller
    {
        QLBanGiayEntities db = new QLBanGiayEntities();
        // GET: GioHang
        public ActionResult GioHang()
        {

            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Site");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        public ActionResult ThemGioHang (int  iMASP, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.Find(n => n.iMASP == iMASP);
            if(sp== null)
            {
                sp = new Giohang(iMASP);
                lstGiohang.Add(sp);
                return Redirect(strURL);
            }
            else 
            {
                sp.iSL++;
                return Redirect(strURL);
            } 
                
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if(lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSL);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if( lstGiohang != null)
            {
                iTongTien = (lstGiohang.Sum(n => n.dTHANHTIEN)) * 70 / 100;
            }
            return iTongTien;
        }
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }
        public ActionResult XoaGioHang (int iMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.SingleOrDefault(n => n.iMASP == iMaSP);

            if(sp!= null)
            {
                lstGiohang.RemoveAll(n => n.iMASP == iMaSP);
                return RedirectToAction("Giohang");
            }    
            if( lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Site");
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sp = lstGiohang.SingleOrDefault(n => n.iMASP == iMaSP);

            if(sp != null)
            {
                sp.iSL = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaTatCa()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Site");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if(Session["TAIKHOAN"]==null || Session["TAIKHOAN"].ToString() == "")
            {
                return RedirectToAction("Login", "KhachHang");
            }

            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);


        }
        public ActionResult DatHang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG) Session["TAIKHOAN"];
            List<Giohang> gh = Laygiohang();
            ddh.MAKH = kh.MAKH;
            ddh.HOTEN = kh.HOTEN;
            ddh.NGAYDAT = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NGAYGIAO"]);
            ddh.NGAYGIAO = DateTime.Parse(ngaygiao);
            ddh.TINHTRANGGIAOHANG = true;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();

            

            foreach (var item in gh)
            {
                CHITIETHOADON ctdh = new CHITIETHOADON();
                ctdh.MADH = ddh.MADH;
                ctdh.MASP = item.iMASP;
                ctdh.SL = item.iSL;
                ctdh.DONGIA = (decimal)item.dDONGIA;
                db.CHITIETHOADONs.Add(ctdh);                               
            }
            db.SaveChanges();
            
            return RedirectToAction("Xacnhandonhang", "Giohang");

        }
        public ActionResult Xacnhandonhang( FormCollection f)
        {
            if (Session["TAIKHOAN"] == null || Session["TAIKHOAN"].ToString() == "")
            {
                return RedirectToAction("Login", "KhachHang");
            }
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View();
           
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Payment()
        {
            string endpoint = "https://payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOQUFO20220604";
            string accessKey = "CBnwSz6apdin7hlg";
            string serectkey = "SH3p93lEUXFTFWigQDaIRGgdlnmvJcRm";
            string orderInfo = "Tấn Phát";
            string returnUrl = "https://localhost:44394/Site/Index";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Site/Index";
            string amount = TongTien().ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();

            string signature = crypto.signSHA256(rawHash, serectkey);


            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount},
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }
            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());           
            JObject jmessage = JObject.Parse(responseFromMomo);
            Session["Giohang"] = null;
            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public ActionResult ConfirmPaymentClient()
        {           
            return View();
        }

        public ActionResult PayPal()
        {
            Session["Giohang"] = null;
            return View();  
        }
    }
}