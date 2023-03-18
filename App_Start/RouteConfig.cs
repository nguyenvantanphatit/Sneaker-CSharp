using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sneaker
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "GioiThieu",
               url: "gioithieu",
               defaults: new { controller = "GioiThieu", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "TinTuc",
              url: "tintuc",
              defaults: new { controller = "TinTuc", action = "Index", id = UrlParameter.Optional }
          );
            routes.MapRoute(
                name: "LienHe",
                url: "lienhe",
                defaults: new { controller = "LienHe", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "GioHang",
               url: "giohang",
               defaults: new { controller = "GioHang", action = "Index", id = UrlParameter.Optional }
           );
            routes.MapRoute(
             name: "TatCaSP",
             url: "tatcasanpham",
             defaults: new { controller = "Site", action = "Product", id = UrlParameter.Optional }
         );
            routes.MapRoute(
           name: "DangNhap",
           url: "dangnhap",
           defaults: new { controller = "KhachHang", action = "Login", id = UrlParameter.Optional }
           );
            routes.MapRoute(
           name: "DangKy",
           url: "dangky",
           defaults: new { controller = "KhachHang", action = "Register", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Site", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
