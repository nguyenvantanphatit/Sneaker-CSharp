using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Sneaker.Models;
namespace Sneaker.Controllers
{
    public class KhachHangController : Controller
    {
        #region ---------------ĐĂNG KÝ--------------------
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] KHACHHANG khachHang)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {

                #region CHECK MAIL CÓ TỒN TẠI HAY CHƯA
                var isExist = IsEmailExist(khachHang.EMAIL);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(khachHang);
                }
                #endregion

                #region TẠO MÃ KÍCH HOẠT TÀI KHOẢN 
                khachHang.ActivationCode = Guid.NewGuid();
                #endregion

                #region MÃ HÓA MẬT KHẨU 
                khachHang.MATKHAU = Crypto.Hash(khachHang.MATKHAU);
                khachHang.ConfirmPassword = Crypto.Hash(khachHang.ConfirmPassword); //
                #endregion
                khachHang.IsEmailVerified = false; //check mail khách hàng

                #region LƯU DỮ LIỆU VÀO DATABSE
                using (QLBanGiayEntities dc = new QLBanGiayEntities())
                {
                    dc.KHACHHANGs.Add(khachHang);
                    dc.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(khachHang.EMAIL, khachHang.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + khachHang.EMAIL;
                    Status = true;
                    return RedirectToAction("XatNhanEgmai", "KhachHang");
                }
                #endregion

            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(khachHang);
        }
        public ActionResult XatNhanEgmai()
        {
            return View();
        }
        #region XÁC NHẬN TÀI KHOẢN
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (QLBanGiayEntities dc = new QLBanGiayEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;
                    
                var v = dc.KHACHHANGs.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        #endregion

        #region HÀM CHECK MAIL CÓ TỒN TẠI TRONG CƠ SỞ DỮ LIỆU KHÔNG
        [NonAction]
        private bool IsEmailExist(string email)
        {
            using (QLBanGiayEntities dc = new QLBanGiayEntities())
            {
                var v = dc.KHACHHANGs.Where(a => a.EMAIL == email).FirstOrDefault();
                return v != null;
            }
        }

        #endregion
        #region HÀM GỬI LINK XÁC NHẬN MẬT KHẨU CHO KHÁCH HÀNG
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/KhachHang/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("jacknhoxdx3@gmail.com", "Tiệm Giày Giá Rẻ");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "jtkvzmzwikpchunx";
            string subject = "Tài khoản của bạn đã được đăng ký!";

            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        #endregion
        #endregion
        #region--------ĐĂNG NHẬP------------------
        [HttpGet]        
        public ActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(KhachHangDangNhap login, string ReturnUrl = "")
        {
            string message = "";
            using (QLBanGiayEntities dc = new QLBanGiayEntities())
            {
                var v = dc.KHACHHANGs.Where(a => a.TAIKHOAN == login.TAIKHOAN).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Vui lòng xác nhận mail trước khi đăng nhập";
                        return View();
                    }

                    if (string.Compare(Crypto.Hash(login.MATKHAU), v.MATKHAU) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.TAIKHOAN, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            
                            Session["TAIKHOAN"] = v;
                            return RedirectToAction("Index", "Site");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

     //   Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Site");
        }
        #endregion
        #region ------QUÊN MẬT KHẨU---------------
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            using (QLBanGiayEntities dc = new QLBanGiayEntities())
            {
                var account = dc.KHACHHANGs.Where(a => a.EMAIL == Email).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EMAIL, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        #region Reset Password
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (QLBanGiayEntities dc = new QLBanGiayEntities())
            {
                var user = dc.KHACHHANGs.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
        #endregion

        #region 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (QLBanGiayEntities dc = new QLBanGiayEntities())
                {
                    var user = dc.KHACHHANGs.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.MATKHAU = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "Đã cập nhật mật khẩu";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/KhachHang/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("jacknhoxdx3@gmail.com", "TIỆM GIÀY GIÁ RẺ");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "jtkvzmzwikpchunx";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        #endregion
        #endregion
    }
}