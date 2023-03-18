using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sneaker.Models
{
    
        [MetadataType(typeof(KHACHHANGMetadata))]
        public partial class KHACHHANG
    {
            public string ConfirmPassword { get; set; }
        }
        public class KHACHHANGMetadata
    {
            //[Display(Name = " Họ Tên ")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập họ tên người đăng ký")]
            public string HOTEN { get; set; }

           // [Display(Name = " Email ")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập Email")]
            [DataType(DataType.EmailAddress)]
            public string EMAIL { get; set; }

           // [Display(Name = " Số Điện Thoại ")]
            [DataType(DataType.PhoneNumber)]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập số điện thọai")]
            [MaxLength(10, ErrorMessage = "Số điện thoại tối đa là 10 ký tự số")]
            public string DIENTHOAIKH { get; set; }

           // [Display(Name = "Địa Chỉ")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập địa chỉ")]
            public string DIACHI { get; set; }

           // [Display(Name = "Tài Khoản")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập tài khoản")]
            public string TAIKHOAN { get; set; }

          //  [Display(Name = "Mật khẩu")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Mật khẩu cần ít nhất 6 ký tự")]
            public string MATKHAU { get; set; }

           // [Display(Name = "Xác nhận mật khẩu")]
            [DataType(DataType.Password)]
            [Compare("MATKHAU", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không trùng khớp")]
            public string ConfirmPassword { get; set; }

        }
    
}