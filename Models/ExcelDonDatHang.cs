using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sneaker.Models
{
    public class ExcelDonDatHang
    {
        public int MADH { get; set; }
        public string HOTEN { get; set; }

        

        public Nullable<bool> TINHTRANGGIAOHANG { get; set; }
        public Nullable<System.DateTime> NGAYDAT { get; set; }
        public Nullable<System.DateTime> NGAYGIAO { get; set; }
        public Nullable<int> MAKH { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CHITIETHOADON> CHITIETHOADONs { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}