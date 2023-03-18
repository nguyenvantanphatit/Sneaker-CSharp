using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using Sneaker.Models;
namespace Sneaker.Controllers
{
    public class ExcelController : Controller
    {
        QLBanGiayEntities db = new QLBanGiayEntities();
        // GET: Excel
        public ActionResult ImportExcel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase postedFile)
        {
                   
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.SANPHAM";


                        //Set the database table name.

                        //[OPTIONAL]: Map the Excel columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("MASP", "MASP");
                        sqlBulkCopy.ColumnMappings.Add("TENSP", "TENSP");
                        sqlBulkCopy.ColumnMappings.Add("GIABAN", "GIABAN");
                        sqlBulkCopy.ColumnMappings.Add("ANHBIA", "ANHBIA");
                        sqlBulkCopy.ColumnMappings.Add("MOTA", "MOTA");
                        sqlBulkCopy.ColumnMappings.Add("NGAYSANXUAT", "NGAYSANXUAT");
                        sqlBulkCopy.ColumnMappings.Add("NOISANXUAT", "NOISANXUAT");
                        sqlBulkCopy.ColumnMappings.Add("SL", "SL");
                        sqlBulkCopy.ColumnMappings.Add("MALOAI", "MALOAI");
                        sqlBulkCopy.ColumnMappings.Add("MANCC", "MANCC");

                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();
                    
                        
                    }
                }
            }

            return View();
        }
        public ActionResult ExportExcel()
        {
            
            List<ExcelDonDatHang> emplist = db.DONDATHANGs.Select(x => new ExcelDonDatHang
            
            {
                MADH = x.MADH,
                HOTEN = x.HOTEN,
                TINHTRANGGIAOHANG = x.TINHTRANGGIAOHANG,
                NGAYDAT = x.NGAYDAT,
                NGAYGIAO = x.NGAYGIAO,
                MAKH = x.MAKH
            }).ToList();
            return View(emplist);
        }
        
        public void ExportToExcel()
        {
            List<ExcelDonDatHang> emplist = db.DONDATHANGs.Select(x => new ExcelDonDatHang

            {
                MADH = x.MADH,
                HOTEN = x.HOTEN,
                TINHTRANGGIAOHANG = x.TINHTRANGGIAOHANG,
                NGAYDAT = x.NGAYDAT,
                NGAYGIAO = x.NGAYGIAO,
                MAKH = x.MAKH
            }).ToList();

            ExcelPackage pck = new ExcelPackage();
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Người lập";
            ws.Cells["B1"].Value = "Admin";

            ws.Cells["A2"].Value = "Cửa hàng giày";
            ws.Cells["B2"].Value = "Tấn Phát";

            ws.Cells["A3"].Value = "Date";
            ws.Cells["B3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);

            ws.Cells["A6"].Value = "Mã Đơn Hàng";
            ws.Cells["B6"].Value = "Họ Tên";
            ws.Cells["C6"].Value = "Tình Trạng";
            ws.Cells["D6"].Value = "Ngày Đặt";
            ws.Cells["E6"].Value = "Ngày Giao";
            ws.Cells["F6"].Value = "Mã Khách Hàng";

            int rowStart = 7;
            foreach (var item in emplist)
            {
                
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.MADH;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.HOTEN;
                ws.Cells[string.Format("C{0}", rowStart)].Value = item.TINHTRANGGIAOHANG;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.NGAYDAT.ToString();
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.NGAYGIAO.ToString();
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.MAKH;
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();

        }
        
    }
}