using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sneaker.Models
{
    public class KhachHangDangNhap
    {
        //[Display(Name = "Tài Khoản")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập tài khoản")]
        public string TAIKHOAN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MATKHAU { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}