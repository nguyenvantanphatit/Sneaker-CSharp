using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sneaker.Models
{
    public class Giohang
    {
        QLBanGiayEntities db = new QLBanGiayEntities();


        public int iMASP { get; set; }

        public string sTENSP { get; set; }

        public string sANHBIA { get; set; }

        public Double dDONGIA { get; set; }

        public int iSL { get; set; }

        public Double dTHANHTIEN
        {
            get { return iSL * dDONGIA; }
        }

        public Giohang(int MASP)
        {
            iMASP = MASP;
            SANPHAM sanpham = db.SANPHAMs.Single(n => n.MASP == iMASP);
            sTENSP = sanpham.TENSP;
            sANHBIA = sanpham.ANHBIA;
            dDONGIA = double.Parse(sanpham.GIABAN.ToString());
            iSL = 1;
        }
    }
}